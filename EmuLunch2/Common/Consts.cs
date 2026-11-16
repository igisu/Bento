using System;

namespace Emu.Bento.Common
{
    /// <summary>
    /// 定数クラス
    /// </summary>
    public static class Consts
    {
        /// <summary>
        /// アプリケーションタイトル
        /// </summary>
        public const string APP_TITLE = "エムランチ配送管理システム";

        /// <summary>
        /// update.zipのファイル名
        /// </summary>
        public const string UPDATE_ZIP = "EmuLunch2.zip";

        /// <summary>
        /// ゆうちょの銀行CD
        /// </summary>
        public const string YUCHO = "9900";


        /// <summary>
        /// 受注方法区分
        /// </summary>
        public enum JuchuKubun
        {
            Kotei = 0,          // 固定
            Call = 1,           // 電話要
            Wait = 2,           // 電話待
            FAX = 3,            // FAX
            Haisoin = 4,        // 配送員
            Yoyakuhyo = 5,      // 予約表
            Mail = 6            // メール
        }

        /// <summary>
        /// 弁当区分
        /// </summary>
        public enum BentoKubun
        {
            Bento = 0,          // 弁当
            SubDish = 1,        // 副食
            MainDish = 2,       // 主食
            Ticket = 8,         // チケット
            Other = 9           // その他
        }

        /// <summary>
        /// 状態区分
        /// </summary>
        public enum JotaiKubun
        {
            Syonin = 10,        // 承認
            KariSyonin = 20,    // 仮承認
            MiSyonin = 30,      // 未承認
            Shikkyaku = 40,     // 失客
            Dekin = 50,         // 出入禁止
            Black = 60          // ブラック
        }

        /// <summary>
        /// 支払方法区分
        /// </summary>
        public enum ShiharaiHohoKubun
        {
            Genkin = 1,         // 現金
            Kogitte = 2,        // 小切手
            Furikomi = 3,       // 振込
            Furikae = 4,        // 振替
            Ticket = 5,         // チケット
            Kinken = 7,         // 金券
            CreditCard = 10     // クレジットカード
        }

        /// <summary>
        /// 注文区分
        /// </summary>
        public enum ChumonKubun
        {
            Chuumon = 0,        // 注文
            Yosoku = 1          // 予測
        }

        /// <summary>
        /// 締め区分
        /// </summary>
        public enum ShimeKubun
        {
            Daily = 0,          // 日締め
            Weekly = 1,         // 週締め
            Monthly = 2,        // 月締め
            Tsuke = 3           // 未集金（現金の得意先に対してその日に集金できずに後日回収する場合(その日は掛売の売上が発生する)）（請求一覧以外のリストの選択肢には現れない）
        }

        /// <summary>
        /// コース区分
        /// </summary>
        public enum CourseKubun
        {
            Weekday = 1,        // 平日
            Saturday = 2,       // 土曜
            Sunday = 3,         // 日曜
            Holiday = 4         // 祝日
        }

        /// <summary>
        /// 売掛現金区分
        /// </summary>
        public enum UriGenKubun
        {
            /// <summary>
            /// 現金
            /// </summary>
            Genkin = 0,

            /// <summary>
            /// 売掛
            /// </summary>
            Urikake = 1,

            /// <summary>
            /// LINE決済（V_売掛現金区分には無い）
            /// </summary>
            Line = 5
        }

        /// <summary>
        /// 売上区分
        /// </summary>
        public enum UriageKubun
        {
            Genkin = 0,         // 現金
            Urikake = 1,        // 売掛
            Ticket = 2,         // チケット
            Barclay = 3,        // バークレー
            Bonus = 4,          // ボーナス
            Line = 5            // Tapion（旧：LINE決済）
        }

        /// <summary>
        /// 入金入力区分
        /// </summary>
        public enum NyukinNyuryokuKubun
        {
            PC = 0,         // PC
            Mobile = 1,     // Mobile
            EB = 2          // EB
        }

