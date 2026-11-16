using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento
{
    /// <summary>
    /// カードご利用状況表（兼 ご利用明細）
    /// </summary>
    public partial class Report819A : BaseReport
    {
        private const string TITLE = "カードご利用状況表（兼 ご利用明細）";

        public override string Title { get{ return TITLE; } }
        public override string TitleWithSize { get{ return TITLE + "(A4横)"; } }

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Report819A()
        {
            //
            // デザイナー サポートに必要なメソッドです。
            //
            InitializeComponent();
        }
        #endregion
    }
}
