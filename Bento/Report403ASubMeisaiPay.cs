using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento
{
    /// <summary>
    /// 売上明細表サブレポート(電子決済)
    /// </summary>
    public partial class Report403ASubMeisaiPay : GrapeCity.ActiveReports.SectionReport
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 一度だけ出力するためのフラグ
        /// </summary>
        private bool _oneTimeFlg;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Report403ASubMeisaiPay()
        {
            //
            // デザイナー サポートに必要なメソッドです。
            //
            InitializeComponent();

            _oneTimeFlg = false;
        }
        #endregion

        //**************************************
        //* イベント
        //**************************************
        #region イベント
        /// <summary>
        /// 詳細フォーマット
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void detail_Format(object sender, EventArgs e)
        {
            // 商品名
            if (txtSyohinName.Value != null && string.IsNullOrEmpty(txtSyohinName.Value.ToString()) == false)
            {
                txtSyohinName.Text = txtSyohinName.Value.ToString() + " ×";
            }
            else
            {
                txtSyohinName.Text = "";
            }

            // 数量
            if (txtSuryo.Value != null && string.IsNullOrEmpty(txtSuryo.Value.ToString()) == false)
            {
                txtSuryo.Text = txtSuryo.Value.ToString() + " ＝";
            }
            else
            {
                txtSuryo.Text = "0";
            }

            // 売上金額
            if (txtKingaku.Value != null && string.IsNullOrEmpty(txtKingaku.Value.ToString()) == false)
            {
                txtKingaku.Text = string.Format("{0:#,##0}", txtKingaku.Value);
            }
            else
            {
                txtKingaku.Text = "0";
            }

            // 値引額
            if (txtNebiki.Value != null && string.IsNullOrEmpty(txtNebiki.Value.ToString()) == false)
            {
                txtNebiki.Text = string.Format("{0:#,##0}", txtNebiki.Value);
            }
            else
            {
                txtNebiki.Text = "0";
            }

            // 回収電子決済
            if (_oneTimeFlg == false)
            {
                txtUrikake.Text = string.Format("{0}件分", txtUrikake.Value);
            }
            else
            {
                txtUrikake.Text = "";
            }

            // 本日電子決済
            if (_oneTimeFlg == false)
            {
                txtTotal.Text = "⑧-⑨+⑩+⑪=";
            }
            else
            {
                txtTotal.Text = "";
            }

            _oneTimeFlg = true;     // 出力済み
        }
        #endregion
    }
}