        /// <summary>
        /// 入金方法
        /// </summary>
        public enum NyukinHoho
        {
            Genkin = 1,         // 現金
            Kogitte = 2,        // 小切手
            Furikomi = 3,       // 振込
            Furikae = 4,        // 振替
            Ticket = 5,         // チケット
            Kinken = 7,         // 金券
            Tesuryo = 8,        // 手数料
            Tapion = 9,         // Tapion
            CreditCard = 10     // クレジットカード
        }

        /// <summary>
        /// 帳票CD
        /// </summary>
        public enum Reports
        {
            Nouhinsho = 100,            // 納品書
            Seikyusho = 200,            // 請求書
            Ryoshusho = 300,            // 領収書
            YokiKaisyuhyo = 303,        // 容器回収表
            Tanzaku = 304,              // 短冊
            BigBag = 307,               // 大袋ラベル
            UriageMeisaihyo = 403,      // 売上明細表
            UriageNikkeihyo = 405,      // 売上集計表
            HanbaiJissekiList = 408,    // 販売実績一覧表
            HaisoSyukeihyo = 500,       // 配送集計表
            UriageBunsekiHyo = 817,     // 集計タイプ別売上分析表
            UriageList = 402,           // 売上一覧
            SeikyuList = 501,           // 請求一覧
            TPList = 512,               // TP決済一覧
            NyukinList = 600            // 入金一覧
        }

        /// <summary>
        /// 請求書タイプ
        /// </summary>
        public enum SeikyusyoType
        {
            Detail = 1,         // 明細
            CalendarYoko = 2,   // カレンダー横
            CalendarTate = 3,   // カレンダー縦
        }

        /// <summary>
        /// 曜日と祝日
        /// </summary>
        public enum EmuDayOfWeek
        {
            Sunday = DayOfWeek.Sunday,
            Monday = DayOfWeek.Monday,
            Tuesday = DayOfWeek.Tuesday,
            Wednesday = DayOfWeek.Wednesday,
            Thursday = DayOfWeek.Thursday,
            Friday = DayOfWeek.Friday,
            Saturday = DayOfWeek.Saturday,
            Holiday = DayOfWeek.Saturday + 1
        }

        /// <summary>
        /// 各種コード
        /// </summary>
        public enum KakusyuCD
        {
            Cource = 19             // コース
        }

        /// <summary>
        /// 集計対象区分
        /// </summary>
        public enum SyukeiTaisyoKubun
        {
            Uriage = 0,         // 売上
            Nyukin = 1          // 入金
        }

        /// <summary>
        /// CSVテンプレートCD
        /// </summary>
        public enum CSVTemplates
        {
            KeikeiRenkei = 1    // 会計連携
        }

        /// <summary>
        /// 大袋ラベル印刷種別
        /// </summary>
        public enum BigBagPrintKind
        {
            Set = 1,            // セット
            Unit = 2,           // 単品
        }

        /// <summary>
        /// 配送集計表に出力する商品の取得元区分
        /// </summary>
        public enum HaisoSyukeihyoSyohin
        {
            SyuyoSyohin = 0,    // 主要商品リストマスタ
            Chumon = 1,         // その日の注文
        }

        /// <summary>
        /// FR_T_クレカ決済履歴 - 処理タイプ
        /// </summary>
        public enum FRProcessType
        {
            Settle = 1, // 決済   
            Cancel = 2  // 取消
        }

        /// <summary>
        /// FR_T_クレカ決済履歴 - 処理結果
        /// </summary>
        public enum FRProcessResult
        {
            Success = 1,    // 成功
            Fail = 2        // 失敗
        }

        /// <summary>
        /// 請求合計 - クレカ決済エラー区分
        /// </summary>
        public enum FRErrorType
        {
            None = 0,           // エラーなし
            MatchError = 1,     // 不整合エラー
            SystemError = 2,    // システムエラー
        }

        #region CTI発信用
        /// <summary>
        /// エフェメラルポートの最小値
        /// </summary>
        public const int EPHEMERAL_PORT_MIN = 49152;

        /// <summary>
        /// エフェメラルポートの最大値
        /// </summary>
        public const int EPHEMERAL_PORT_MAX = 65535;

        /// <summary>
        /// 次のエフェメラルポートを求める際の乱数の最大値
        /// </summary>
        public const int PORT_STEP = 256;
        #endregion


