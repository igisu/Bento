using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento
{
    /// <summary>
    /// TP決済一覧
    /// </summary>
    public partial class Report512A : BaseReport
    {
        private const string TITLE = "TP決済一覧";

        public override string Title { get{ return TITLE; } }
        public override string TitleWithSize { get{ return TITLE + "(A4横)"; } }

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Report512A()
        {
            //
            // デザイナー サポートに必要なメソッドです。
            //
            InitializeComponent();
        }
        #endregion

    }
}
