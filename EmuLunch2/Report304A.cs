using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento
{
    /// <summary>
    /// 短冊
    /// </summary>
    public partial class Report304A : BaseReport
    {

        private const string TITLE = "短冊";

        public override string Title { get{ return TITLE; } }
        public override string TitleWithSize { get{ return TITLE; } }

        public Report304A()
        {
            //
            // デザイナー サポートに必要なメソッドです。
            //
            InitializeComponent();
        }
    }
}
