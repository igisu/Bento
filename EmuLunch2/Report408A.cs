using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento
{
    /// <summary>
    /// 販売実績一覧表
    /// </summary>
    public partial class Report408A : BaseReport
    {
        private const string TITLE = "販売実績一覧表";

        public override string Title { get{ return TITLE; } }
        public override string TitleWithSize { get{ return TITLE + "(A4横)"; } }

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private int? _合計行フラグ;
        private int? _ソート用部署ＣＤ;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Report408A()
        {
            //
            // デザイナー サポートに必要なメソッドです。
            //
            InitializeComponent();
        }
        #endregion

        //**************************************
        //* イベント
        //**************************************
        #region イベント
        /// <summary>
        /// 新しいレコードが処理されるごとに発生します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eArgs"></param>
        private void Report408A_FetchData(object sender, FetchEventArgs eArgs)
        {
            _合計行フラグ = Fields["合計行フラグ"].Value as int?;
            _ソート用部署ＣＤ = Fields["ソート用部署ＣＤ"].Value as int?;

            // 総合計の場合は、グループフッター後の改ページをしない
            // ┌─────────────────────────────────────────────────────────
            // │Ｑ．グループごとに改ページをやるなら、ページヘッダーではなくグループヘッダーでよいのでは？
            // │Ａ．グループヘッダーにReportInfoを配置してページ番号を出力すると、同一グループは同一ページ番号が出力されてしまう。
            // │    これを回避するには、グループヘッダーではなくページヘッダーにReportInfoを配置するしかない。
            // │    したがって改ページはグループヘッダーではなくグループフッターの出力後になるので、これを制御することになる。
            // └─────────────────────────────────────────────────────────
            if (_ソート用部署ＣＤ == Form408.DELIMITER_BUSHO_CD)
            {
                groupFooter1.Visible = false;
                //groupFooter1.NewPage = GrapeCity.ActiveReports.SectionReportModel.NewPage.None;   // ←これは要らなかった
            }
        }

        /// <summary>
        /// 詳細セクションフォーマット
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void detail_Format(object sender, EventArgs e)
        {
            // 合計行の場合は配送者の左の罫線を描画しない
            if (_合計行フラグ == 1)
            {
                lineDetailHaisosya.Visible = false;
            }
            else
            {
                lineDetailHaisosya.Visible = true;
            }
        }
        #endregion
    }
}
