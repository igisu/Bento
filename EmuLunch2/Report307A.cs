using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento
{
    /// <summary>
    /// 大袋ラベル
    /// </summary>
    public partial class Report307A : BaseReport
    {

        private const string TITLE = "大袋ラベル";

        public override string Title { get{ return TITLE; } }
        public override string TitleWithSize { get{ return TITLE; } }

        public Report307A()
        {
            //
            // デザイナー サポートに必要なメソッドです。
            //
            InitializeComponent();
        }
    }
}
