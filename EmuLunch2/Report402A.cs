using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento
{
    /// <summary>
    /// 売上一覧
    /// </summary>
    public partial class Report402A : BaseReport
    {
        private const string TITLE = "売上一覧";

        public override string Title { get{ return TITLE; } }
        public override string TitleWithSize { get{ return TITLE + "(A4横)"; } }

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Report402A()
        {
            //
            // デザイナー サポートに必要なメソッドです。
            //
            InitializeComponent();
        }
        #endregion

    }
}