        /// <summary>
        /// 締め区分の名称を返します。
        /// </summary>
        /// <param name="shimeKubun">締め区分</param>
        /// <returns>締め区分の名称</returns>
        public static string DisplayName(this ShimeKubun shimeKubun)
        {
            string[] names = { "日締", "週締", "月締", "未収金" };
            return names[(int)shimeKubun];
        }

        /// <summary>
        /// 締め区分の選択肢文字列を返します。
        /// </summary>
        /// <param name="shimeKubun">締め区分の</param>
        /// <returns>締め区分の選択肢文字列</returns>
        public static string OptionDisplay(this ShimeKubun shimeKubun)
        {
            return ((int)shimeKubun).ToString() + ":" + shimeKubun.DisplayName();
        }

        /// <summary>
        /// コース区分の名称を返します。
        /// </summary>
        /// <param name="shimeKubun">コース区分</param>
        /// <returns>コース区分の名称</returns>
        public static string DisplayName(this CourseKubun courseKubun)
        {
            string[] names = { "", "平日", "土曜", "日曜", "祝日" };
            return names[(int)courseKubun];
        }

        /// <summary>
        /// 帳票CDの名称を返します。
        /// </summary>
        /// <param name="reports">帳票CD</param>
        /// <returns></returns>
        public static string DisplayName(this Reports reports)
        {
            string name = "";
            switch (reports)
            {
                case Reports.Nouhinsho:
                    name = "納品書";
                    break;

                case Reports.Seikyusho:
                    name = "請求書";
                    break;

                case Reports.Ryoshusho:
                    name = "領収書";
                    break;

                case Reports.YokiKaisyuhyo:
                    name = "容器回収表";
                    break;

                case Reports.Tanzaku:
                    name = "短冊";
                    break;

                case Reports.BigBag:
                    name = "大袋ラベル";
                    break;

                case Reports.UriageMeisaihyo:
                    name = "売上明細表";
                    break;

                case Reports.UriageNikkeihyo:
                    name = "売上集計表";
                    break;

                case Reports.HanbaiJissekiList:
                    name = "販売実績一覧表";
                    break;

                case Reports.HaisoSyukeihyo:
                    name = "配送集計表";
                    break;

                case Reports.UriageBunsekiHyo:
                    name = "集計タイプ別売上分析表";
                    break;

                case Reports.UriageList:
                    name = "売上一覧";
                    break;

                case Reports.SeikyuList:
                    name = "請求一覧";
                    break;

                case Reports.NyukinList:
                    name = "入金一覧";
                    break;
            }
            return name;
        }

        /// <summary>
        /// 売上分析表の集計タイプ
        /// </summary>
        public enum JissekiSyukeiPrintType
        {
            Busyo = 1,      // 部署
            Course = 2,     // コース
            Tantousya = 3   // 担当者
        }

        /// <summary>
        /// 集計タイプ別売上分析表の名称を返します。
        /// </summary>
        /// <param name="jissekiSyukeiPrintType">売上分析表の集計タイプ</param>
        /// <returns></returns>
        public static string DisplayName(this JissekiSyukeiPrintType jissekiSyukeiPrintType)
        {
            string name = "";
            switch (jissekiSyukeiPrintType)
            {
                case JissekiSyukeiPrintType.Busyo:
                    name = "部署別実績日計";
                    break;

                case JissekiSyukeiPrintType.Course:
                    name = "コース別実績日計";
                    break;

                case JissekiSyukeiPrintType.Tantousya:
                    name = "担当者別実績集計";
                    break;
            }
            return name;
        }

        /// <summary>
        /// 大袋ラベル印刷種別の名称を返します。
        /// </summary>
        /// <param name="bigBagPrintKind">大袋ラベル印刷種別</param>
        /// <returns></returns>
        public static string DisplayName(this BigBagPrintKind bigBagPrintKind)
        {
            string name = "";
            switch (bigBagPrintKind)
            {
                case BigBagPrintKind.Set:
                    name = "セット";
                    break;

                case BigBagPrintKind.Unit:
                    name = "単品";
                    break;
            }
            return name;
        }

    }
}
