using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;
using Emu.Bento.Common;
using Emu.Bento.Data;
using log4net;

namespace Emu.Bento
{
    /// <summary>
    /// CSV出力
    /// </summary>
    /// <remarks>
    /// CSVTemplateの内容に従ってDataTableをCSVに出力します。
    /// </remarks>
    public static class CSVWriter
    {
        //**************************************
        //* スタティック変数
        //**************************************
        #region スタティック変数
        /// <summary>
        /// ロガー
        /// </summary>
        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        //**************************************
        //* スタティックメソッド
        //**************************************
        #region スタティックメソッド
        /// <summary>
        /// CSV出力
        /// </summary>
        /// <param name="csvTemplateCD">CSVテンプレートCD</param>
        /// <param name="dt">出力する内容が格納されたデータテーブル</param>
        /// <param name="valueMap">出力内容のデータテーブルとは関連付かない内容を格納したマップ</param>
        /// <param name="filePath">出力先ファイルパス</param>
        public static void Output(Consts.CSVTemplates csvTemplateCD, DataTable dt, Dictionary<string, object> valueMap, string filePath)
        {
            // CSVテンプレートCDに該当するテンプレートマスタを取得
            List<CSVTemplate> csvTemplateList = GetCSVTemplateList(csvTemplateCD);

            // テーブル全件を対象に、テンプレートを適用してレコードからCSVを生成する
            List<string> csvList = Generate(dt, valueMap, csvTemplateList);

            // ファイルを出力
            Output(filePath, csvList);
        }

        /// <summary>
        /// テンプレートCDに該当するテンプレートのリストを返します。
        /// </summary>
        /// <param name="csvTemplateCD">CSVテンプレートCD</param>
        /// <returns>CSVテンプレートのリスト</returns>
        private static List<CSVTemplate> GetCSVTemplateList(Consts.CSVTemplates csvTemplateCD)
        {
            try
            {
                List<CSVTemplate> csvTemplateList = new List<CSVTemplate>();    // テンプレートのリスト
                DataTable dt = null;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     テンプレートＣＤ" +
                                      "\r\n     ,項目順" +
                                      "\r\n     ,項目名" +
                                      "\r\n     ,値" +
                                      "\r\n     ,フォーマット" +
                                      "\r\n FROM" +
                                      "\r\n     CSVテンプレートマスタ" +
                                      "\r\n WHERE" +
                                      "\r\n     企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND テンプレートＣＤ = @テンプレートＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     項目順";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@テンプレートＣＤ", SqlDbType.Int).Value = (int)csvTemplateCD;
                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("CSVテンプレートマスタ");
                    adp.Fill(dt);
                }

                // テンプレートを生成し、リストに格納
                foreach (DataRow dr in dt.Rows)
                {
                    CSVTemplate csvTemplate = new CSVTemplate(dr);
                    csvTemplateList.Add(csvTemplate);
                }

                return csvTemplateList;
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// CSVを生成する
        /// </summary>
        /// <param name="dt">出力する内容が格納されたデータテーブル</param>
        /// <param name="valueMap">出力内容のデータテーブルとは関連付かない内容を格納したマップ</param>
        /// <param name="csvTemplateList">CSVテンプレートのリスト</param>
        /// <returns>CSV格納リスト</returns>
        private static List<string> Generate(DataTable dt, Dictionary<string, object> valueMap, List<CSVTemplate> csvTemplateList)
        {
            try
            {
                List<string> csvList = new List<string>();

                // テーブル全件を対象に、テンプレートを適用してレコードからCSVを生成する
                foreach (DataRow dr in dt.Rows)
                {
                    StringBuilder csvLine = new StringBuilder();
                    string comma = "";

                    // フォーマットに従ってCSV文字列を生成
                    foreach (CSVTemplate csvTemplate in csvTemplateList)
                    {
                        CSVTemplate csvTemplate_wk = new CSVTemplate(csvTemplate);  // フォーマット原紙と作業用フォーマットは別にする
                        string value = csvTemplate.値;
                        if (string.IsNullOrEmpty(value) == false)
                        {
                            string[] values = value.Split(new char[] { ',' });  // 複数のリテラルorデータの取得元はカンマで区切られている
                            foreach (string token in values)
                            {
                                // トークンがデータ取得元であれば置き換える
                                string[] tokens = token.Split(new char[] { '.' });
                                if (tokens.Length > 1)
                                {
                                    // データベースから取得したカレントレコード
                                    if (tokens[0] == CSVTemplate.ValueType.D.ToString())
                                    {
                                        if (dr.Table.Columns.Contains(tokens[1]) == false)
                                        {
                                            log.WarnFormat("！！！列名【{0}】はレコードに存在しません！！！", tokens[1]);
                                            throw new BentoException(string.Format("CSVテンプレートが不正です。({0})\nCSVが作成できません。", token));
                                        }
                                        else
                                        {
                                            csvTemplate_wk.出力用値リスト.Add(dr[tokens[1]]);
                                        }
                                    }

                                    // マップ
                                    if (tokens[0] == CSVTemplate.ValueType.V.ToString())
                                    {
                                        if (valueMap.ContainsKey(tokens[1]) == false)
                                        {
                                            log.WarnFormat("！！！キー【{0}】はマップに存在しません！！！", tokens[1]);
                                            throw new BentoException(string.Format("CSVテンプレートが不正です。({0})\nCSVが作成できません。", token));
                                        }
                                        else
                                        {
                                            csvTemplate_wk.出力用値リスト.Add(valueMap[tokens[1]]);
                                        }
                                    }
                                }
                                else
                                {
                                    // トークンがリテラルであればそのまま採用する
                                    csvTemplate_wk.出力用値リスト.Add(token);
                                }
                            }
                        }

                        // テンプレートによる編集結果をCSV行に追記
                        csvLine.AppendFormat("{0}{1}", comma, csvTemplate_wk.ToString());
                        comma = ",";
                    }

                    // 完成したCSV行をリストに追加
                    csvList.Add(csvLine.ToString());
                }

                return csvList;
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// 請求EBデータファイルを出力する
        /// </summary>
        /// <param name="filePath">出力先ファイルパス</param>
        /// <param name="csvList">出力内容のリスト</param>
        private static void Output(string filePath, List<string> csvList)
        {
            try
            {
                // EBデータファイル出力
                using (StreamWriter sw = new StreamWriter(filePath, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    foreach (string text in csvList)
                    {
                        sw.WriteLine(text);
                    }
                    sw.Flush();
                    sw.Close();
                }

                log.Info("CSVを出力しました。");
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }
        #endregion
    }
}
