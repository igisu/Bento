using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using Emu.Bento.Common;
using log4net;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 請求EBデータ（EBレコードフォーマットの集合）
    /// </summary>
    public class EBContainer
    {
        //**************************************
        //* スタティック変数
        //**************************************
        #region スタティック変数
        /// <summary>
        /// ロガー
        /// </summary>
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// 振替結果コードの内容を返す
        /// </summary>
        /// <param name="resultCode">振替結果コード</param>
        /// <returns>振替結果コードに該当する内容</returns>
        public static string GetReuslt(string resultCode)
        {
            string result = "(不明)";
            switch (resultCode)
            {
                case "0":       // 振替済み
                    result = "振替済み";
                    break;

                case "1":       // 資金不足
                    result = "資金不足";
                    break;

                case "2":       // 取引なし
                    result = "取引なし";
                    break;

                case "3":       // 預金者の都合による振替停止
                    result = "預金者の都合による振替停止";
                    break;

                case "4":       // 預金口座振替依頼書なし
                    result = "預金口座振替依頼書なし";
                    break;

                case "8":       // 委託者の都合による振替停止
                    result = "委託者の都合による振替停止";
                    break;

                case "9":       // その他または請求不能データ（エラー・データ）
                    result = "その他または請求不能データ（エラー・データ）";
                    break;

                case "E":       // データエラー
                    result = "データエラー";
                    break;

                case "N":       // 振替結果未着
                    result = "振替結果未着";
                    break;
            }
            return result;
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// ヘッダレコードフォーマット（データ区分=1）
        /// </summary>
        private List<EBFormat> _header;

        /// <summary>
        /// データレコードフォーマット（データ区分=2）
        /// </summary>
        private List<EBFormat> _data;

        /// <summary>
        /// トレーラーレコードフォーマット（データ区分=8）
        /// </summary>
        private List<EBFormat> _trailer;

        /// <summary>
        /// エンドレコードフォーマット（データ区分=9）
        /// </summary>
        private List<EBFormat> _end;

        /// <summary>
        /// データレコードの項目名と値のリスト（パースした結果を格納する）
        /// </summary>
        private List<Dictionary<string, string>> _dataList;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ（EB文字列作成用）
        /// </summary>
        /// <param name="bankCD">銀行CD</param>
        public EBContainer(string bankCD)
        {
            _header = new List<EBFormat>();
            _data = new List<EBFormat>();
            _trailer = new List<EBFormat>();
            _end = new List<EBFormat>();

            // EBレコードフォーマット取得
            GetEBFormat(bankCD);
        }

        /// <summary>
        /// コンストラクタ（EB文字列読み込み用）
        /// </summary>
        /// <param name="bankCD">銀行CD</param>
        /// <param name="EBStr">EB文字列</param>
        public EBContainer(string bankCD, string EBStr) : this(bankCD)
        {
            // パースした結果を格納するリストを生成
            _dataList = new List<Dictionary<string, string>>();

            // EB文字列を読み込む
            ReadEBStr(EBStr);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ

        /// <summary>
        /// データレコードの項目名と値のリスト（パースした結果）
        /// </summary>
        public List<Dictionary<string, string>> DataList
        {
            get
            {
                return _dataList;
            }
        }
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// EBレコードフォーマット取得
        /// </summary>
        /// <param name="bankCD">銀行CD</param>
        private void GetEBFormat(string bankCD)
        {
            try
            {
                DataTable dt = null;
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     銀行ＣＤ" +
                                      "\r\n     ,データ区分" +
                                      "\r\n     ,項目順" +
                                      "\r\n     ,項目名" +
                                      "\r\n     ,文字数" +
                                      "\r\n     ,値" +
                                      "\r\n     ,左寄せフラグ" +
                                      "\r\n FROM" +
                                      "\r\n     EBレコードフォーマット" +
                                      "\r\n WHERE" +
                                      "\r\n     銀行ＣＤ = @銀行ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     データ区分" +
                                      "\r\n     ,項目順";

                    cmd.Parameters.Add("@銀行ＣＤ", SqlDbType.VarChar).Value = bankCD;
                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("EBレコードフォーマット");
                    adp.Fill(dt);
                }

                // データ区分ごとのリストに格納
                foreach (DataRow dr in dt.Rows)
                {
                    EBFormat ebFormat = new EBFormat(dr);
                    switch (ebFormat.データ区分)
                    {
                        case (int)EBFormat.DataKubun.Header:    // ヘッダー
                            _header.Add(ebFormat);
                            break;

                        case (int)EBFormat.DataKubun.Data:      // データ
                            _data.Add(ebFormat);
                            break;

                        case (int)EBFormat.DataKubun.Trailer:   // トレーラー
                            _trailer.Add(ebFormat);
                            break;

                        case (int)EBFormat.DataKubun.End:       // エンド
                            _end.Add(ebFormat);
                            break;
                    }
                }
            }
            catch (EmuException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new EmuException(ex);
            }
        }

        /// <summary>
        /// ヘッダ文字列を生成
        /// </summary>
        /// <param name="valueMap">EBレコードに出力する値を格納したマップ</param>
        /// <returns></returns>
        public string GetHeaderString(Dictionary<string, string> valueMap)
        {
            return GetString(_header, null, valueMap);
        }

        /// <summary>
        /// データ文字列を生成
        /// </summary>
        /// <param name="dr">データレコード</param>
        /// <param name="valueMap">EBレコードに出力する値を格納したマップ</param>
        /// <returns></returns>
        public string GetDataString(DataRow dr, Dictionary<string, string> valueMap)
        {
            return GetString(_data, dr, valueMap);
        }

        /// <summary>
        /// トレーラー文字列を生成
        /// </summary>
        /// <param name="valueMap">EBレコードに出力する値を格納したマップ</param>
        /// <returns></returns>
        public string GetTrailerString(Dictionary<string, string> valueMap)
        {
            return GetString(_trailer, null, valueMap);
        }

        /// <summary>
        /// エンド文字列を生成
        /// </summary>
        /// <param name="valueMap">EBレコードに出力する値を格納したマップ</param>
        /// <returns></returns>
        public string GetEndString(Dictionary<string, string> valueMap)
        {
            return GetString(_end, null, valueMap);
        }

        /// <summary>
        /// EBレコードフォーマットに従って、データレコードまたはマップの値を埋め込んでEBレコード文字列を作成して返します。
        /// </summary>
        /// <param name="ebFormatList">EBレコードフォーマットのリスト</param>
        /// <param name="dr">データレコード</param>
        /// <param name="valueMap">EBレコードに出力する値を格納したマップ</param>
        /// <returns>EBレコード文字列</returns>
        private string GetString(List<EBFormat> ebFormatList, DataRow dr, Dictionary<string, string> valueMap)
        {
            try
            {
                StringBuilder ebData = new StringBuilder();

                // フォーマットに従ってレコード文字列を生成
                foreach (EBFormat ebFormat in ebFormatList)
                {
                    EBFormat ebFormat_wk = new EBFormat(ebFormat);  // フォーマット原紙と作業用フォーマットは別にする
                    string data = "";
                    string value = ebFormat.値;
                    string[] values = value.Split(new char[] { '.' });
                    // 値が指定されていれば置き換える。指定されていなければそのまま使う
                    if (values.Length > 1)
                    {
                        // データベースから取得したカレントレコード
                        if (values[0] == EBFormat.ValueType.D.ToString())
                        {
                            if (dr.Table.Columns.Contains(values[1]) == false)
                            {
                                log.WarnFormat("！！！列名【{0}】はレコードに存在しません！！！", values[1]);
                                throw new EmuException(string.Format("EBレコードフォーマットが不正です。({0})\nEBデータが作成できません。", value));
                            }
                            else
                            {
                                data = dr[values[1]].ToString();
                            }
                        }

                        // マップ
                        if (values[0] == EBFormat.ValueType.V.ToString())
                        {
                            if (valueMap.ContainsKey(values[1]) == false)
                            {
                                log.WarnFormat("！！！キー【{0}】はマップに存在しません！！！", values[1]);
                                throw new EmuException(string.Format("EBレコードフォーマットが不正です。({0})\nEBデータが作成できません。", value));
                            }
                            else
                            {
                                data = valueMap[values[1]];
                            }
                        }

                        ebFormat_wk.値 = data;
                    }

                    ebData.Append(ebFormat_wk.ToString());
                }

                return ebData.ToString();
            }
            catch (EmuException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new EmuException(ex);
            }
        }

        /// <summary>
        /// EB文字列を読み込む
        /// </summary>
        /// <param name="EBStr">EB文字列</param>
        private void ReadEBStr(string EBStr)
        {
            try
            {
                string[] lines = EBStr.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    switch (line.Substring(0, 1))
                    {
                        case "1":           // ヘッダー
                            _header = SetString(_header, line);
                            break;

                        case "2":           // データ
                            // データレコードは複数存在するので、フォーマット原紙には格納しない
                            List<EBFormat> data = SetString(_data, line);
                            Dictionary<string, string> map = new Dictionary<string, string>();
                            foreach (EBFormat ebFormat in data)
                            {
                                map[ebFormat.項目名] = ebFormat.値;
                            }
                            _dataList.Add(map);
                            break;

                        case "8":           // トレーラー
                            _trailer = SetString(_trailer, line);
                            break;

                        case "9":           // エンド
                            _end = SetString(_end, line);
                            break;
                    }
                }
            }
            catch (EmuException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new EmuException(ex);
            }
        }

        /// <summary>
        /// EB文字列の内容をEBレコードフォーマットの値に格納して返します。
        /// </summary>
        /// <param name="ebFormatList">EBレコードフォーマットのリスト</param>
        /// <param name="line">EB文字列</param>
        /// <returns>パース結果が格納されたEBレコードフォーマットのリスト</returns>
        private List<EBFormat> SetString(List<EBFormat> ebFormatList, string line)
        {
            List<EBFormat> parsedEBFormatList = new List<EBFormat>();

            try
            {
                int index = 0;
                // フォーマットに従って部分文字列を取り出し値にセット
                foreach (EBFormat ebFormat in ebFormatList)
                {
                    EBFormat ebFormat_wk = new EBFormat(ebFormat);          // フォーマット原紙と作業用フォーマットは別にする
                    string s = line.Substring(index, ebFormat_wk.文字数);   // フォーマットが持つ文字数分の文字列を取り出して…
                    ebFormat_wk.値 = s;                                     // それを値として持たせる
                    index += ebFormat_wk.文字数;                            // インデックスを進める
                    parsedEBFormatList.Add(ebFormat_wk);                    // パースした結果をリストに格納
                }
                return parsedEBFormatList;
            }
            catch (EmuException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new EmuException(ex);
            }
        }
        #endregion
    }
}
