﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VRCEntryBoard.Domain.Model
{
    internal class Player : INotifyPropertyChanged
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name">プレイヤー名</param>
        public Player(string name)
        {
            Name = name;
            EntryStatus = emEntryStatus.AskMe;
            StaffStatus = false;
            ExpStatus = emExpStatus.None;
            JoinStatus = true;
            GroupNo = 0;
        }

        /// <summary>プロパティ変更イベントハンドラ</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>プレイヤー名</summary>
        public string Name { get; set; }

        private static readonly PropertyChangedEventArgs _entryStatusChangedEventArgs = new PropertyChangedEventArgs(nameof(EntryStatus));
        private emEntryStatus _entryStatus;
        /// <summary>エントリーステータス</summary>
        public emEntryStatus EntryStatus
        {
            get { return _entryStatus; }
            set
            {
                if (value == _entryStatus) return;
                _entryStatus = value;
                PropertyChanged?.Invoke(this, _entryStatusChangedEventArgs);
            }
        }

        private static readonly PropertyChangedEventArgs _staffStatusChangedEventArgs = new PropertyChangedEventArgs(nameof(StaffStatus));
        private bool _staffStatus;
        /// <summary>スタッフステータス</summary>
        public bool StaffStatus
        {
            get { return _staffStatus; }
            set
            {
                if (value == StaffStatus) return;
                _staffStatus = value;
                PropertyChanged?.Invoke(this, _staffStatusChangedEventArgs);
            }
        }

        private static readonly PropertyChangedEventArgs _expStatusChangedEventArgs = new PropertyChangedEventArgs(nameof(ExpStatus));
        //private ExpStatus _expStatus;
        private emExpStatus _expStatus;
        /// <summary>経験ステータス</summary>
        public emExpStatus ExpStatus
        {
            get { return _expStatus; }
            set
            {
                if (value == _expStatus) return;
                if(emExpStatus.None == value) _expStatus = value;
                else _expStatus = value;
                PropertyChanged?.Invoke(this, _expStatusChangedEventArgs);
            }
        }

        private static readonly PropertyChangedEventArgs _joinStatusChangedEventArgs = new PropertyChangedEventArgs(nameof(JoinStatus));
        private bool _joinStatus;
        /// <summary>Joinステータス</summary>
        public bool JoinStatus
        {
            get { return _joinStatus; }
            set
            {
                if (value == _joinStatus) return;
                _joinStatus = value;
                PropertyChanged?.Invoke(this, _joinStatusChangedEventArgs);
            }
        }

        public int GroupNo { get; set; }
    }
}
