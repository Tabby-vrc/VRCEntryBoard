using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Windows.Forms;

using VRCEntryBoard.App.Controller;
using VRCEntryBoard.Domain.Model;
using VRCEntryBoard.HMI.CustomUserControl;
using Microsoft.AspNetCore.Mvc;

namespace VRCEntryBoard.HMI
{
    internal partial class CEntryView : UserControl
    {
        private CEntryViewController _controller;
        private PlayerPanel _lastSelectPlayerPanel;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="controller">コントローラ部</param>
        public CEntryView(CEntryViewController controller)
        {
            InitializeComponent();

            _controller = controller;
            _controller.SetView(this);

            UpdateEntryNum(new EntryNumDto());

            this.pnlPlayerList.GetType().InvokeMember(
            "DoubleBuffered",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                    null,
                    pnlPlayerList,
                    new object[] { true });

            // 参加ボタン描画
            var BtnClrPair = new[]
            {
                new { Btn = this.rBtnEntry,     Color = Color.LawnGreen},
                new { Btn = this.rBtnVisiter,   Color = Color.Gray },
                new { Btn = this.rBtnAskMe,     Color = Color.DarkOrange }
            };
            foreach (var kvp in BtnClrPair)
            {
                Bitmap bitmap = new Bitmap(48, 48, PixelFormat.Format32bppPArgb);
                Graphics graphics = Graphics.FromImage(bitmap);
                Brush brush = new SolidBrush(kvp.Color);
                graphics.FillEllipse(brush, 8, 8, 32, 32);
                kvp.Btn.Image = bitmap;
            }
        }

        /// <summary>
        /// プレイヤーリストの更新
        /// </summary>
        /// <param name="playerList">プレイヤーリスト</param>
        public void UpdatePlayerList(List<Player> playerList)
        {
            this.pnlPlayerList.SuspendLayout();
            this.pnlPlayerList.Controls.Clear();

            foreach (var player in playerList)
            {
                PlayerPanel playerPanel = new PlayerPanel
                {
                    Margin = new Padding(top: 10, bottom: 5, left: 10, right: 10)
                };
                playerPanel.PlayerName = player.Name;
                playerPanel.EntryStatusText = player.EntryStatus;
                playerPanel.ExpIconVisible = player.ExpStatus;
                playerPanel.StaffIconVisible = player.StaffStatus;
                playerPanel.ErrorIconVisible = !player.JoinStatus;

                // プロパティ変更の登録.
                player.PropertyChanged += (sender, arg) =>
                {
                    if (nameof(player.EntryStatus) == arg.PropertyName)
                    {
                        playerPanel.EntryStatusText = player.EntryStatus;
                    }
                    if (nameof(player.ExpStatus) == arg.PropertyName)
                    {
                        playerPanel.ExpIconVisible = player.ExpStatus;
                    }
                    if (nameof(player.StaffStatus) == arg.PropertyName)
                    {
                        playerPanel.StaffIconVisible = player.StaffStatus;
                    }
                    if (nameof(player.JoinStatus) == arg.PropertyName)
                    {
                        playerPanel.ErrorIconVisible = !player.JoinStatus;
                    }
                };

                playerPanel.SetMouseDownEventHandler(this.PlayerPanel_Click);
                this.pnlPlayerList.Controls.Add(playerPanel);
            }
            // 選択UI、選択情報のリセット.
            ReSetStatusUI();
            _lastSelectPlayerPanel = null;

            this.pnlPlayerList.ResumeLayout();
        }

        /// <summary>
        /// 参加者数アップデート
        /// </summary>
        /// <param name="entryNumDto">参加者数データ</param>
        public void UpdateEntryNum(EntryNumDto entryNumDto)
        {
            this.lblReg1Num.Text = entryNumDto.Reg1Num.ToString();
            this.lblReg2Num.Text = entryNumDto.Reg2Num.ToString();
            this.lblBeginnerNum.Text = entryNumDto.BeginnerNum.ToString();
            this.lblStaffNum.Text = entryNumDto.StaffNum.ToString();
            this.lblInstanceNum.Text = entryNumDto.InstanceNum.ToString();
        }

        /// <summary>
        /// プレイヤーパネルクリック処理
        /// </summary>
        private void PlayerPanel_Click(object sender, MouseEventArgs e)
        {
            var panel = sender as PlayerPanel;

            if (panel == _lastSelectPlayerPanel) return;

            panel.PanelSelect(true);
            if(null != _lastSelectPlayerPanel)
            {
                _lastSelectPlayerPanel.PanelSelect(false);
            }
            _lastSelectPlayerPanel = panel;

            SetStatusUI(panel.PlayerName);
        }

