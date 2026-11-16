namespace Emu.Bento.Common.Control
{
    partial class DateTimePickerA
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.textBoxA1 = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.nullableDateTimePicker1 = new Emu.Bento.Common.Control.NullableDateTimePicker(this.components);
            this.SuspendLayout();
            // 
            // textBoxA1
            // 
            this.textBoxA1.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.textBoxA1.Location = new System.Drawing.Point(0, 0);
            this.textBoxA1.MaxLength = 10;
            this.textBoxA1.Name = "textBoxA1";
            this.textBoxA1.Size = new System.Drawing.Size(121, 22);
            this.textBoxA1.TabIndex = 0;
            this.textBoxA1.TextChanged += new System.EventHandler(this.textBoxA1_TextChanged);
            this.textBoxA1.Enter += new System.EventHandler(this.textBoxA1_Enter);
            this.textBoxA1.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxA1_Validating);
            this.textBoxA1.Validated += new System.EventHandler(this.textBoxA1_Validated);
            // 
            // nullableDateTimePicker1
            // 
            this.nullableDateTimePicker1.CustomFormat = "yyyy/MM/dd(ddd)";
            this.nullableDateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.nullableDateTimePicker1.Location = new System.Drawing.Point(0, 0);
            this.nullableDateTimePicker1.Margin = new System.Windows.Forms.Padding(4);
            this.nullableDateTimePicker1.Name = "nullableDateTimePicker1";
            this.nullableDateTimePicker1.NullValue = null;
            this.nullableDateTimePicker1.Size = new System.Drawing.Size(152, 22);
            this.nullableDateTimePicker1.TabIndex = 1;
            this.nullableDateTimePicker1.TabStop = false;
            this.nullableDateTimePicker1.Value = null;
            this.nullableDateTimePicker1.CloseUp += new System.EventHandler(this.nullableDateTimePicker1_CloseUp);
            // 
            // DateTimePickerA
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBoxA1);
            this.Controls.Add(this.nullableDateTimePicker1);
            this.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "DateTimePickerA";
            this.Size = new System.Drawing.Size(152, 22);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private NullableDateTimePicker nullableDateTimePicker1;
        private TextBoxA textBoxA1;
    }
}
