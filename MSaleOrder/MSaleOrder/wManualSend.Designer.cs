namespace MSaleOrder
{
    partial class wManualSend
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.oraSendAllRow = new System.Windows.Forms.RadioButton();
            this.oraSendSelected = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ockStaFailed = new System.Windows.Forms.CheckBox();
            this.ockStaSuccess = new System.Windows.Forms.CheckBox();
            this.ockStaUnsent = new System.Windows.Forms.CheckBox();
            this.ocmSendCancel = new System.Windows.Forms.Button();
            this.ocmSendOK = new System.Windows.Forms.Button();
            this.olbMnlSndPlnList = new System.Windows.Forms.CheckedListBox();
            this.ockMnlSndAllPln = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.oraSendAllRow);
            this.groupBox1.Controls.Add(this.oraSendSelected);
            this.groupBox1.Location = new System.Drawing.Point(177, 13);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(270, 57);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Send which data";
            // 
            // oraSendAllRow
            // 
            this.oraSendAllRow.AutoSize = true;
            this.oraSendAllRow.Checked = true;
            this.oraSendAllRow.Location = new System.Drawing.Point(15, 23);
            this.oraSendAllRow.Margin = new System.Windows.Forms.Padding(4);
            this.oraSendAllRow.Name = "oraSendAllRow";
            this.oraSendAllRow.Size = new System.Drawing.Size(127, 21);
            this.oraSendAllRow.TabIndex = 1;
            this.oraSendAllRow.TabStop = true;
            this.oraSendAllRow.Text = "All rows in table";
            this.oraSendAllRow.UseVisualStyleBackColor = true;
            this.oraSendAllRow.CheckedChanged += new System.EventHandler(this.oraSendAllRow_CheckedChanged);
            // 
            // oraSendSelected
            // 
            this.oraSendSelected.AutoSize = true;
            this.oraSendSelected.Location = new System.Drawing.Point(145, 23);
            this.oraSendSelected.Margin = new System.Windows.Forms.Padding(4);
            this.oraSendSelected.Name = "oraSendSelected";
            this.oraSendSelected.Size = new System.Drawing.Size(117, 21);
            this.oraSendSelected.TabIndex = 0;
            this.oraSendSelected.Text = "Selected rows";
            this.oraSendSelected.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ockStaFailed);
            this.groupBox2.Controls.Add(this.ockStaSuccess);
            this.groupBox2.Controls.Add(this.ockStaUnsent);
            this.groupBox2.Location = new System.Drawing.Point(177, 77);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(270, 84);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Which status";
            // 
            // ockStaFailed
            // 
            this.ockStaFailed.AutoSize = true;
            this.ockStaFailed.Checked = true;
            this.ockStaFailed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ockStaFailed.Location = new System.Drawing.Point(15, 54);
            this.ockStaFailed.Margin = new System.Windows.Forms.Padding(4);
            this.ockStaFailed.Name = "ockStaFailed";
            this.ockStaFailed.Size = new System.Drawing.Size(68, 21);
            this.ockStaFailed.TabIndex = 2;
            this.ockStaFailed.Text = "Failed";
            this.ockStaFailed.UseVisualStyleBackColor = true;
            // 
            // ockStaSuccess
            // 
            this.ockStaSuccess.AutoSize = true;
            this.ockStaSuccess.Location = new System.Drawing.Point(134, 25);
            this.ockStaSuccess.Margin = new System.Windows.Forms.Padding(4);
            this.ockStaSuccess.Name = "ockStaSuccess";
            this.ockStaSuccess.Size = new System.Drawing.Size(83, 21);
            this.ockStaSuccess.TabIndex = 1;
            this.ockStaSuccess.Text = "Success";
            this.ockStaSuccess.UseVisualStyleBackColor = true;
            // 
            // ockStaUnsent
            // 
            this.ockStaUnsent.AutoSize = true;
            this.ockStaUnsent.Checked = true;
            this.ockStaUnsent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ockStaUnsent.Location = new System.Drawing.Point(15, 25);
            this.ockStaUnsent.Margin = new System.Windows.Forms.Padding(4);
            this.ockStaUnsent.Name = "ockStaUnsent";
            this.ockStaUnsent.Size = new System.Drawing.Size(79, 21);
            this.ockStaUnsent.TabIndex = 0;
            this.ockStaUnsent.Text = "Unsent ";
            this.ockStaUnsent.UseVisualStyleBackColor = true;
            // 
            // ocmSendCancel
            // 
            this.ocmSendCancel.Location = new System.Drawing.Point(177, 170);
            this.ocmSendCancel.Margin = new System.Windows.Forms.Padding(4);
            this.ocmSendCancel.Name = "ocmSendCancel";
            this.ocmSendCancel.Size = new System.Drawing.Size(103, 27);
            this.ocmSendCancel.TabIndex = 2;
            this.ocmSendCancel.Text = "Cancel";
            this.ocmSendCancel.UseVisualStyleBackColor = true;
            this.ocmSendCancel.Click += new System.EventHandler(this.ocmSendCancel_Click);
            // 
            // ocmSendOK
            // 
            this.ocmSendOK.Location = new System.Drawing.Point(344, 170);
            this.ocmSendOK.Margin = new System.Windows.Forms.Padding(4);
            this.ocmSendOK.Name = "ocmSendOK";
            this.ocmSendOK.Size = new System.Drawing.Size(103, 27);
            this.ocmSendOK.TabIndex = 3;
            this.ocmSendOK.Text = "Send";
            this.ocmSendOK.UseVisualStyleBackColor = true;
            this.ocmSendOK.Click += new System.EventHandler(this.ocmSendOK_Click);
            // 
            // olbMnlSndPlnList
            // 
            this.olbMnlSndPlnList.FormattingEnabled = true;
            this.olbMnlSndPlnList.Location = new System.Drawing.Point(12, 38);
            this.olbMnlSndPlnList.Name = "olbMnlSndPlnList";
            this.olbMnlSndPlnList.Size = new System.Drawing.Size(158, 157);
            this.olbMnlSndPlnList.TabIndex = 4;
            this.olbMnlSndPlnList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.olbMnlSndPlnList_ItemCheck);
            // 
            // ockMnlSndAllPln
            // 
            this.ockMnlSndAllPln.AutoSize = true;
            this.ockMnlSndAllPln.Location = new System.Drawing.Point(12, 13);
            this.ockMnlSndAllPln.Name = "ockMnlSndAllPln";
            this.ockMnlSndAllPln.Size = new System.Drawing.Size(81, 21);
            this.ockMnlSndAllPln.TabIndex = 5;
            this.ockMnlSndAllPln.Text = "All Plant";
            this.ockMnlSndAllPln.UseVisualStyleBackColor = true;
            this.ockMnlSndAllPln.CheckedChanged += new System.EventHandler(this.ockMnlSndAllPln_CheckedChanged);
            // 
            // wManualSend
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 204);
            this.Controls.Add(this.ockMnlSndAllPln);
            this.Controls.Add(this.olbMnlSndPlnList);
            this.Controls.Add(this.ocmSendOK);
            this.Controls.Add(this.ocmSendCancel);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "wManualSend";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "wManualSend";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton oraSendAllRow;
        private System.Windows.Forms.RadioButton oraSendSelected;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox ockStaFailed;
        private System.Windows.Forms.CheckBox ockStaSuccess;
        private System.Windows.Forms.CheckBox ockStaUnsent;
        private System.Windows.Forms.Button ocmSendCancel;
        private System.Windows.Forms.Button ocmSendOK;
        private System.Windows.Forms.CheckedListBox olbMnlSndPlnList;
        private System.Windows.Forms.CheckBox ockMnlSndAllPln;
    }
}