        /// <summary>
        /// 更新ボタンクリック処理.
        /// </summary>
        private async void btnListUpdate_Click(object sender, EventArgs e)
        {
            // ボタンを一時的に無効化して連打防止
            this.btnListUpdate.Enabled = false;
            // 非同期メソッドを呼び出す
            await this._controller.UpdatePlayerList();
            // 処理完了後、ボタンを再有効化
            this.btnListUpdate.Enabled = true;
        }

        /// <summary>
        /// 出力ボタンクリック処理.
        /// </summary>
        private void btnOutput_Click(object sender, EventArgs e)
        {
            _controller.GroupingPlayerList();
            _controller.OutputPlayerList();
        }

        /// <summary>
        /// ステータスラジオボタン選択
        /// </summary>
        public void rBtnSelect(emEntryStatus status)
        {
            if(emEntryStatus.Entry == status)
            {
                rBtnEntry.Checked = true;
            }
            else if(emEntryStatus.Visiter == status)
            {
                rBtnVisiter.Checked = true;
            }
            else if(emEntryStatus.AskMe == status)
            {
                rBtnAskMe.Checked = true;
            }
            else
            {
                rBtnReset();
            }
        }
        /// <summary>
        /// ステータスラジオボタンリセット
        /// </summary>
        public void rBtnReset()
        {
            rBtnNone.Checked = true;
        }

        private void rBtnEntry_Click(object sender, EventArgs e)
        {
            if (null == _lastSelectPlayerPanel) return;
            _controller.UpdateEntryStatus(_lastSelectPlayerPanel.PlayerName, emEntryStatus.Entry);
        }
        private void rBtnVisiter_Click(object sender, EventArgs e)
        {
            if (null == _lastSelectPlayerPanel) return;
            _controller.UpdateEntryStatus(_lastSelectPlayerPanel.PlayerName, emEntryStatus.Visiter);
        }
        private void rBtnAskMe_Click(object sender, EventArgs e)
        {
            if (null == _lastSelectPlayerPanel) return;
            _controller.UpdateEntryStatus(_lastSelectPlayerPanel.PlayerName, emEntryStatus.AskMe);
        }

        private void rBtnReg1_Click(object sender, EventArgs e)
        {
            if (null == _lastSelectPlayerPanel) return;
            _controller.UpdateRegulationStatus(_lastSelectPlayerPanel.PlayerName, 1);
        }
        private void rBtnReg2_Click(object sender, EventArgs e)
        {
            if (null == _lastSelectPlayerPanel) return;
            _controller.UpdateRegulationStatus(_lastSelectPlayerPanel.PlayerName, 2);
        }

        /// <summary>
        /// 初心者ステータス更新
        /// </summary>
        private void rBtnBeginner_Click(object sender, EventArgs e)
        {
            if (null == _lastSelectPlayerPanel) return;
            _controller.UpdateBeginnerStatus(_lastSelectPlayerPanel.PlayerName, (sender as CheckBox).Checked);
        }

        /// <summary>
        /// スタッフステータス更新
        /// </summary>
        private void chkBoxStaff_Click(object sender, EventArgs e)
        {
            if (null == _lastSelectPlayerPanel) return;
            _controller.UpdateStaff(_lastSelectPlayerPanel.PlayerName, (sender as CheckBox).Checked);
        }

        /// <summary>
        /// ステータスUIセット
        /// </summary>
        private void SetStatusUI(string playerName)
        {
            emEntryStatus entryStatus  = _controller.GetEntryStatus(playerName);
            int regStatus              = _controller.GetRegulationStatus(playerName);
            emExpStatus expStatus      = _controller.GetExpStatus(playerName);
            bool isStarff              = _controller.GetStaff(playerName);

            // エントリーボタンセット.
            if (emEntryStatus.Entry == entryStatus)
            {
                rBtnEntry.Checked = true;
            }
            else if (emEntryStatus.Visiter == entryStatus)
            {
                rBtnVisiter.Checked = true;
            }
            else if (emEntryStatus.AskMe == entryStatus)
            {
                rBtnAskMe.Checked = true;
            }
            else
            {
                rBtnNone.Checked = true;
            }

            // レギュレーションボタンセット.
            if(regStatus == 1)
            {
                rBtnReg1.Checked = true;
            }
            else if(regStatus == 2)
            {
                rBtnReg2.Checked = true;
            }

            // 初心者ボタンセット.
            rBtnBeginner.Checked = (expStatus & emExpStatus.Beginner) == emExpStatus.Beginner;

            // スタッフボタンセット.
            chkBoxStaff.Checked = isStarff;
        }

        /// <summary>
        /// ステータスUIリセット
        /// </summary>
        private void ReSetStatusUI()
        {
            rBtnNone.Checked = true;
            rBtnReg1.Checked = true;
            chkBoxStaff.Checked = false;
        }
    }
}
