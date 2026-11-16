using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento
{
    /// <summary>
    /// 容器回収表
    /// </summary>
    public partial class Report303A : GrapeCity.ActiveReports.SectionReport
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// ページ番号
        /// </summary>
        private int _page = 0;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Report303A()
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
        /// グループヘッダーフォーマット
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void groupHeader1_Format(object sender, EventArgs e)
        {
            //***********************************************************************************************************************
            // Ｑ．なぜReportInfoを使ってページ番号を出力しない？
            // Ａ．・ページの上部にページ番号を出力すべくpageHeaderにReportInfoを配置する。
            //     ・そのpageHeaderにレコードごとに値が変化するバウンドコントロール（データソースに連結したコントロール）を配置する。
            //     ・グループごとに改ページをすべくgroupHeader/groupFooterを配置する。
            //     以上の条件のときに、明細行がページ当たりの行数を超えて自動的に改ページしたときに、
            //     pageHeaderには改ページ直後の先頭レコードではなく、その次のレコードの内容が出力されてしまう。
            //     これはActiveReportの仕様。
            //     これを回避するには以下のようにする必要がある。
            //      ・pageHeaderに配置しているものをgroupHeaderに配置する。
            //      ・ReportInfoを使わずに、groupHeader_Formatにてインクリメントしたページ番号をテキストボックスに設定する。
            //      ・groupHeaderのRepeatStyleをOnPageにする
            //***********************************************************************************************************************
            _page++;
            txtPage.Text = _page.ToString();
        }
        #endregion
    }
}
