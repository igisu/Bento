using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento
{
    /// <summary>
    /// 配送集計表
    /// </summary>
    public partial class Report500A : BaseReport
    {

        private const string TITLE = "配送集計表";

        public override string Title { get{ return TITLE; } }
        public override string TitleWithSize { get{ return TITLE + "(A4横)"; } }

        public Report500A()
        {
            //
            // デザイナー サポートに必要なメソッドです。
            //
            InitializeComponent();
        }
    }
}
