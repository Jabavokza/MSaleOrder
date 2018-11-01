namespace MSaleOrder
{
    partial class wPlantCode
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
            this.otbPCCngPlntCd = new System.Windows.Forms.TextBox();
            this.ocmPCCngOK = new System.Windows.Forms.Button();
            this.ocmPCCngCancel = new System.Windows.Forms.Button();
            this.olaPCCngMssg = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // otbPCCngPlntCd
            // 
            this.otbPCCngPlntCd.Location = new System.Drawing.Point(24, 32);
            this.otbPCCngPlntCd.MaxLength = 4;
            this.otbPCCngPlntCd.Name = "otbPCCngPlntCd";
            this.otbPCCngPlntCd.Size = new System.Drawing.Size(149, 22);
            this.otbPCCngPlntCd.TabIndex = 0;
            // 
            // ocmPCCngOK
            // 
            this.ocmPCCngOK.Location = new System.Drawing.Point(24, 60);
            this.ocmPCCngOK.Name = "ocmPCCngOK";
            this.ocmPCCngOK.Size = new System.Drawing.Size(64, 28);
            this.ocmPCCngOK.TabIndex = 1;
            this.ocmPCCngOK.Text = "OK";
            this.ocmPCCngOK.UseVisualStyleBackColor = true;
            this.ocmPCCngOK.Click += new System.EventHandler(this.ocmPCCngOK_Click);
            // 
            // ocmPCCngCancel
            // 
            this.ocmPCCngCancel.Location = new System.Drawing.Point(99, 60);
            this.ocmPCCngCancel.Name = "ocmPCCngCancel";
            this.ocmPCCngCancel.Size = new System.Drawing.Size(74, 28);
            this.ocmPCCngCancel.TabIndex = 2;
            this.ocmPCCngCancel.Text = "Cancel";
            this.ocmPCCngCancel.UseVisualStyleBackColor = true;
            this.ocmPCCngCancel.Click += new System.EventHandler(this.ocmPCCngCancel_Click);
            // 
            // olaPCCngMssg
            // 
            this.olaPCCngMssg.AutoSize = true;
            this.olaPCCngMssg.Location = new System.Drawing.Point(25, 11);
            this.olaPCCngMssg.Name = "olaPCCngMssg";
            this.olaPCCngMssg.Size = new System.Drawing.Size(0, 17);
            this.olaPCCngMssg.TabIndex = 3;
            // 
            // wPlantCode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(197, 101);
            this.Controls.Add(this.olaPCCngMssg);
            this.Controls.Add(this.ocmPCCngCancel);
            this.Controls.Add(this.ocmPCCngOK);
            this.Controls.Add(this.otbPCCngPlntCd);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "wPlantCode";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.wPlantCode_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox otbPCCngPlntCd;
        private System.Windows.Forms.Button ocmPCCngOK;
        private System.Windows.Forms.Button ocmPCCngCancel;
        private System.Windows.Forms.Label olaPCCngMssg;
    }
}