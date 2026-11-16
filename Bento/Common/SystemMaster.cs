using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Emu.Bento.Data;

namespace Emu.Bento.Common
{
    /// <summary>
    /// システム設定
    /// </summary>
    public static class SystemMaster
    {
        /// <summary>
        /// 画像の拡張子
        /// </summary>
        private static string[] exts = { "png", "gif", "jpg", "jpe", "jpeg", "bmp" };

        /// <summary>
        /// システム設定の内容を取得
        /// </summary>
        /// <param name="key">システム設定のキー</param>
        /// <returns>システム設定の内容</returns>
        public static string Get(string key)
        {
            string value = "";

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = " SELECT" +
                                    "     設定値" +
                                    " FROM" +
                                    "     システム設定" +
                                    " WHERE" +
                                    "     企業ＣＤ = @企業ＣＤ" +
                                    " AND 設定名   = @設定名";
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@設定名", SqlDbType.VarChar).Value = key;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        value = reader["設定値"].ToString();
                    }
                    else
                    {
                        throw new ArgumentException(key + " は システム設定に存在しません");
                    }
                }

                return value;
            }
        }

        /// <summary>
        /// 部署とプレフィックスに該当する画像のフルパスを返す
        /// </summary>
        /// <param name="busyoCD">部署CD</param>
        /// <param name="folderKey">画像格納フォルダのシステム設定キー</param>
        /// <param name="prefix">ファイル名のプレフィックス</param>
        /// <returns>システム設定テーブルにあるパスと部署CDから求めたファイルパス</returns>
        private static string GetImagePath(int? busyoCD, string folderKey, string prefix)
        {
            string folder = Get(folderKey);
            string logoPath = "";
            foreach (string ext in exts)
            {
                string fileName = string.Format("{0}_{1}.{2}", prefix, (busyoCD ?? 0), ext);
                logoPath = Path.Combine(folder, fileName);
                if (File.Exists(logoPath))
                {
                    break;
                }
            }
            return logoPath;
        }

        /// <summary>
        /// 受注発信一覧での再読込間隔(秒)を返します
        /// </summary>
        /// <returns>受注発信一覧での再読込間隔(秒)</returns>
        public static int Get204RefreshInterval()
        {
            int ret = GetInt("受注発信一覧再読込間隔秒");
            if (ret < 0)
            {
                ret = 0;
            }
            return ret;
        }

        /// <summary>
        /// 請求書宛先左位置オフセットを返します
        /// </summary>
        /// <returns>請求書宛先左位置オフセット(cm)</returns>
        public static float GetSeikyusyoAddrLeftOffset()
        {
            float ret = GetFloat("請求書宛先左位置オフセットcm");
            return ret;
        }

        /// <summary>
        /// 請求書宛先上位置オフセットを返します
        /// </summary>
        /// <returns>請求書宛先上位置オフセット(cm)</returns>
        public static float GetSeikyusyoAddrTopOffset()
        {
            float ret = GetFloat("請求書宛先上位置オフセットcm");
            return ret;
        }

        /// <summary>
        /// 入金入力での表示期間（過去ｎヶ月）を返します
        /// </summary>
        /// <returns>受注発信一覧での再読込間隔(秒)</returns>
        public static int Get601ShowMonths()
        {
            int ret = GetInt("入金入力表示期間月数");
            if (ret <= 0)
            {
                ret = 1;    // 最小は１ヶ月
            }
            return ret;
        }

        /// <summary>
        /// メニュー非活性フォームIDのリストを返します
        /// </summary>
        /// <returns>メニュー非活性フォームIDのリスト</returns>
        public static List<int> GetDisableMenuIDs()
        {
            return GetCSVList("メニュー非活性フォームID群");
        }

        /// <summary>
        /// 未来日警告閾値を返します
        /// </summary>
        /// <returns>未来日警告閾値</returns>
        public static int GetWarningFutureDays()
        {
            int ret = GetInt("未来日警告閾値");
            if (ret <= 0)
            {
                ret = 90;    // デフォルトは90日
            }
            return ret;
        }

        /// <summary>
        /// 未来月数警告閾値を返します
        /// </summary>
        /// <returns>未来日警告閾値を30で割ったもの（＝月数）</returns>
        public static int GetWarningFutureMonths()
        {
            int ret = GetWarningFutureDays();
            ret /= 30;  // 日数を月数に変換
            return ret;
        }

        /// <summary>
        /// 主要商品最大数を返します
        /// </summary>
        /// <returns>主要商品最大数</returns>
        public static int GetSyuyoSyohinMax()
        {
            int ret = GetInt("主要商品最大数");
            if (ret <= 0)
            {
                ret = 6;    // デフォルトは6
            }
            return ret;
        }

        /// <summary>
        /// 主要部品最大数を返します
        /// </summary>
        /// <returns>主要部品最大数</returns>
        public static int GetSyuyoBuhinMax()
        {
            int ret = GetInt("主要部品最大数");
            if (ret <= 0)
            {
                ret = 6;    // デフォルトは6
            }
            return ret;
        }

        /// <summary>
        /// 請求書に回収予定日を出力するかどうかのフラグを返します
        /// </summary>
        /// <returns>
        /// 請求書に回収予定日を出力する場合はtrue。
        /// 請求書に回収予定日を出力しない場合はfalse。
        /// </returns>
        public static bool IsPrintSeikyuKaisyuYotei()
        {
            return GetFlag("請求書回収予定日印字フラグ");
        }

        /// <summary>
        /// システム接続端末数を返します
        /// </summary>
        /// <returns>システム接続端末数</returns>
        public static int GetLicenseCount()
        {
            try
            {
                int ret = GetInt("システム接続端末数");
                return ret;
            }
            catch (ArgumentException)
            {
                // キーが存在しない場合はログイン可能と見なす
                return 0;
            }
        }

        /// <summary>
        /// 祝日CSV取得URLを返します
        /// </summary>
        /// <returns>祝日CSV取得URL</returns>
        public static string GetHoridayCsvURL()
        {
            return Get("祝日CSV取得URL");
        }

        /// <summary>
        /// 祝日CSV取得タイムアウト秒を返します
        /// </summary>
        /// <returns>祝日CSV取得タイムアウト秒</returns>
        public static int GetHoridayCsvTimeoutSec()
        {
            int ret = GetInt("祝日CSV取得タイムアウト秒");
            if (ret <= 0)
            {
                ret = 10;   // デフォルトは10
            }
            return ret;
        }

        /// <summary>
        /// 短冊シール注文単位枚数を返します
        /// </summary>
        /// <returns>短冊シール注文単位枚数</returns>
        public static int GetSealMaisu()
        {
            try
            {
                int ret = GetInt("短冊シール注文単位枚数");
                return ret;
            }
            catch (ArgumentException)
            {
                // キーが存在しない場合はデフォルト値
                return 15000;
            }
        }

        /// <summary>
        /// 得意先ＣＤ表示フラグを返します
        /// </summary>
        /// <returns>得意先ＣＤ表示フラグ</returns>
        public static bool GetDispTokuisakiCD()
        {
            try
            {
                bool ret = GetFlag("得意先ＣＤ表示フラグ");
                return ret;
            }
            catch (ArgumentException)
            {
                // キーが存在しない場合はデフォルト値
                return false;
            }
        }

        /// <summary>
        /// 得意先名略称表示フラグを返します
        /// </summary>
        /// <returns>得意先名略称表示フラグ</returns>
        public static bool GetDispTokuisakiRyaku()
        {
            try
            {
                bool ret = GetFlag("得意先名略称表示フラグ");
                return ret;
            }
            catch (ArgumentException)
            {
                // キーが存在しない場合はデフォルト値
                return false;
            }
        }

        /// <summary>
        /// 領収書印鑑左位置オフセットを返します
        /// </summary>
        /// <returns>領収書印鑑左位置オフセット(cm)</returns>
        public static float GetReceiptStampLeftOffset()
        {
            try
            {
                float ret = GetFloat("領収書印鑑左位置オフセットcm");
                return ret;
            }
            catch (ArgumentException)
            {
                return 0.0f;
            }
        }

        /// <summary>
        /// 領収書印鑑上位置オフセットを返します
        /// </summary>
        /// <returns>領収書印鑑上位置オフセット(cm)</returns>
        public static float GetReceiptStampTopOffset()
        {
            try
            {
                float ret = GetFloat("領収書印鑑上位置オフセットcm");
                return ret;
            }
            catch (ArgumentException)
            {
                return 0.0f;
            }
        }

        /// <summary>
        /// １個口当たりの数量を返します
        /// </summary>
        /// <returns>出荷番号あたりの短冊を印刷する枚数に使用する</returns>
        public static int GetQuantityPerBox()
        {
            try
            {
                int ret = GetInt("１箱あたりの数量");
                return ret;
            }
            catch (ArgumentException)
            {
                // キーが存在しない場合はデフォルト値
                return 0;
            }
        }

        /// <summary>
        /// 会計連携CSV出力対象パターンを返します
        /// </summary>
        /// <returns></returns>
        public static int GetForm840Pattern()
        {
            try
            {
                int ret = GetInt("会計連携CSV出力対象パターン");
                return ret;
            }
            catch (ArgumentException)
            {
                // キーが存在しない場合はデフォルト値
                return 0;
            }
        }

        /// <summary>
        /// 請求書の「内消費税」見出しを出力するかどうかのフラグを返します
        /// </summary>
        /// <returns>請求書の消費税の見出しを「内消費税」にしたい場合はtrue。それ以外はfalse。</returns>
        public static bool IsPrintTaxIncludeTitle()
        {
            try
            {
                bool ret = GetFlag("軽減税率対応_内消費税見出し");
                return ret;
            }
            catch (ArgumentException)
            {
                // キーが存在しない場合はデフォルト値
                return false;
            }
        }

        /// <summary>
        /// 軽減税率の商品を示す「※」を、全ての商品が軽減税率対象であっても出力するかどうかのフラグを返します
        /// </summary>
        /// <returns>軽減税率の商品であれば「※」を出力するのが原則。しかし、全ての商品が軽減税率対象の場合に「※」を出力したくない場合はfalse。出力する場合はtrue。</returns>
        /// <remarks>
        /// ┌─────┬─────────┬─────────┬─────────────────┐
        /// │フラグの値│全て軽減税率対象  │全て通常税率対象  │両者が混在                        │
        /// ├─────┼─────────┼─────────┼─────────────────┤
        /// │true      │「※」を出力する  │「※」は出力しない│軽減税率対象にのみ「※」を出力する│
        /// ├─────┼─────────┼─────────┼─────────────────┤
        /// │false     │「※」は出力しない│「※」は出力しない│軽減税率対象にのみ「※」を出力する│
        /// └─────┴─────────┴─────────┴─────────────────┘
        /// </remarks>
        public static bool IsPrintMarkWhenAllReducedTaxRate()
        {
            try
            {
                bool ret = GetFlag("軽減税率対応_全て軽減対象でも※を出力");
                return ret;
            }
            catch (ArgumentException)
            {
                // キーが存在しない場合はデフォルト値
                return false;
            }
        }

        /// <summary>
        /// 領収書に税率ごとの内訳を出力するかどうかのフラグを返します
        /// </summary>
        /// <returns>領収書に税率ごとの内訳を出力する場合はtrue。しない場合はfalse。</returns>
        public static bool IsPrintRateDetailForReceipt()
        {
            try
            {
                bool ret = GetFlag("軽減税率対応_領収書税率ごと内訳出力");
                return ret;
            }
            catch (ArgumentException)
            {
                // キーが存在しない場合はデフォルト値
                return false;
            }
        }

        /// <summary>
        /// LINE決済用中間サーバWebAPIURLを返します
        /// </summary>
        /// <returns>LINE決済用中間サーバWebAPIURL</returns>
        public static string GetWebAPIURL()
        {
            try
            {
                return Get("LINE決済用中間サーバWebAPIURL");
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        /// <summary>
        /// LINE決済用中間サーバコントローラーを返します
        /// </summary>
        /// <returns>LINE決済用中間サーバWebAPIコントローラー</returns>
        public static string GetWebAPIController()
        {
            try
            {
                return Get("LINE決済用中間サーバWebAPIコントローラー");
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        /// <summary>
        /// 請求締め処理タイムアウト秒数を返します
        /// </summary>
        /// <returns>請求締め処理タイムアウト秒数</returns>
        public static int GetTimeoutSecForSeikyuShimeExec()
        {
            try
            {
                int ret = GetInt("請求締め処理タイムアウト秒");
                if (ret <= 0) ret = 30;
                return ret;
            }
            catch (ArgumentException)
            {
                // キーが存在しない場合はデフォルト値
                return 30;
            }
        }

        /// <summary>
        /// 請求書発行タイムアウト秒数を返します
        /// </summary>
        /// <returns>請求書発行タイムアウト秒数</returns>
        public static int GetTimeoutSecForSeikyuPrint()
        {
            try
            {
                int ret = GetInt("請求書発行タイムアウト秒");
                if (ret <= 0) ret = 30;
                return ret;
            }
            catch (ArgumentException)
            {
                // キーが存在しない場合はデフォルト値
                return 30;
            }
        }

        /// <summary>
        /// 電子決済請求締め処理タイムアウト秒数を返します
        /// </summary>
        /// <returns>電子決済請求締め処理タイムアウト秒数</returns>
        public static int GetTimeoutSecForPaySeikyuShimeExec()
        {
            try
            {
                int ret = GetInt("電子決済請求締め処理タイムアウト秒");
                if (ret <= 0) ret = 30;
                return ret;
            }
            catch (ArgumentException)
            {
                // キーが存在しない場合はデフォルト値
                return 30;
            }
        }

        /// <summary>
        /// 月次集計タイムアウト秒数を返します
        /// </summary>
        /// <returns>月次集計タイムアウト秒数</returns>
        public static int GetTimeoutSecForGetsujiSyukei()
        {
            try
            {
                int ret = GetInt("月次集計タイムアウト秒");
                if (ret <= 0) ret = 30;
                return ret;
            }
            catch (ArgumentException)
            {
                // キーが存在しない場合はデフォルト値
                return 30;
            }
        }

        /// <summary>
        /// キー（設定名）に該当する設定値をフラグとして返します
        /// </summary>
        /// <param name="key">フラグ値を持つキー</param>
        /// <returns>
        /// キーに該当する設定値が以下の文字列のいずれかに一致する場合は場合はfalse。それ以外はtrue。（大文字小文字は問わない）
        /// ・false
        /// ・off
        /// ・no
        /// ・0
        /// ・（空文字列）
        /// </returns>
        private static bool GetFlag(string key)
        {
            string isPrintLogoString = Get(key);
            bool isPrintLogo = true;
            if (isPrintLogoString == null)
            {
                return false;
            }
            switch (isPrintLogoString.ToLower().Trim())
            {
                case "false":
                case "off":
                case "no":
                case "0":
                case "":
                    isPrintLogo = false;
                    break;
            }

            return isPrintLogo;
        }

        /// <summary>
        /// キー（設定名）に該当する設定値をInt値として返します
        /// </summary>
        /// <param name="key">Int値を持つキー</param>
        /// <returns>キーに該当する設定値（Int型）</returns>
        private static int GetInt(string key)
        {
            string value = Get(key);
            int i;
            if (int.TryParse(value, out i) == false)
            {
                i = 0;
            }
            return i;
        }

        /// <summary>
        /// キー（設定名）に該当する設定値をFloat値として返します
        /// </summary>
        /// <param name="key">Float値を持つキー</param>
        /// <returns>キーに該当する設定値（Float型）</returns>
        private static float GetFloat(string key)
        {
            string value = Get(key);
            float f;
            if (float.TryParse(value, out f) == false)
            {
                f = 0f;
            }
            return f;
        }

        /// <summary>
        /// キー（設定名）に該当する設定値(CSV)をIntのリストとして返します
        /// </summary>
        /// <param name="key">CSVを持つキー</param>
        /// <returns>キーに該当する設定値（Intのリスト）</returns>
        private static List<int> GetCSVList(string key)
        {
            List<int> csvList = new List<int>();
            string ret = Get(key);
            if (string.IsNullOrEmpty(ret) == false)
            {
                // カンマ区切り文字列を分割
                string[] tokens = ret.Split(',');
                foreach (string s in tokens)
                {
                    // 数値だけを取り出してリストに追加
                    int i;
                    if (int.TryParse(s, out i))
                    {
                        csvList.Add(i);
                    }
                }
            }
            return csvList;
        }
    }
}
