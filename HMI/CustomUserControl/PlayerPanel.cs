using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using VRCEntryBoard.Domain.Model;

namespace VRCEntryBoard.HMI.CustomUserControl
{
    /// <summary>
    /// プレイヤー情報パネルコントロール.
    /// </summary>
    internal partial class PlayerPanel : UserControl
    {
        private MouseEventHandler _mouseEventHandler;

        public PlayerPanel()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// プレイヤー名（プロパティ）
        /// </summary>
        public string PlayerName
        {
            get
            {
                return this.lblPlayerName.Text;
            }
            set
            {
                this.lblPlayerName.Text = value;
            }
        }
        /// <summary>
        /// エントリーステータス（プロパティ）
        /// </summary>
        public emEntryStatus EntryStatusText
        {
            //get {  return emEntryStatus.Unknown; }
            set
            {
                lblEntryStatus.Text = value.ToStringEx();

                // Statusに応じた着色イベント.
                // AskMe　：橙.
                // Entry　：緑.
                // Visitor：灰.
                if (emEntryStatus.AskMe == value)
                {
                    PanelColor = Color.DarkOrange;
                }
                else if (emEntryStatus.Entry == value)
                {
                    PanelColor = Color.LawnGreen;
                }
                else if (emEntryStatus.Visiter == value)
                {
                    PanelColor = Color.Gray;
                }
            }
        }
        /// <summary>
        /// レギュレーションアイコン表示（プロパティ）
        /// </summary>
        public int RegulationIconVisible
        {
            // get { return 0; }
            set
            {
                this.picReg1Icon.Visible = value == 1;
                this.picReg2Icon.Visible = value == 2;
            }
        }
        /// <summary>
        /// 経験アイコン表示（プロパティ）
        /// </summary>
        public emExpStatus ExpIconVisible
        {
            //get { return ExpStatus.None; }
            set
            {
                picNewUserIcon.Visible = value.HasFlag(emExpStatus.NewUser);
                picBeginnerIcon.Visible = value.HasFlag(emExpStatus.Beginner);
            }
        }
        /// <summary>
        /// スタッフアイコン表示
        /// </summary>
        public bool StaffIconVisible
        {
            get
            {
                return this.picStaffIcon.Visible;
            }
            set
            {
                this.picStaffIcon.Visible = value;
            }
        }
        /// <summary>
        /// エラーアイコン表示（プロパティ）
        /// </summary>
        public bool ErrorIconVisible
        {
            get
            {
                return this.picErrorIcon.Visible;
            }
            set
            {
                this.picErrorIcon.Visible = value;
            }
        }
        /// <summary>
        /// パネルカラー（プロパティ）
        /// </summary>
        public Color PanelColor
        {
            get
            {
                return this.BackColor;
            }
            set
            {                
                // 新しいビットマップを作成.
                Bitmap bitmap = new Bitmap(this.picEntryIcon.Width, this.picEntryIcon.Height, PixelFormat.Format32bppPArgb);
                Graphics graphics = Graphics.FromImage(bitmap);

                // 塗りつぶされた円を描画.
                Brush brush = new SolidBrush(value);
                graphics.FillEllipse(brush, 0, 0, 24, 24);
                this.picEntryIcon.Image = bitmap;
            }
        }
        public void PanelSelect(bool isSelect)
        {
            Color color = Color.White;
            if(isSelect)
            {
                color = SystemColors.GradientActiveCaption;
            }

            this.BackColor = color;
            this.lblPlayerName.BackColor = color;
            this.lblEntryStatus.BackColor = color;
        }

        public void SetMouseDownEventHandler(MouseEventHandler eventHandler)
        {
            this.MouseDown += eventHandler;
            this._mouseEventHandler = eventHandler;
            this.lblPlayerName.MouseDown += ChildMouseDownEvent;
            this.lblEntryStatus.MouseDown += ChildMouseDownEvent;
            this.picEntryIcon.MouseDown += ChildMouseDownEvent;
            this.picNewUserIcon.MouseDown += ChildMouseDownEvent;
            this.picStaffIcon.MouseDown += ChildMouseDownEvent;
            this.picErrorIcon.MouseDown += ChildMouseDownEvent;
        }

        private void ChildMouseDownEvent(object sender, MouseEventArgs e)
        {
            this._mouseEventHandler(this, e);
        }
    }
}
