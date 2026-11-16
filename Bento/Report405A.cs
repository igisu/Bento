using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento
{
    /// <summary>
    /// 売上集計表
    /// </summary>
    public partial class Report405A : GrapeCity.ActiveReports.SectionReport
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private int? _合計行フラグ;
        private int? _総売上見出しフラグ;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Report405A()
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
        private void Report405A_FetchData(object sender, FetchEventArgs eArgs)
        {
            _合計行フラグ = Fields["合計行フラグ"].Value as int?;
            _総売上見出しフラグ = Fields["総売上見出しフラグ"].Value as int?;
        }

        /// <summary>
        /// ページヘッダーセクションフォーマット
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pageHeader_Format(object sender, EventArgs e)
        {
            if (_総売上見出しフラグ == 0)
            {
                lblGrossUriNote1.Text = "①+③+⑤+⑥";      // チケット束代を売上とする場合は、現金売上＋チケット代＋掛売合計
            }
            else
            {
                lblGrossUriNote1.Text = "①+②+⑤+⑥";      // チケット回収時に売上とする場合は、現金売上＋チケット＋掛売合計
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
