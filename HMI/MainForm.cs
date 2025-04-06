using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VRCEntryBoard.HMI
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainForm(Action<MainForm> initializer)
        {
            InitializeComponent();
            
            // DIコンテナから渡されたイニシャライザアクションを実行
            initializer(this);
        }
    }
}
