namespace SiriAssistant
{
    partial class MainForm
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.IPLabel = new System.Windows.Forms.Label();
            this.IPValueLabel = new System.Windows.Forms.Label();
            this.PortLabel = new System.Windows.Forms.Label();
            this.PortText = new System.Windows.Forms.TextBox();
            this.ExecLogText = new System.Windows.Forms.RichTextBox();
            this.SaveBtn = new System.Windows.Forms.Button();
            this.NotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.LogPanel = new System.Windows.Forms.Panel();
            this.LogPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // IPLabel
            // 
            this.IPLabel.AutoSize = true;
            this.IPLabel.Location = new System.Drawing.Point(25, 25);
            this.IPLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.IPLabel.Name = "IPLabel";
            this.IPLabel.Size = new System.Drawing.Size(41, 12);
            this.IPLabel.TabIndex = 0;
            this.IPLabel.Text = "本机IP";
            // 
            // IPValueLabel
            // 
            this.IPValueLabel.Location = new System.Drawing.Point(75, 25);
            this.IPValueLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.IPValueLabel.Name = "IPValueLabel";
            this.IPValueLabel.Size = new System.Drawing.Size(90, 12);
            this.IPValueLabel.TabIndex = 1;
            // 
            // PortLabel
            // 
            this.PortLabel.AutoSize = true;
            this.PortLabel.Location = new System.Drawing.Point(200, 25);
            this.PortLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.PortLabel.Name = "PortLabel";
            this.PortLabel.Size = new System.Drawing.Size(29, 12);
            this.PortLabel.TabIndex = 0;
            this.PortLabel.Text = "端口";
            // 
            // PortText
            // 
            this.PortText.Location = new System.Drawing.Point(240, 22);
            this.PortText.Margin = new System.Windows.Forms.Padding(2);
            this.PortText.Name = "PortText";
            this.PortText.Size = new System.Drawing.Size(52, 21);
            this.PortText.TabIndex = 1;
            // 
            // ExecLogText
            // 
            this.ExecLogText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ExecLogText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ExecLogText.Font = new System.Drawing.Font("Cascadia Mono", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExecLogText.Location = new System.Drawing.Point(0, 0);
            this.ExecLogText.Margin = new System.Windows.Forms.Padding(2);
            this.ExecLogText.Name = "ExecLogText";
            this.ExecLogText.ReadOnly = true;
            this.ExecLogText.Size = new System.Drawing.Size(742, 328);
            this.ExecLogText.TabIndex = 1;
            this.ExecLogText.Text = "";
            // 
            // SaveBtn
            // 
            this.SaveBtn.Enabled = false;
            this.SaveBtn.Location = new System.Drawing.Point(696, 402);
            this.SaveBtn.Name = "SaveBtn";
            this.SaveBtn.Size = new System.Drawing.Size(75, 25);
            this.SaveBtn.TabIndex = 2;
            this.SaveBtn.Text = "应用更改";
            this.SaveBtn.UseVisualStyleBackColor = true;
            // 
            // NotifyIcon
            // 
            this.NotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("NotifyIcon.Icon")));
            this.NotifyIcon.Text = "SiriAssistant";
            this.NotifyIcon.Visible = true;
            this.NotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_MouseDoubleClick);
            // 
            // LogPanel
            // 
            this.LogPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LogPanel.Controls.Add(this.ExecLogText);
            this.LogPanel.Location = new System.Drawing.Point(27, 55);
            this.LogPanel.Margin = new System.Windows.Forms.Padding(2);
            this.LogPanel.Name = "LogPanel";
            this.LogPanel.Size = new System.Drawing.Size(744, 330);
            this.LogPanel.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.LogPanel);
            this.Controls.Add(this.SaveBtn);
            this.Controls.Add(this.IPValueLabel);
            this.Controls.Add(this.IPLabel);
            this.Controls.Add(this.PortLabel);
            this.Controls.Add(this.PortText);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.Text = "SiriAssistant";
            this.LogPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label IPLabel;
        private System.Windows.Forms.Label IPValueLabel;
        private System.Windows.Forms.Label PortLabel;
        private System.Windows.Forms.TextBox PortText;
        private System.Windows.Forms.RichTextBox ExecLogText;
        private System.Windows.Forms.Button SaveBtn;
        private System.Windows.Forms.NotifyIcon NotifyIcon;
        private System.Windows.Forms.Panel LogPanel;
    }
}

