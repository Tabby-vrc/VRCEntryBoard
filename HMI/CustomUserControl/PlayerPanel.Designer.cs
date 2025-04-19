namespace VRCEntryBoard.HMI.CustomUserControl
{
    partial class PlayerPanel
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
            this.lblPlayerName = new System.Windows.Forms.Label();
            this.lblEntryStatus = new System.Windows.Forms.Label();
            this.picReg1Icon = new System.Windows.Forms.PictureBox();
            this.picReg2Icon = new System.Windows.Forms.PictureBox();
            this.picBeginnerIcon = new System.Windows.Forms.PictureBox();
            this.picNewUserIcon = new System.Windows.Forms.PictureBox();
            this.picErrorIcon = new System.Windows.Forms.PictureBox();
            this.picStaffIcon = new System.Windows.Forms.PictureBox();
            this.picEntryIcon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picReg1Icon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picReg2Icon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBeginnerIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNewUserIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picErrorIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picStaffIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEntryIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // lblPlayerName
            // 
            this.lblPlayerName.Font = new System.Drawing.Font("Noto Sans JP", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblPlayerName.Location = new System.Drawing.Point(6, 0);
            this.lblPlayerName.Margin = new System.Windows.Forms.Padding(8, 0, 4, 0);
            this.lblPlayerName.Name = "lblPlayerName";
            this.lblPlayerName.Size = new System.Drawing.Size(211, 30);
            this.lblPlayerName.TabIndex = 0;
            this.lblPlayerName.Text = "プレイヤー名";
            this.lblPlayerName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblEntryStatus
            // 
            this.lblEntryStatus.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblEntryStatus.Font = new System.Drawing.Font("Noto Sans JP", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblEntryStatus.Location = new System.Drawing.Point(41, 30);
            this.lblEntryStatus.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.lblEntryStatus.Name = "lblEntryStatus";
            this.lblEntryStatus.Size = new System.Drawing.Size(69, 28);
            this.lblEntryStatus.TabIndex = 1;
            this.lblEntryStatus.Text = "未確認";
            this.lblEntryStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // picReg1Icon
            // 
            this.picReg1Icon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.picReg1Icon.Image = global::VRCEntryBoard.Properties.Resources.Reg1_48;
            this.picReg1Icon.Location = new System.Drawing.Point(187, 26);
            this.picReg1Icon.Margin = new System.Windows.Forms.Padding(3, 3, 6, 8);
            this.picReg1Icon.Name = "picReg1Icon";
            this.picReg1Icon.Size = new System.Drawing.Size(30, 30);
            this.picReg1Icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picReg1Icon.TabIndex = 8;
            this.picReg1Icon.TabStop = false;
            // 
            // picReg2Icon
            // 
            this.picReg2Icon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.picReg2Icon.Image = global::VRCEntryBoard.Properties.Resources.Reg2_48;
            this.picReg2Icon.Location = new System.Drawing.Point(187, 26);
            this.picReg2Icon.Margin = new System.Windows.Forms.Padding(3, 3, 6, 8);
            this.picReg2Icon.Name = "picReg2Icon";
            this.picReg2Icon.Size = new System.Drawing.Size(30, 30);
            this.picReg2Icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picReg2Icon.TabIndex = 7;
            this.picReg2Icon.TabStop = false;
            // 
            // picBeginnerIcon
            // 
            this.picBeginnerIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.picBeginnerIcon.Image = global::VRCEntryBoard.Properties.Resources.newuser_48;
            this.picBeginnerIcon.Location = new System.Drawing.Point(187, 26);
            this.picBeginnerIcon.Margin = new System.Windows.Forms.Padding(3, 3, 6, 8);
            this.picBeginnerIcon.Name = "picBeginnerIcon";
            this.picBeginnerIcon.Size = new System.Drawing.Size(30, 30);
            this.picBeginnerIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBeginnerIcon.TabIndex = 6;
            this.picBeginnerIcon.TabStop = false;
            // 
            // picNewUserIcon
            // 
            this.picNewUserIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.picNewUserIcon.Image = global::VRCEntryBoard.Properties.Resources.NEW_48;
            this.picNewUserIcon.Location = new System.Drawing.Point(187, 26);
            this.picNewUserIcon.Margin = new System.Windows.Forms.Padding(3, 3, 6, 8);
            this.picNewUserIcon.Name = "picNewUserIcon";
            this.picNewUserIcon.Size = new System.Drawing.Size(30, 30);
            this.picNewUserIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picNewUserIcon.TabIndex = 3;
            this.picNewUserIcon.TabStop = false;
            // 
            // picErrorIcon
            // 
            this.picErrorIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.picErrorIcon.Image = global::VRCEntryBoard.Properties.Resources.Error_48;
            this.picErrorIcon.Location = new System.Drawing.Point(115, 26);
            this.picErrorIcon.Margin = new System.Windows.Forms.Padding(3, 3, 3, 8);
            this.picErrorIcon.Name = "picErrorIcon";
            this.picErrorIcon.Size = new System.Drawing.Size(30, 30);
            this.picErrorIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picErrorIcon.TabIndex = 5;
            this.picErrorIcon.TabStop = false;
            // 
            // picStaffIcon
            // 
            this.picStaffIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.picStaffIcon.Image = global::VRCEntryBoard.Properties.Resources.staff_48;
            this.picStaffIcon.Location = new System.Drawing.Point(151, 26);
            this.picStaffIcon.Margin = new System.Windows.Forms.Padding(3, 3, 3, 8);
            this.picStaffIcon.Name = "picStaffIcon";
            this.picStaffIcon.Size = new System.Drawing.Size(30, 30);
            this.picStaffIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picStaffIcon.TabIndex = 4;
            this.picStaffIcon.TabStop = false;
            // 
            // picEntryIcon
            // 
            this.picEntryIcon.Location = new System.Drawing.Point(11, 30);
            this.picEntryIcon.Name = "picEntryIcon";
            this.picEntryIcon.Size = new System.Drawing.Size(30, 30);
            this.picEntryIcon.TabIndex = 2;
            this.picEntryIcon.TabStop = false;
            // 
            // PlayerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.picBeginnerIcon);
            this.Controls.Add(this.picReg1Icon);
            this.Controls.Add(this.picReg2Icon);
            this.Controls.Add(this.picNewUserIcon);
            this.Controls.Add(this.picErrorIcon);
            this.Controls.Add(this.picStaffIcon);
            this.Controls.Add(this.picEntryIcon);
            this.Controls.Add(this.lblEntryStatus);
            this.Controls.Add(this.lblPlayerName);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "PlayerPanel";
            this.Size = new System.Drawing.Size(221, 60);
            ((System.ComponentModel.ISupportInitialize)(this.picReg1Icon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picReg2Icon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBeginnerIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNewUserIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picErrorIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picStaffIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEntryIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblEntryStatus;
        internal System.Windows.Forms.Label lblPlayerName;
        private System.Windows.Forms.PictureBox picEntryIcon;
        private System.Windows.Forms.PictureBox picStaffIcon;
        private System.Windows.Forms.PictureBox picNewUserIcon;
        private System.Windows.Forms.PictureBox picErrorIcon;
        private System.Windows.Forms.PictureBox picBeginnerIcon;
        private System.Windows.Forms.PictureBox picReg2Icon;
        private System.Windows.Forms.PictureBox picReg1Icon;
    }
}
