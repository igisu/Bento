using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento
{
    /// <summary>
    /// 請求書マージ用ダミー帳票
    /// </summary>
    public partial class Report000A : BaseReport
    {

        private const string TITLE = "請求書";

        public override string Title { get{ return TITLE; } }
        public override string TitleWithSize { get{ return TITLE + "(A4)"; } }

        public Report000A()
        {
            //
            // デザイナー サポートに必要なメソッドです。
            //
            InitializeComponent();
        }
    }
}
