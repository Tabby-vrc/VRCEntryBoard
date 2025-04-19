namespace VRCEntryBoard.HMI
{
    partial class CEntryView
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
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
            this.pnlPlayerList = new System.Windows.Forms.FlowLayoutPanel();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.lblReg1Num = new System.Windows.Forms.Label();
            this.lblReg1 = new System.Windows.Forms.Label();
            this.rBtnEntry = new System.Windows.Forms.RadioButton();
            this.rBtnVisiter = new System.Windows.Forms.RadioButton();
            this.rBtnAskMe = new System.Windows.Forms.RadioButton();
            this.rBtnNone = new System.Windows.Forms.RadioButton();
            this.btnOutput = new System.Windows.Forms.Button();
            this.btnListUpdate = new System.Windows.Forms.Button();
            this.chkBoxStaff = new System.Windows.Forms.CheckBox();
            this.lblBeginner = new System.Windows.Forms.Label();
            this.lblBeginnerNum = new System.Windows.Forms.Label();
            this.lblStaff = new System.Windows.Forms.Label();
            this.lblStaffNum = new System.Windows.Forms.Label();
            this.lblInstance = new System.Windows.Forms.Label();
            this.lblInstanceNum = new System.Windows.Forms.Label();
            this.lblReg2Num = new System.Windows.Forms.Label();
            this.lblReg2 = new System.Windows.Forms.Label();
            this.rBtnReg1 = new System.Windows.Forms.RadioButton();
            this.rBtnReg2 = new System.Windows.Forms.RadioButton();
            this.rBtnBeginner = new System.Windows.Forms.RadioButton();
            this.gBoxEntry = new System.Windows.Forms.Panel();
            this.gBoxReg = new System.Windows.Forms.Panel();
            this.gBoxEntry.SuspendLayout();
            this.gBoxReg.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlPlayerList
            // 
            this.pnlPlayerList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlPlayerList.AutoScroll = true;
            this.pnlPlayerList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlPlayerList.Location = new System.Drawing.Point(16, 62);
            this.pnlPlayerList.Margin = new System.Windows.Forms.Padding(16, 8, 4, 8);
            this.pnlPlayerList.Name = "pnlPlayerList";
            this.pnlPlayerList.Size = new System.Drawing.Size(980, 570);
            this.pnlPlayerList.TabIndex = 2;
            // 
            // lblReg1Num
            // 
            this.lblReg1Num.Font = new System.Drawing.Font("Noto Sans JP", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblReg1Num.Location = new System.Drawing.Point(128, 4);
            this.lblReg1Num.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblReg1Num.Name = "lblReg1Num";
            this.lblReg1Num.Size = new System.Drawing.Size(65, 54);
            this.lblReg1Num.TabIndex = 8;
            this.lblReg1Num.Text = "99";
            // 
            // lblReg1
            // 
            this.lblReg1.Font = new System.Drawing.Font("Noto Sans JP", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblReg1.Location = new System.Drawing.Point(10, 19);
            this.lblReg1.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.lblReg1.Name = "lblReg1";
            this.lblReg1.Size = new System.Drawing.Size(135, 35);
            this.lblReg1.TabIndex = 9;
            this.lblReg1.Text = "レギュ①：";
            // 
            // rBtnEntry
            // 
            this.rBtnEntry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rBtnEntry.Appearance = System.Windows.Forms.Appearance.Button;
            this.rBtnEntry.Font = new System.Drawing.Font("Noto Sans JP", 14.25F);
            this.rBtnEntry.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rBtnEntry.Location = new System.Drawing.Point(17, 31);
            this.rBtnEntry.Margin = new System.Windows.Forms.Padding(3, 16, 3, 0);
            this.rBtnEntry.Name = "rBtnEntry";
            this.rBtnEntry.Size = new System.Drawing.Size(136, 56);
            this.rBtnEntry.TabIndex = 10;
            this.rBtnEntry.TabStop = true;
            this.rBtnEntry.Text = "参加";
            this.rBtnEntry.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.rBtnEntry.UseVisualStyleBackColor = true;
            this.rBtnEntry.Click += new System.EventHandler(this.rBtnEntry_Click);
            // 
            // rBtnVisiter
            // 
            this.rBtnVisiter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rBtnVisiter.Appearance = System.Windows.Forms.Appearance.Button;
            this.rBtnVisiter.Font = new System.Drawing.Font("Noto Sans JP", 14.25F);
            this.rBtnVisiter.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rBtnVisiter.Location = new System.Drawing.Point(17, 92);
            this.rBtnVisiter.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.rBtnVisiter.Name = "rBtnVisiter";
            this.rBtnVisiter.Size = new System.Drawing.Size(136, 56);
            this.rBtnVisiter.TabIndex = 11;
            this.rBtnVisiter.TabStop = true;
            this.rBtnVisiter.Text = "見学";
            this.rBtnVisiter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.rBtnVisiter.UseVisualStyleBackColor = true;
            this.rBtnVisiter.Click += new System.EventHandler(this.rBtnVisiter_Click);
            // 
            // rBtnAskMe
            // 
            this.rBtnAskMe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rBtnAskMe.Appearance = System.Windows.Forms.Appearance.Button;
            this.rBtnAskMe.Font = new System.Drawing.Font("Noto Sans JP", 14.25F);
            this.rBtnAskMe.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rBtnAskMe.Location = new System.Drawing.Point(17, 153);
            this.rBtnAskMe.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.rBtnAskMe.Name = "rBtnAskMe";
            this.rBtnAskMe.Size = new System.Drawing.Size(136, 56);
            this.rBtnAskMe.TabIndex = 12;
            this.rBtnAskMe.TabStop = true;
            this.rBtnAskMe.Text = "未確認";
            this.rBtnAskMe.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.rBtnAskMe.UseVisualStyleBackColor = true;
            this.rBtnAskMe.Click += new System.EventHandler(this.rBtnAskMe_Click);
            // 
            // rBtnNone
            // 
            this.rBtnNone.Appearance = System.Windows.Forms.Appearance.Button;
            this.rBtnNone.Font = new System.Drawing.Font("Noto Sans JP", 14.25F);
            this.rBtnNone.Location = new System.Drawing.Point(17, 23);
            this.rBtnNone.Name = "rBtnNone";
            this.rBtnNone.Size = new System.Drawing.Size(136, 56);
            this.rBtnNone.TabIndex = 13;
            this.rBtnNone.TabStop = true;
            this.rBtnNone.Text = "未選択";
            this.rBtnNone.UseVisualStyleBackColor = true;
            this.rBtnNone.Visible = false;
            // 
            // btnOutput
            // 
            this.btnOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOutput.Font = new System.Drawing.Font("Noto Sans JP", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnOutput.Image = global::VRCEntryBoard.Properties.Resources.upload_48;
            this.btnOutput.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOutput.Location = new System.Drawing.Point(1003, 124);
            this.btnOutput.Margin = new System.Windows.Forms.Padding(16, 6, 16, 0);
            this.btnOutput.Name = "btnOutput";
            this.btnOutput.Size = new System.Drawing.Size(136, 56);
            this.btnOutput.TabIndex = 6;
            this.btnOutput.Text = "出力";
            this.btnOutput.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnOutput.UseVisualStyleBackColor = true;
            this.btnOutput.Click += new System.EventHandler(this.btnOutput_Click);
            // 
            // btnListUpdate
            // 
            this.btnListUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnListUpdate.Font = new System.Drawing.Font("Noto Sans JP", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnListUpdate.Image = global::VRCEntryBoard.Properties.Resources.renew_48;
            this.btnListUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnListUpdate.Location = new System.Drawing.Point(1003, 62);
            this.btnListUpdate.Margin = new System.Windows.Forms.Padding(16, 16, 16, 0);
            this.btnListUpdate.Name = "btnListUpdate";
            this.btnListUpdate.Size = new System.Drawing.Size(136, 56);
            this.btnListUpdate.TabIndex = 4;
            this.btnListUpdate.Text = "更新";
            this.btnListUpdate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnListUpdate.UseVisualStyleBackColor = true;
            this.btnListUpdate.Click += new System.EventHandler(this.btnListUpdate_Click);
            // 
            // chkBoxStaff
            // 
            this.chkBoxStaff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkBoxStaff.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkBoxStaff.Font = new System.Drawing.Font("Noto Sans JP", 14.25F);
            this.chkBoxStaff.Image = global::VRCEntryBoard.Properties.Resources.staff_48;
            this.chkBoxStaff.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.chkBoxStaff.Location = new System.Drawing.Point(1003, 576);
            this.chkBoxStaff.Margin = new System.Windows.Forms.Padding(3, 8, 3, 0);
            this.chkBoxStaff.Name = "chkBoxStaff";
            this.chkBoxStaff.Size = new System.Drawing.Size(136, 56);
            this.chkBoxStaff.TabIndex = 16;
            this.chkBoxStaff.Text = "Staff";
            this.chkBoxStaff.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.chkBoxStaff.UseVisualStyleBackColor = true;
            this.chkBoxStaff.Click += new System.EventHandler(this.chkBoxStaff_Click);
            // 
            // lblBeginner
            // 
            this.lblBeginner.Font = new System.Drawing.Font("Noto Sans JP", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBeginner.Location = new System.Drawing.Point(378, 19);
            this.lblBeginner.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.lblBeginner.Name = "lblBeginner";
            this.lblBeginner.Size = new System.Drawing.Size(135, 35);
            this.lblBeginner.TabIndex = 17;
            this.lblBeginner.Text = "初心者枠：";
            // 
            // lblBeginnerNum
            // 
            this.lblBeginnerNum.AutoSize = true;
            this.lblBeginnerNum.Font = new System.Drawing.Font("Noto Sans JP", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblBeginnerNum.Location = new System.Drawing.Point(495, 4);
            this.lblBeginnerNum.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblBeginnerNum.Name = "lblBeginnerNum";
            this.lblBeginnerNum.Size = new System.Drawing.Size(65, 54);
            this.lblBeginnerNum.TabIndex = 18;
            this.lblBeginnerNum.Text = "99";
            // 
            // lblStaff
            // 
            this.lblStaff.AutoSize = true;
            this.lblStaff.Font = new System.Drawing.Font("Noto Sans JP", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblStaff.Location = new System.Drawing.Point(563, 19);
            this.lblStaff.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.lblStaff.Name = "lblStaff";
            this.lblStaff.Size = new System.Drawing.Size(159, 35);
            this.lblStaff.TabIndex = 19;
            this.lblStaff.Text = "スタッフ数：";
            // 
            // lblStaffNum
            // 
            this.lblStaffNum.AutoSize = true;
            this.lblStaffNum.Font = new System.Drawing.Font("Noto Sans JP", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblStaffNum.Location = new System.Drawing.Point(705, 4);
            this.lblStaffNum.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblStaffNum.Name = "lblStaffNum";
            this.lblStaffNum.Size = new System.Drawing.Size(65, 54);
            this.lblStaffNum.TabIndex = 20;
            this.lblStaffNum.Text = "99";
            // 
            // lblInstance
            // 
            this.lblInstance.AutoSize = true;
            this.lblInstance.Font = new System.Drawing.Font("Noto Sans JP", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblInstance.Location = new System.Drawing.Point(777, 19);
            this.lblInstance.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.lblInstance.Name = "lblInstance";
            this.lblInstance.Size = new System.Drawing.Size(207, 35);
            this.lblInstance.TabIndex = 21;
            this.lblInstance.Text = "インスタンス数：";
            // 
            // lblInstanceNum
            // 
            this.lblInstanceNum.AutoSize = true;
            this.lblInstanceNum.Font = new System.Drawing.Font("Noto Sans JP", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblInstanceNum.Location = new System.Drawing.Point(968, 4);
            this.lblInstanceNum.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblInstanceNum.Name = "lblInstanceNum";
            this.lblInstanceNum.Size = new System.Drawing.Size(65, 54);
            this.lblInstanceNum.TabIndex = 22;
            this.lblInstanceNum.Text = "99";
            // 
            // lblReg2Num
            // 
            this.lblReg2Num.Font = new System.Drawing.Font("Noto Sans JP", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblReg2Num.Location = new System.Drawing.Point(307, 4);
            this.lblReg2Num.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblReg2Num.Name = "lblReg2Num";
            this.lblReg2Num.Size = new System.Drawing.Size(65, 54);
            this.lblReg2Num.TabIndex = 23;
            this.lblReg2Num.Text = "99";
            // 
            // lblReg2
            // 
            this.lblReg2.Font = new System.Drawing.Font("Noto Sans JP", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblReg2.Location = new System.Drawing.Point(190, 19);
            this.lblReg2.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.lblReg2.Name = "lblReg2";
            this.lblReg2.Size = new System.Drawing.Size(135, 35);
            this.lblReg2.TabIndex = 24;
            this.lblReg2.Text = "レギュ②：";
            // 
            // rBtnReg1
            // 
            this.rBtnReg1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rBtnReg1.Appearance = System.Windows.Forms.Appearance.Button;
            this.rBtnReg1.Font = new System.Drawing.Font("Noto Sans JP", 14.25F);
            this.rBtnReg1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rBtnReg1.Location = new System.Drawing.Point(17, 18);
            this.rBtnReg1.Margin = new System.Windows.Forms.Padding(3, 16, 3, 0);
            this.rBtnReg1.Name = "rBtnReg1";
            this.rBtnReg1.Size = new System.Drawing.Size(136, 56);
            this.rBtnReg1.TabIndex = 25;
            this.rBtnReg1.TabStop = true;
            this.rBtnReg1.Text = "①";
            this.rBtnReg1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.rBtnReg1.UseVisualStyleBackColor = true;
            this.rBtnReg1.Click += new System.EventHandler(this.rBtnReg1_Click);
            // 
            // rBtnReg2
            // 
            this.rBtnReg2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rBtnReg2.Appearance = System.Windows.Forms.Appearance.Button;
            this.rBtnReg2.Font = new System.Drawing.Font("Noto Sans JP", 14.25F);
            this.rBtnReg2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rBtnReg2.Location = new System.Drawing.Point(17, 79);
            this.rBtnReg2.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.rBtnReg2.Name = "rBtnReg2";
            this.rBtnReg2.Size = new System.Drawing.Size(136, 56);
            this.rBtnReg2.TabIndex = 26;
            this.rBtnReg2.TabStop = true;
            this.rBtnReg2.Text = "②";
            this.rBtnReg2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.rBtnReg2.UseVisualStyleBackColor = true;
            this.rBtnReg2.Click += new System.EventHandler(this.rBtnReg2_Click);
            // 
            // rBtnBeginner
            // 
            this.rBtnBeginner.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rBtnBeginner.Appearance = System.Windows.Forms.Appearance.Button;
            this.rBtnBeginner.Font = new System.Drawing.Font("Noto Sans JP", 14.25F);
            this.rBtnBeginner.Image = global::VRCEntryBoard.Properties.Resources.newuser_48;
            this.rBtnBeginner.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rBtnBeginner.Location = new System.Drawing.Point(17, 140);
            this.rBtnBeginner.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.rBtnBeginner.Name = "rBtnBeginner";
            this.rBtnBeginner.Size = new System.Drawing.Size(136, 56);
            this.rBtnBeginner.TabIndex = 27;
            this.rBtnBeginner.TabStop = true;
            this.rBtnBeginner.Text = "初心者";
            this.rBtnBeginner.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.rBtnBeginner.UseVisualStyleBackColor = true;
            this.rBtnBeginner.Click += new System.EventHandler(this.rBtnBeginner_Click);
            // 
            // gBoxEntry
            // 
            this.gBoxEntry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gBoxEntry.Controls.Add(this.rBtnEntry);
            this.gBoxEntry.Controls.Add(this.rBtnNone);
            this.gBoxEntry.Controls.Add(this.rBtnVisiter);
            this.gBoxEntry.Controls.Add(this.rBtnAskMe);
            this.gBoxEntry.Location = new System.Drawing.Point(986, 160);
            this.gBoxEntry.Name = "gBoxEntry";
            this.gBoxEntry.Size = new System.Drawing.Size(159, 226);
            this.gBoxEntry.TabIndex = 28;
            // 
            // gBoxReg
            // 
            this.gBoxReg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gBoxReg.Controls.Add(this.rBtnBeginner);
            this.gBoxReg.Controls.Add(this.rBtnReg1);
            this.gBoxReg.Controls.Add(this.rBtnReg2);
            this.gBoxReg.Location = new System.Drawing.Point(986, 372);
            this.gBoxReg.Name = "gBoxReg";
            this.gBoxReg.Size = new System.Drawing.Size(169, 196);
            this.gBoxReg.TabIndex = 29;
            // 
            // CEntryView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.lblReg2Num);
            this.Controls.Add(this.lblReg2);
            this.Controls.Add(this.lblReg1Num);
            this.Controls.Add(this.lblInstanceNum);
            this.Controls.Add(this.lblInstance);
            this.Controls.Add(this.lblStaffNum);
            this.Controls.Add(this.lblStaff);
            this.Controls.Add(this.lblBeginnerNum);
            this.Controls.Add(this.lblBeginner);
            this.Controls.Add(this.chkBoxStaff);
            this.Controls.Add(this.lblReg1);
            this.Controls.Add(this.btnOutput);
            this.Controls.Add(this.btnListUpdate);
            this.Controls.Add(this.pnlPlayerList);
            this.Controls.Add(this.gBoxEntry);
            this.Controls.Add(this.gBoxReg);
            this.Name = "CEntryView";
            this.Size = new System.Drawing.Size(1145, 640);
            this.Load += new System.EventHandler(this.CEntryView_Load);
            this.gBoxEntry.ResumeLayout(false);
            this.gBoxReg.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel pnlPlayerList;
        private System.Windows.Forms.Button btnListUpdate;
        private System.Windows.Forms.Button btnOutput;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label lblReg1Num;
        private System.Windows.Forms.Label lblReg1;
        private System.Windows.Forms.RadioButton rBtnEntry;
        private System.Windows.Forms.RadioButton rBtnVisiter;
        private System.Windows.Forms.RadioButton rBtnAskMe;
        private System.Windows.Forms.RadioButton rBtnNone;
        private System.Windows.Forms.CheckBox chkBoxStaff;
        private System.Windows.Forms.Label lblBeginner;
        private System.Windows.Forms.Label lblBeginnerNum;
        private System.Windows.Forms.Label lblStaff;
        private System.Windows.Forms.Label lblStaffNum;
        private System.Windows.Forms.Label lblInstance;
        private System.Windows.Forms.Label lblInstanceNum;
        private System.Windows.Forms.Label lblReg2Num;
        private System.Windows.Forms.Label lblReg2;
        private System.Windows.Forms.RadioButton rBtnReg1;
        private System.Windows.Forms.RadioButton rBtnReg2;
        private System.Windows.Forms.RadioButton rBtnBeginner;
        private System.Windows.Forms.Panel gBoxEntry;
        private System.Windows.Forms.Panel gBoxReg;
    }
}
