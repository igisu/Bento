namespace Emu.Bento
{
    partial class FormPreview
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ActiveReportViewer = new GrapeCity.ActiveReports.Viewer.Win.Viewer();
            this.SuspendLayout();
            // 
            // ActiveReportViewer
            // 
            this.ActiveReportViewer.CurrentPage = 0;
            this.ActiveReportViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ActiveReportViewer.Location = new System.Drawing.Point(0, 0);
            this.ActiveReportViewer.Name = "ActiveReportViewer";
            this.ActiveReportViewer.PreviewPages = 0;
            // 
            // 
            // 
            // 
            // 
            // 
            this.ActiveReportViewer.Sidebar.ParametersPanel.ContextMenu = null;
            this.ActiveReportViewer.Sidebar.ParametersPanel.Text = "パラメータ";
            this.ActiveReportViewer.Sidebar.ParametersPanel.Width = 200;
            // 
            // 
            // 
            this.ActiveReportViewer.Sidebar.SearchPanel.ContextMenu = null;
            this.ActiveReportViewer.Sidebar.SearchPanel.Text = "検索";
            this.ActiveReportViewer.Sidebar.SearchPanel.Width = 200;
            // 
            // 
            // 
            this.ActiveReportViewer.Sidebar.ThumbnailsPanel.ContextMenu = null;
            this.ActiveReportViewer.Sidebar.ThumbnailsPanel.Text = "サムネイル";
            this.ActiveReportViewer.Sidebar.ThumbnailsPanel.Width = 200;
            this.ActiveReportViewer.Sidebar.ThumbnailsPanel.Zoom = 0.1D;
            // 
            // 
            // 
            this.ActiveReportViewer.Sidebar.TocPanel.ContextMenu = null;
            this.ActiveReportViewer.Sidebar.TocPanel.Expanded = true;
            this.ActiveReportViewer.Sidebar.TocPanel.Text = "見出しマップラベル";
            this.ActiveReportViewer.Sidebar.TocPanel.Width = 200;
            this.ActiveReportViewer.Sidebar.Width = 200;
            this.ActiveReportViewer.Size = new System.Drawing.Size(884, 661);
            this.ActiveReportViewer.TabIndex = 0;
            this.ActiveReportViewer.ViewType = GrapeCity.Viewer.Common.Model.ViewType.Continuous;
            // 
            // FormPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 661);
            this.Controls.Add(this.ActiveReportViewer);
            this.Name = "FormPreview";
            this.Text = "FormPreview";
            this.ResumeLayout(false);

        }

        #endregion

        public GrapeCity.ActiveReports.Viewer.Win.Viewer ActiveReportViewer;



    }
}