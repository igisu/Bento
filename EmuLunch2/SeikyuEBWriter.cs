using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using Emu.Bento.Common;
using Emu.Bento.Data;
using log4net;

namespace Emu.Bento
{
    /// <summary>
    /// 請求EBデータファイル出力
    /// </summary>
    public class SeikyuEBWriter
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
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        private int _部署ＣＤ;
        private string _銀行ＣＤ;
        private string _支店ＣＤ;
        private DateTime _引落日;

        // 部署EB設定マスタの内容を格納するマップ
        private Dictionary<string, string> _valueMap = new Dictionary<string, string>();

        // EBレコード文字列格納リスト
        private List<string> _texts = new List<string>();

        // 請求番号リスト（EB出力済みフラグを更新用）
        private List<decimal> _seikyuBangoList = new List<decimal>();

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="busyoCD">部署ＣＤ</param>
        /// <param name="bankCD">銀行ＣＤ</param>
        /// <param name="branchCD">支店ＣＤ</param>
        /// <param name="hikiotoshiBi">引落日</param>
        public SeikyuEBWriter(int busyoCD, string bankCD, string branchCD, DateTime hikiotoshiBi)
        {
            _部署ＣＤ = busyoCD;
            _銀行ＣＤ = bankCD;
            _支店ＣＤ = branchCD;
            _引落日 = hikiotoshiBi;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        /// <summary>
        /// 金額（出力ファイル名の決定に用いる）
        /// </summary>
        public string 金額
        {
            get
            {
                if (_valueMap.ContainsKey("金額") == false)
                {
                    return "";
                }
                return _valueMap["金額"];
            }
        }

        /// <summary>
        /// 件数（出力ファイル名の決定に用いる）
        /// </summary>
        public string 件数
        {
            get
            {
                if (_valueMap.ContainsKey("件数") == false)
                {
                    return "";
                }
                return _valueMap["件数"];
            }
        }

        /// <summary>
        /// DB更新時の(エラー)メッセージ
        /// </summary>
        public string Message { get; private set; }
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 請求EBデータファイルを生成する
        /// </summary>
        /// <param name="dtSeikyu">出力対象のデータが格納されたデータテーブル</param>
        /// <returns>出力に失敗した場合はfalse</returns>
        public bool Generate(DataTable dtSeikyu)
        {
            try
            {
                Message = null;

                // 請求データチェック
                if (CheckSeikyuData(dtSeikyu) == false)
                {
                    return false;
                }

                // 再振替日(ゆうちょ用)を算出
                DateTime saifurikaebi = _引落日.AddDays(10);
                // 土日なら翌月曜にする
                if (saifurikaebi.DayOfWeek == DayOfWeek.Saturday)
                {
                    saifurikaebi = saifurikaebi.AddDays(2);
                }
                if (saifurikaebi.DayOfWeek == DayOfWeek.Sunday)
                {
                    saifurikaebi = saifurikaebi.AddDays(1);
                }

                // 部署EB設定マスタを取得
                BusyoEB busyoEB = BusyoEB.GetBusyoEB(_部署ＣＤ, _銀行ＣＤ, _支店ＣＤ);
                // 取得した部署EB設定マスタの内容をマップに格納（EBヘッダレコードに出力用）
                _valueMap = new Dictionary<string, string>();
                busyoEB.GetProperties(_valueMap);

                // 引落日と再振替日(ゆうちょ用)をマップに格納
                _valueMap["引落日"] = _引落日.ToString("MMdd");
                _valueMap["再振替日"] = saifurikaebi.ToString("MMdd");

                // EBフォーマット読み込み
                EBContainer ebContainer = new EBContainer(_銀行ＣＤ);

                // EBレコード生成
                _texts = new List<string>();                            // EBレコード文字列格納リストを生成
                _seikyuBangoList = new List<decimal>();                 // 請求番号リスト（EB出力済みフラグを更新用）
                _texts.Add(ebContainer.GetHeaderString(_valueMap));       // ヘッダレコード作成
                decimal totalAmount = 0m;
                foreach (DataRow dr in dtSeikyu.Rows)
                {
                    _seikyuBangoList.Add((decimal)dr["請求番号"]);          // 請求番号リストに追加

                    _texts.Add(ebContainer.GetDataString(dr, _valueMap));    // データレコード作成

                    totalAmount += (decimal)dr["請求金額"];                 // 合計金額
                }
                _valueMap["金額"] = totalAmount.ToString();
                _valueMap["件数"] = dtSeikyu.Rows.Count.ToString();
                _texts.Add(ebContainer.GetTrailerString(_valueMap));     // トレーラーレコード作成
                _texts.Add(ebContainer.GetEndString(_valueMap));         // エンドレコード作成

                log.Info("請求EBデータを出力しました。");

                return true;
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
        /// 請求EBデータファイルを出力する
        /// </summary>
        /// <param name="filePath">出力先ファイルパス</param>
        public void Output(string filePath)
        {
            try
            {
                // EBデータファイル出力
                using (StreamWriter sw = new StreamWriter(filePath, false, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    foreach (string text in _texts)
                    {
                        sw.WriteLine(text);
                    }
                    sw.Flush();
                    sw.Close();
                }

                log.Info("請求EBデータを出力しました。");
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
        /// 請求合計のEB出力済みフラグを更新
        /// </summary>
        /// <param name="title">呼び出し元画面のタイトル（操作履歴出力用）</param>
        public bool UpdateSeikyuTotal(string title)
        {
            try
            {
                Message = null;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();
                    try
                    {
                        foreach (decimal seikyuNo in _seikyuBangoList)
                        {
                            SqlCommand cmd = con.CreateCommand();
                            cmd.Transaction = tran;
                            cmd.CommandText = "UPDATE 請求合計 SET EB作成済フラグ = 1 WHERE 企業ＣＤ = @企業ＣＤ AND 請求番号 = @請求番号";
                            cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                            cmd.Parameters.Add("@請求番号", SqlDbType.Decimal).Value = seikyuNo;
                            log.Debug(cmd.CommandTextWithParameters());
                            cmd.ExecuteNonQuery();

                            // 操作履歴に保存
                            History.Save(con, tran, title, "請求合計", seikyuNo.ToString(), History.ProcessType.Update);
                        }
                        tran.Commit();
                    }
                    catch (SqlException ex)
                    {
                        log.Error(ex);
                        tran.Rollback();
                        if (ex.Number == 2627 || ex.Number == 2601)
                        {
                            Message = "請求合計テーブルが重複しています。";
                        }
                        else
                        {
                            History.Error(ex);
                            Message = string.Format("請求合計テーブルの{0}に失敗しました。\n{1}", History.ProcessType.Update.DisplayName(), ex.ToString());
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        tran.Rollback();
                        History.Error(ex);
                        Message = string.Format("請求合計の{0}に失敗しました。\n{1}", History.ProcessType.Update.DisplayName(), ex.ToString());
                        return false;
                    }
                }
                return true;
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
        /// 請求データをチェックします。
        /// </summary>
        /// <param name="dtSeikyu">請求データ</param>
        /// <returns>チェックOKの場合はtrue。それ以外はfalse</returns>
        private bool CheckSeikyuData(DataTable dtSeikyu)
        {
            try
            {
                foreach (DataRow dr in dtSeikyu.Rows)
                {
                    // 以下の項目をチェックする
                    // ・金融機関ＣＤ AS 銀行ＣＤ
                    // ・銀行名カナ
                    // ・金融機関支店ＣＤ AS 支店ＣＤ
                    // ・支店名カナ
                    // ・口座種別
                    // ・口座番号
                    // ・口座名義人
                    string tokuisakiName = dr["得意先名"] as string;
                    string bankCD = dr["銀行ＣＤ"] as string;
                    string bankName = dr["銀行名カナ"] as string;
                    string branchCD = dr["支店ＣＤ"] as string;
                    string branchName = dr["支店名カナ"] as string;
                    int? kozaSyubetsu = dr["口座種別"] as int?;
                    string kozaNo = dr["口座番号"] as string;
                    string kozaName = dr["口座名義人"] as string;

                    if (string.IsNullOrEmpty(bankCD))
                    {
                        Message = string.Format("得意先「{0}」の金融機関ＣＤが未入力です。処理を中止します。", tokuisakiName);
                        log.Warn(Message);
                        return false;
                    }

                    if (string.IsNullOrEmpty(bankName))
                    {
                        Message = string.Format("得意先「{0}」の金融機関ＣＤ【{1}】は存在しません。処理を中止します。", tokuisakiName, bankCD);
                        log.Warn(Message);
                        return false;
                    }

                    if (string.IsNullOrEmpty(branchCD))
                    {
                        Message = string.Format("得意先「{0}」の金融機関支店ＣＤが未入力です。処理を中止します。", tokuisakiName);
                        log.Warn(Message);
                        return false;
                    }

                    if (string.IsNullOrEmpty(branchName))
                    {
                        Message = string.Format("得意先「{0}」の金融機関支店ＣＤ【{1}】は存在しません。処理を中止します。", tokuisakiName, branchCD);
                        log.Warn(Message);
                        return false;
                    }

                    if (kozaSyubetsu == null)
                    {
                        Message = string.Format("得意先「{0}」の口座種別が未入力です。処理を中止します。", tokuisakiName);
                        log.Warn(Message);
                        return false;
                    }

                    if (string.IsNullOrEmpty(kozaNo))
                    {
                        Message = string.Format("得意先「{0}」の口座番号が未入力です。処理を中止します。", tokuisakiName);
                        log.Warn(Message);
                        return false;
                    }

                    // 口座名義人が半角かどうかチェック
                    Tokuisaki tokuisaki = new Tokuisaki();
                    tokuisaki.口座名義人 = kozaName;
                    if (string.IsNullOrEmpty(tokuisaki["口座名義人"]) == false)
                    {
                        Message = string.Format("得意先「{0}」の口座名義人が半角ではありません。処理を中止します。", tokuisakiName);
                        log.Warn(Message);
                        return false;
                    }
                }
                return true;
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
