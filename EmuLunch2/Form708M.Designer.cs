namespace Emu.Bento
{
    partial class Form708M
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
            this.components = new System.ComponentModel.Container();
            this.courseTableLeftBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnQuit = new System.Windows.Forms.Button();
            this.btnLeftAll = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnRightAll = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblBushoRight = new System.Windows.Forms.Label();
            this.courseMoveBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.lblCourseKbnRight = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnBottomRight = new System.Windows.Forms.Button();
            this.btnTopRight = new System.Windows.Forms.Button();
            this.btnDownRight = new System.Windows.Forms.Button();
            this.btnUpRight = new System.Windows.Forms.Button();
            this.lstRight = new System.Windows.Forms.ListBox();
            this.courseTableRightBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnQuery = new System.Windows.Forms.Button();
            this.btnCourseCDRight = new System.Windows.Forms.Button();
            this.lblCourseNameRight = new System.Windows.Forms.Label();
            this.txtCourseCDRight = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblBushoLeft = new System.Windows.Forms.Label();
            this.courseBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label6 = new System.Windows.Forms.Label();
            this.lblCourseKbnLeft = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBottomLeft = new System.Windows.Forms.Button();
            this.btnTopLeft = new System.Windows.Forms.Button();
            this.lstLeft = new System.Windows.Forms.ListBox();
            this.btnDownLeft = new System.Windows.Forms.Button();
            this.btnUpLeft = new System.Windows.Forms.Button();
            this.lblCourseNameLeft = new System.Windows.Forms.Label();
            this.lblCourseCDLeft = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.btnReset = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.courseTableLeftBindingSource)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.courseMoveBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.courseTableRightBindingSource)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.courseBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.AutoSize = true;
            this.btnQuit.CausesValidation = false;
            this.btnQuit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnQuit.Location = new System.Drawing.Point(675, 429);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(103, 25);
            this.btnQuit.TabIndex = 8;
            this.btnQuit.Text = "終了(Esc)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // btnLeftAll
            // 
            this.btnLeftAll.Location = new System.Drawing.Point(349, 259);
            this.btnLeftAll.Name = "btnLeftAll";
            this.btnLeftAll.Size = new System.Drawing.Size(75, 23);
            this.btnLeftAll.TabIndex = 4;
            this.btnLeftAll.Text = "<<";
            this.btnLeftAll.UseVisualStyleBackColor = true;
            this.btnLeftAll.Click += new System.EventHandler(this.btnLeftAll_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(349, 230);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(75, 23);
            this.btnLeft.TabIndex = 3;
            this.btnLeft.Text = "<";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnRightAll
            // 
            this.btnRightAll.Location = new System.Drawing.Point(349, 201);
            this.btnRightAll.Name = "btnRightAll";
            this.btnRightAll.Size = new System.Drawing.Size(75, 23);
            this.btnRightAll.TabIndex = 2;
            this.btnRightAll.Text = ">>";
            this.btnRightAll.UseVisualStyleBackColor = true;
            this.btnRightAll.Click += new System.EventHandler(this.btnRightAll_Click);
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(349, 172);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(75, 23);
            this.btnRight.TabIndex = 1;
            this.btnRight.Text = ">";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.lblBushoRight);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.lblCourseKbnRight);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.btnBottomRight);
            this.groupBox2.Controls.Add(this.btnTopRight);
            this.groupBox2.Controls.Add(this.btnDownRight);
            this.groupBox2.Controls.Add(this.btnUpRight);
            this.groupBox2.Controls.Add(this.lstRight);
            this.groupBox2.Controls.Add(this.btnQuery);
            this.groupBox2.Controls.Add(this.btnCourseCDRight);
            this.groupBox2.Controls.Add(this.lblCourseNameRight);
            this.groupBox2.Controls.Add(this.txtCourseCDRight);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(430, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(331, 411);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "移動先";
            // 
            // lblBushoRight
            // 
            this.lblBushoRight.AutoEllipsis = true;
            this.lblBushoRight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblBushoRight.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.courseMoveBindingSource, "部署名", true));
            this.lblBushoRight.Location = new System.Drawing.Point(184, 48);
            this.lblBushoRight.Name = "lblBushoRight";
            this.lblBushoRight.Size = new System.Drawing.Size(141, 22);
            this.lblBushoRight.TabIndex = 8;
            this.lblBushoRight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // courseMoveBindingSource
            // 
            this.courseMoveBindingSource.DataSource = typeof(Emu.Bento.Data.CourseMoveCriteria);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(141, 52);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 15);
            this.label7.TabIndex = 7;
            this.label7.Text = "部署";
            // 
            // lblCourseKbnRight
            // 
            this.lblCourseKbnRight.AutoEllipsis = true;
            this.lblCourseKbnRight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCourseKbnRight.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.courseMoveBindingSource, "コース区分名", true));
            this.lblCourseKbnRight.Location = new System.Drawing.Point(82, 48);
            this.lblCourseKbnRight.Name = "lblCourseKbnRight";
            this.lblCourseKbnRight.Size = new System.Drawing.Size(53, 22);
            this.lblCourseKbnRight.TabIndex = 6;
            this.lblCourseKbnRight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 15);
            this.label4.TabIndex = 5;
            this.label4.Text = "コース区分";
            // 
            // btnBottomRight
            // 
            this.btnBottomRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBottomRight.Location = new System.Drawing.Point(302, 336);
            this.btnBottomRight.Name = "btnBottomRight";
            this.btnBottomRight.Size = new System.Drawing.Size(23, 60);
            this.btnBottomRight.TabIndex = 0;
            this.btnBottomRight.Text = "↓￣";
            this.btnBottomRight.UseVisualStyleBackColor = true;
            this.btnBottomRight.Click += new System.EventHandler(this.btnBottomRight_Click);
            // 
            // btnTopRight
            // 
            this.btnTopRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTopRight.Location = new System.Drawing.Point(302, 108);
            this.btnTopRight.Name = "btnTopRight";
            this.btnTopRight.Size = new System.Drawing.Size(23, 60);
            this.btnTopRight.TabIndex = 11;
            this.btnTopRight.Text = "＿↑";
            this.btnTopRight.UseVisualStyleBackColor = true;
            this.btnTopRight.Click += new System.EventHandler(this.btnTopRight_Click);
            // 
            // btnDownRight
            // 
            this.btnDownRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDownRight.Location = new System.Drawing.Point(302, 255);
            this.btnDownRight.Name = "btnDownRight";
            this.btnDownRight.Size = new System.Drawing.Size(23, 75);
            this.btnDownRight.TabIndex = 13;
            this.btnDownRight.Text = "下へ↓";
            this.btnDownRight.UseVisualStyleBackColor = true;
            this.btnDownRight.Click += new System.EventHandler(this.btnDownRight_Click);
            // 
            // btnUpRight
            // 
            this.btnUpRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUpRight.Location = new System.Drawing.Point(302, 174);
            this.btnUpRight.Name = "btnUpRight";
            this.btnUpRight.Size = new System.Drawing.Size(23, 75);
            this.btnUpRight.TabIndex = 12;
            this.btnUpRight.Text = "↑上へ";
            this.btnUpRight.UseVisualStyleBackColor = true;
            this.btnUpRight.Click += new System.EventHandler(this.btnUpRight_Click);
            // 
            // lstRight
            // 
            this.lstRight.AllowDrop = true;
            this.lstRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lstRight.DataSource = this.courseTableRightBindingSource;
            this.lstRight.FormattingEnabled = true;
            this.lstRight.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.lstRight.ItemHeight = 15;
            this.lstRight.Location = new System.Drawing.Point(9, 108);
            this.lstRight.Name = "lstRight";
            this.lstRight.Size = new System.Drawing.Size(287, 289);
            this.lstRight.TabIndex = 10;
            this.lstRight.DragDrop += new System.Windows.Forms.DragEventHandler(this.ListBox_DragDrop);
            this.lstRight.DragOver += new System.Windows.Forms.DragEventHandler(this.ListBox_DragOver);
            this.lstRight.QueryContinueDrag += new System.Windows.Forms.QueryContinueDragEventHandler(this.ListBox_QueryContinueDrag);
            this.lstRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ListBox_MouseDown);
            this.lstRight.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ListBox_MouseMove);
            this.lstRight.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ListBox_MouseUp);
            // 
            // btnQuery
            // 
            this.btnQuery.AutoSize = true;
            this.btnQuery.Location = new System.Drawing.Point(129, 77);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(73, 25);
            this.btnQuery.TabIndex = 9;
            this.btnQuery.Text = "表示";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // btnCourseCDRight
            // 
            this.btnCourseCDRight.AutoSize = true;
            this.btnCourseCDRight.Location = new System.Drawing.Point(278, 16);
            this.btnCourseCDRight.Name = "btnCourseCDRight";
            this.btnCourseCDRight.Size = new System.Drawing.Size(47, 25);
            this.btnCourseCDRight.TabIndex = 3;
            this.btnCourseCDRight.TabStop = false;
            this.btnCourseCDRight.Text = "参照";
            this.btnCourseCDRight.UseVisualStyleBackColor = true;
            this.btnCourseCDRight.Click += new System.EventHandler(this.btnCourseCDRight_Click);
            // 
            // lblCourseNameRight
            // 
            this.lblCourseNameRight.AutoEllipsis = true;
            this.lblCourseNameRight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCourseNameRight.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.courseMoveBindingSource, "コース名", true));
            this.lblCourseNameRight.Location = new System.Drawing.Point(141, 18);
            this.lblCourseNameRight.Name = "lblCourseNameRight";
            this.lblCourseNameRight.Size = new System.Drawing.Size(131, 22);
            this.lblCourseNameRight.TabIndex = 2;
            this.lblCourseNameRight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtCourseCDRight
            // 
            this.txtCourseCDRight.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.courseMoveBindingSource, "コースＣＤ", true));
            this.txtCourseCDRight.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtCourseCDRight.Location = new System.Drawing.Point(52, 18);
            this.txtCourseCDRight.MaxLength = 9;
            this.txtCourseCDRight.Name = "txtCourseCDRight";
            this.txtCourseCDRight.Size = new System.Drawing.Size(83, 22);
            this.txtCourseCDRight.TabIndex = 1;
            this.txtCourseCDRight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCourseCDRight.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCourseCDRight_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "コース";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.lblBushoLeft);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.lblCourseKbnLeft);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnBottomLeft);
            this.groupBox1.Controls.Add(this.btnTopLeft);
            this.groupBox1.Controls.Add(this.lstLeft);
            this.groupBox1.Controls.Add(this.btnDownLeft);
            this.groupBox1.Controls.Add(this.btnUpLeft);
            this.groupBox1.Controls.Add(this.lblCourseNameLeft);
            this.groupBox1.Controls.Add(this.lblCourseCDLeft);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(331, 411);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "移動元";
            // 
            // lblBushoLeft
            // 
            this.lblBushoLeft.AutoEllipsis = true;
            this.lblBushoLeft.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblBushoLeft.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.courseBindingSource, "部署名", true));
            this.lblBushoLeft.Location = new System.Drawing.Point(184, 48);
            this.lblBushoLeft.Name = "lblBushoLeft";
            this.lblBushoLeft.Size = new System.Drawing.Size(141, 22);
            this.lblBushoLeft.TabIndex = 6;
            this.lblBushoLeft.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // courseBindingSource
            // 
            this.courseBindingSource.DataSource = typeof(Emu.Bento.Data.Course);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(141, 52);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 15);
            this.label6.TabIndex = 5;
            this.label6.Text = "部署";
            // 
            // lblCourseKbnLeft
            // 
            this.lblCourseKbnLeft.AutoEllipsis = true;
            this.lblCourseKbnLeft.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCourseKbnLeft.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.courseBindingSource, "コース区分名", true));
            this.lblCourseKbnLeft.Location = new System.Drawing.Point(82, 48);
            this.lblCourseKbnLeft.Name = "lblCourseKbnLeft";
            this.lblCourseKbnLeft.Size = new System.Drawing.Size(53, 22);
            this.lblCourseKbnLeft.TabIndex = 4;
            this.lblCourseKbnLeft.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "コース区分";
            // 
            // btnBottomLeft
            // 
            this.btnBottomLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBottomLeft.Location = new System.Drawing.Point(302, 336);
            this.btnBottomLeft.Name = "btnBottomLeft";
            this.btnBottomLeft.Size = new System.Drawing.Size(23, 60);
            this.btnBottomLeft.TabIndex = 11;
            this.btnBottomLeft.Text = "↓￣";
            this.btnBottomLeft.UseVisualStyleBackColor = true;
            this.btnBottomLeft.Click += new System.EventHandler(this.btnBottomLeft_Click);
            // 
            // btnTopLeft
            // 
            this.btnTopLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTopLeft.Location = new System.Drawing.Point(302, 108);
            this.btnTopLeft.Name = "btnTopLeft";
            this.btnTopLeft.Size = new System.Drawing.Size(23, 60);
            this.btnTopLeft.TabIndex = 8;
            this.btnTopLeft.Text = "＿↑";
            this.btnTopLeft.UseVisualStyleBackColor = true;
            this.btnTopLeft.Click += new System.EventHandler(this.btnTopLeft_Click);
            // 
            // lstLeft
            // 
            this.lstLeft.AllowDrop = true;
            this.lstLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lstLeft.DataSource = this.courseTableLeftBindingSource;
            this.lstLeft.FormattingEnabled = true;
            this.lstLeft.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.lstLeft.ItemHeight = 15;
            this.lstLeft.Location = new System.Drawing.Point(9, 108);
            this.lstLeft.Name = "lstLeft";
            this.lstLeft.Size = new System.Drawing.Size(287, 289);
            this.lstLeft.TabIndex = 7;
            this.lstLeft.DragDrop += new System.Windows.Forms.DragEventHandler(this.ListBox_DragDrop);
            this.lstLeft.DragOver += new System.Windows.Forms.DragEventHandler(this.ListBox_DragOver);
            this.lstLeft.QueryContinueDrag += new System.Windows.Forms.QueryContinueDragEventHandler(this.ListBox_QueryContinueDrag);
            this.lstLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ListBox_MouseDown);
            this.lstLeft.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ListBox_MouseMove);
            this.lstLeft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ListBox_MouseUp);
            // 
            // btnDownLeft
            // 
            this.btnDownLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDownLeft.Location = new System.Drawing.Point(302, 255);
            this.btnDownLeft.Name = "btnDownLeft";
            this.btnDownLeft.Size = new System.Drawing.Size(23, 75);
            this.btnDownLeft.TabIndex = 10;
            this.btnDownLeft.Text = "下へ↓";
            this.btnDownLeft.UseVisualStyleBackColor = true;
            this.btnDownLeft.Click += new System.EventHandler(this.btnDownLeft_Click);
            // 
            // btnUpLeft
            // 
            this.btnUpLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUpLeft.Location = new System.Drawing.Point(302, 174);
            this.btnUpLeft.Name = "btnUpLeft";
            this.btnUpLeft.Size = new System.Drawing.Size(23, 75);
            this.btnUpLeft.TabIndex = 9;
            this.btnUpLeft.Text = "↑上へ";
            this.btnUpLeft.UseVisualStyleBackColor = true;
            this.btnUpLeft.Click += new System.EventHandler(this.btnUpLeft_Click);
            // 
            // lblCourseNameLeft
            // 
            this.lblCourseNameLeft.AutoEllipsis = true;
            this.lblCourseNameLeft.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCourseNameLeft.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.courseBindingSource, "コース名", true));
            this.lblCourseNameLeft.Location = new System.Drawing.Point(141, 18);
            this.lblCourseNameLeft.Name = "lblCourseNameLeft";
            this.lblCourseNameLeft.Size = new System.Drawing.Size(131, 22);
            this.lblCourseNameLeft.TabIndex = 2;
            this.lblCourseNameLeft.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCourseCDLeft
            // 
            this.lblCourseCDLeft.AutoEllipsis = true;
            this.lblCourseCDLeft.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCourseCDLeft.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.courseBindingSource, "コースＣＤ", true));
            this.lblCourseCDLeft.Location = new System.Drawing.Point(52, 18);
            this.lblCourseCDLeft.Name = "lblCourseCDLeft";
            this.lblCourseCDLeft.Size = new System.Drawing.Size(83, 22);
            this.lblCourseCDLeft.TabIndex = 1;
            this.lblCourseCDLeft.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 15);
            this.label5.TabIndex = 0;
            this.label5.Text = "コース";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.AutoSize = true;
            this.btnSave.Location = new System.Drawing.Point(566, 429);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(103, 25);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "更新(F9)";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.courseMoveBindingSource;
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReset.AutoSize = true;
            this.btnReset.Location = new System.Drawing.Point(12, 429);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(103, 25);
            this.btnReset.TabIndex = 6;
            this.btnReset.Text = "リセット";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // Form708M
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 466);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnLeftAll);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.btnRightAll);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnQuit);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form708M";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "コース移動";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form708M_FormClosing);
            this.Load += new System.EventHandler(this.Form708M_Load);
            this.Shown += new System.EventHandler(this.Form708M_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form708M_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.courseTableLeftBindingSource)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.courseMoveBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.courseTableRightBindingSource)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.courseBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource courseTableLeftBindingSource;
        private System.Windows.Forms.BindingSource courseMoveBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblCourseCDLeft;
        private System.Windows.Forms.Label lblCourseNameLeft;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnCourseCDRight;
        private System.Windows.Forms.Label lblCourseNameRight;
        private Common.Control.TextBoxAN txtCourseCDRight;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDownLeft;
        private System.Windows.Forms.Button btnUpLeft;
        private System.Windows.Forms.ListBox lstRight;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.ListBox lstLeft;
        private System.Windows.Forms.Button btnDownRight;
        private System.Windows.Forms.Button btnUpRight;
        private System.Windows.Forms.Button btnLeftAll;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnRightAll;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.BindingSource courseTableRightBindingSource;
        private System.Windows.Forms.BindingSource courseBindingSource;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnTopLeft;
        private System.Windows.Forms.Button btnTopRight;
        private System.Windows.Forms.Button btnBottomLeft;
        private System.Windows.Forms.Button btnBottomRight;
        private System.Windows.Forms.Label lblCourseKbnLeft;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblCourseKbnRight;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblBushoLeft;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblBushoRight;
        private System.Windows.Forms.Label label7;
    }
}
