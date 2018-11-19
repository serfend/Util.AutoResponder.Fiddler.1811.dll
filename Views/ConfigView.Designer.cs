namespace Mondol.FiddlerExtension.AutoResponder.Views
{
    partial class ConfigView
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
			this.ctlEnable = new System.Windows.Forms.CheckBox();
			this.ctlPaths = new System.Windows.Forms.TextBox();
			this.ctlSave = new System.Windows.Forms.Button();
			this.ctlPowerBy = new System.Windows.Forms.LinkLabel();
			this.ctlLoadSetting = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// ctlEnable
			// 
			this.ctlEnable.AutoSize = true;
			this.ctlEnable.Location = new System.Drawing.Point(8, 13);
			this.ctlEnable.Name = "ctlEnable";
			this.ctlEnable.Size = new System.Drawing.Size(72, 16);
			this.ctlEnable.TabIndex = 0;
			this.ctlEnable.Text = "是否启用";
			this.ctlEnable.UseVisualStyleBackColor = true;
			this.ctlEnable.CheckedChanged += new System.EventHandler(this.ctlEnable_CheckedChanged);
			// 
			// ctlPaths
			// 
			this.ctlPaths.AcceptsReturn = true;
			this.ctlPaths.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ctlPaths.Location = new System.Drawing.Point(8, 35);
			this.ctlPaths.Multiline = true;
			this.ctlPaths.Name = "ctlPaths";
			this.ctlPaths.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.ctlPaths.Size = new System.Drawing.Size(560, 265);
			this.ctlPaths.TabIndex = 1;
			// 
			// ctlSave
			// 
			this.ctlSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ctlSave.Location = new System.Drawing.Point(493, 306);
			this.ctlSave.Name = "ctlSave";
			this.ctlSave.Size = new System.Drawing.Size(75, 23);
			this.ctlSave.TabIndex = 2;
			this.ctlSave.Text = "保存";
			this.ctlSave.UseVisualStyleBackColor = true;
			this.ctlSave.Click += new System.EventHandler(this.ctlSave_Click);
			// 
			// ctlPowerBy
			// 
			this.ctlPowerBy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.ctlPowerBy.AutoSize = true;
			this.ctlPowerBy.Location = new System.Drawing.Point(6, 311);
			this.ctlPowerBy.Name = "ctlPowerBy";
			this.ctlPowerBy.Size = new System.Drawing.Size(113, 12);
			this.ctlPowerBy.TabIndex = 3;
			this.ctlPowerBy.TabStop = true;
			this.ctlPowerBy.Text = "Powered By Serfend";
			this.ctlPowerBy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ctlPowerBy_LinkClicked);
			// 
			// ctlLoadSetting
			// 
			this.ctlLoadSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ctlLoadSetting.Location = new System.Drawing.Point(412, 306);
			this.ctlLoadSetting.Name = "ctlLoadSetting";
			this.ctlLoadSetting.Size = new System.Drawing.Size(75, 23);
			this.ctlLoadSetting.TabIndex = 4;
			this.ctlLoadSetting.Text = "加载";
			this.ctlLoadSetting.UseVisualStyleBackColor = true;
			// 
			// ConfigView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.ctlLoadSetting);
			this.Controls.Add(this.ctlPowerBy);
			this.Controls.Add(this.ctlSave);
			this.Controls.Add(this.ctlPaths);
			this.Controls.Add(this.ctlEnable);
			this.Name = "ConfigView";
			this.Size = new System.Drawing.Size(577, 344);
			this.Load += new System.EventHandler(this.ConfigView_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox ctlEnable;
        private System.Windows.Forms.TextBox ctlPaths;
        private System.Windows.Forms.Button ctlSave;
        private System.Windows.Forms.LinkLabel ctlPowerBy;
		private System.Windows.Forms.Button ctlLoadSetting;
	}
}
