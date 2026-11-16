using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento
{
    /// <summary>
    /// 請求一覧
    /// </summary>
    public partial class Report501A : GrapeCity.ActiveReports.SectionReport
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private DateTime? _修正日;
        private DateTime? _売上編集日時;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Report501A()
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
        private void Report501A_FetchData(object sender, FetchEventArgs eArgs)
        {
            _修正日 = Fields["修正日"].Value as DateTime?;
            _売上編集日時 = Fields["売上編集日時"].Value as DateTime?;
        }

        /// <summary>
        /// 詳細セクションフォーマット
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void detail_Format(object sender, EventArgs e)
        {
            Color foreColor = Color.Black;

            // 修正日≦売上編集日時 の場合は行の文字色を変える
            if (_修正日 != null && _売上編集日時 != null && _修正日 <= _売上編集日時)
            {
                foreColor = Color.Red;
            }

            txtSeikyuNo.ForeColor = foreColor;
            txtSeikyubi.ForeColor = foreColor;
            txtKaisyubi.ForeColor = foreColor;
            txtCourseCD.ForeColor = foreColor;
            txtCourseName.ForeColor = foreColor;
            txtTokuisakiCD.ForeColor = foreColor;
            txtTokuisakiName.ForeColor = foreColor;
            txtZenkaiSeikyu.ForeColor = foreColor;
            txtNyukin.ForeColor = foreColor;
            txtKurikoshi.ForeColor = foreColor;
            txtKaiage.ForeColor = foreColor;
            txtZei.ForeColor = foreColor;
            txtKonkaiSeikyu.ForeColor = foreColor;
            txtShimekubun.ForeColor = foreColor;
            txtShiharaiHoho.ForeColor = foreColor;
        }
        #endregion
    }
}
