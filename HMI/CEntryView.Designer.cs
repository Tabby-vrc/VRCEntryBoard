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
            this.lblEntryNum = new System.Windows.Forms.Label();
            this.lblEntry = new System.Windows.Forms.Label();
            this.rBtnEntry = new System.Windows.Forms.RadioButton();
            this.rBtnVisiter = new System.Windows.Forms.RadioButton();
            this.rBtnAskMe = new System.Windows.Forms.RadioButton();
            this.rBtnNone = new System.Windows.Forms.RadioButton();
            this.btnOutput = new System.Windows.Forms.Button();
            this.btnListUpdate = new System.Windows.Forms.Button();
            this.chkBoxBeginner = new System.Windows.Forms.CheckBox();
            this.chkBoxStaff = new System.Windows.Forms.CheckBox();
            this.lblBeginner = new System.Windows.Forms.Label();
            this.lblBeginnerNum = new System.Windows.Forms.Label();
            this.lblStaff = new System.Windows.Forms.Label();
            this.lblStaffNum = new System.Windows.Forms.Label();
            this.lblInstance = new System.Windows.Forms.Label();
            this.lblInstanceNum = new System.Windows.Forms.Label();
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
            this.pnlPlayerList.Size = new System.Drawing.Size(980, 464);
            this.pnlPlayerList.TabIndex = 2;
            // 
            // lblEntryNum
            // 
            this.lblEntryNum.Font = new System.Drawing.Font("Noto Sans JP", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblEntryNum.Location = new System.Drawing.Point(128, 4);
            this.lblEntryNum.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblEntryNum.Name = "lblEntryNum";
            this.lblEntryNum.Size = new System.Drawing.Size(65, 54);
            this.lblEntryNum.TabIndex = 8;
            this.lblEntryNum.Text = "99";
            // 
            // lblEntry
            // 
            this.lblEntry.Font = new System.Drawing.Font("Noto Sans JP", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblEntry.Location = new System.Drawing.Point(10, 19);
            this.lblEntry.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.lblEntry.Name = "lblEntry";
            this.lblEntry.Size = new System.Drawing.Size(135, 35);
            this.lblEntry.TabIndex = 9;
            this.lblEntry.Text = "経験者枠：";
            // 
            // rBtnEntry
            // 
            this.rBtnEntry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rBtnEntry.Appearance = System.Windows.Forms.Appearance.Button;
            this.rBtnEntry.Font = new System.Drawing.Font("Noto Sans JP", 14.25F);
            this.rBtnEntry.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rBtnEntry.Location = new System.Drawing.Point(1000, 196);
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
            this.rBtnVisiter.Location = new System.Drawing.Point(1000, 257);
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
            this.rBtnAskMe.Location = new System.Drawing.Point(1000, 318);
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
            this.rBtnNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rBtnNone.Appearance = System.Windows.Forms.Appearance.Button;
            this.rBtnNone.Font = new System.Drawing.Font("Noto Sans JP", 14.25F);
            this.rBtnNone.Location = new System.Drawing.Point(1004, 0);
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
            this.btnOutput.Location = new System.Drawing.Point(1000, 124);
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
            this.btnListUpdate.Location = new System.Drawing.Point(1000, 62);
            this.btnListUpdate.Margin = new System.Windows.Forms.Padding(16, 16, 16, 0);
            this.btnListUpdate.Name = "btnListUpdate";
            this.btnListUpdate.Size = new System.Drawing.Size(136, 56);
            this.btnListUpdate.TabIndex = 4;
            this.btnListUpdate.Text = "更新";
            this.btnListUpdate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnListUpdate.UseVisualStyleBackColor = true;
            this.btnListUpdate.Click += new System.EventHandler(this.btnListUpdate_Click);
            // 
            // chkBoxBeginner
            // 
            this.chkBoxBeginner.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkBoxBeginner.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkBoxBeginner.Font = new System.Drawing.Font("Noto Sans JP", 14.25F);
            this.chkBoxBeginner.Image = global::VRCEntryBoard.Properties.Resources.newuser_48;
            this.chkBoxBeginner.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.chkBoxBeginner.Location = new System.Drawing.Point(1000, 382);
            this.chkBoxBeginner.Margin = new System.Windows.Forms.Padding(3, 8, 3, 0);
            this.chkBoxBeginner.Name = "chkBoxBeginner";
            this.chkBoxBeginner.Size = new System.Drawing.Size(136, 56);
            this.chkBoxBeginner.TabIndex = 15;
            this.chkBoxBeginner.Text = "初心者";
            this.chkBoxBeginner.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.chkBoxBeginner.UseVisualStyleBackColor = true;
            this.chkBoxBeginner.Click += new System.EventHandler(this.chkBoxBeginner_Click);
            // 
            // chkBoxStaff
            // 
            this.chkBoxStaff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkBoxStaff.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkBoxStaff.Font = new System.Drawing.Font("Noto Sans JP", 14.25F);
            this.chkBoxStaff.Image = global::VRCEntryBoard.Properties.Resources.staff_48;
            this.chkBoxStaff.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.chkBoxStaff.Location = new System.Drawing.Point(1000, 446);
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
            this.lblBeginner.Location = new System.Drawing.Point(208, 19);
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
            this.lblBeginnerNum.Location = new System.Drawing.Point(325, 4);
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
            this.lblStaff.Location = new System.Drawing.Point(401, 19);
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
            this.lblStaffNum.Location = new System.Drawing.Point(543, 4);
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
            this.lblInstance.Location = new System.Drawing.Point(619, 19);
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
            this.lblInstanceNum.Location = new System.Drawing.Point(810, 4);
            this.lblInstanceNum.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblInstanceNum.Name = "lblInstanceNum";
            this.lblInstanceNum.Size = new System.Drawing.Size(65, 54);
            this.lblInstanceNum.TabIndex = 22;
            this.lblInstanceNum.Text = "99";
            // 
            // CEntryView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.lblEntryNum);
            this.Controls.Add(this.lblInstanceNum);
            this.Controls.Add(this.lblInstance);
            this.Controls.Add(this.lblStaffNum);
            this.Controls.Add(this.lblStaff);
            this.Controls.Add(this.lblBeginnerNum);
            this.Controls.Add(this.lblBeginner);
            this.Controls.Add(this.chkBoxStaff);
            this.Controls.Add(this.chkBoxBeginner);
            this.Controls.Add(this.rBtnNone);
            this.Controls.Add(this.rBtnAskMe);
            this.Controls.Add(this.rBtnVisiter);
            this.Controls.Add(this.rBtnEntry);
            this.Controls.Add(this.lblEntry);
            this.Controls.Add(this.btnOutput);
            this.Controls.Add(this.btnListUpdate);
            this.Controls.Add(this.pnlPlayerList);
            this.Name = "CEntryView";
            this.Size = new System.Drawing.Size(1140, 534);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel pnlPlayerList;
        private System.Windows.Forms.Button btnListUpdate;
        private System.Windows.Forms.Button btnOutput;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label lblEntryNum;
        private System.Windows.Forms.Label lblEntry;
        private System.Windows.Forms.RadioButton rBtnEntry;
        private System.Windows.Forms.RadioButton rBtnVisiter;
        private System.Windows.Forms.RadioButton rBtnAskMe;
        private System.Windows.Forms.RadioButton rBtnNone;
        private System.Windows.Forms.CheckBox chkBoxBeginner;
        private System.Windows.Forms.CheckBox chkBoxStaff;
        private System.Windows.Forms.Label lblBeginner;
        private System.Windows.Forms.Label lblBeginnerNum;
        private System.Windows.Forms.Label lblStaff;
        private System.Windows.Forms.Label lblStaffNum;
        private System.Windows.Forms.Label lblInstance;
        private System.Windows.Forms.Label lblInstanceNum;
    }
}
