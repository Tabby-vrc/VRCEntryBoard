using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCEntryBoard.Domain.Interfaces;

namespace VRCEntryBoard.Infra
{
    internal class CEntryRecorder : IEntryRecorder
    {
        private readonly string _recordFileName = "GAGEntryRecord.csv";

        private List<string> _recordList = new List<string>();

        public CEntryRecorder() { }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            string currentDirectoryPath = Directory.GetCurrentDirectory();
            string recordFilePath = Path.Combine(currentDirectoryPath, this._recordFileName);

            if(!File.Exists(recordFilePath))
            {
                // 参加履歴がない場合は何もしない.
                return;
            }

            // ファイル解析開始.
            using (var fs = new FileStream(recordFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                // テキストエンコーディングにUTF-8を用いてstreamの読み込みを行うStreamReaderを作成する.
                var reader = new StreamReader(fs, Encoding.UTF8);
                while (-1 != reader.Peek())
                {
                    // プレイヤー名を1行ずつ読み取る.
                    string line = reader.ReadLine();
                    if (null == line) break;
                    this._recordList.Add(line);
                }
            }
        }

        public List<string> GetRecotdList()
        {
            return this._recordList;
        }
    }
}
