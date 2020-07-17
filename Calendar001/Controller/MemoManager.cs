using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Calendar001.Src
{
    /// <summary>
    /// 캘린더가 사용하는 메모들을 관리합니다.
    /// </summary>
    public class MemoManager
    {
        private static readonly string DATA_FILE_NAME = "memos.dat";
        private static readonly string DATA_FILE_DIRECTORY = "data";
        private Dictionary<DateTime, Memo> memos;

        private static MemoManager instance;

        public static MemoManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MemoManager();
                }
                return instance;
            }
        }

        private MemoManager()
        {
            memos = new Dictionary<DateTime, Memo>();
        }

        public Dictionary<DateTime, Memo> GetMemos(int year, int month)
        {
            // 조건에 해당하는 메모를 배열 형태로 반환함.
            Dictionary<DateTime, Memo> newMemos = new Dictionary<DateTime, Memo>();
            DateTime tempDate = new DateTime(year, month, 1);
            Memo memo;

            for(int idx = 0; idx < 31; idx++)
            {
                if(memos.TryGetValue(tempDate, out memo))
                {
                    newMemos.Add(tempDate, memo);
                }
                tempDate = tempDate.AddDays(1);
            }

            return newMemos;
        }

        public void SaveMemo(DateTime date, string memo)
        {
            // 빈칸일 경우에는 2가지 경우가 존재함
            if (memo.Trim() == "")
            {
                if(!memos.ContainsKey(date)) // 빈칸일 때, 해당 date에 메모가 존재하지 않을 경우
                {
                    return;
                }
            }

            // 메모를 저장하여 memos에 추가함.
            Memo newMemo = new Memo()
            {
                Date = date,
                Content = memo,
                CreatedDate = DateTime.Now
            };

            if(memos.ContainsKey(date)) // 이미 메모가 존재하는 날짜라면, 기존에 있던 것을 삭제한다.
            {
                memos.Remove(date);
            }
            memos.Add(date, newMemo);
            SaveDataToFile();
        }

        /// <summary>
        /// 배열에 저장된 데이터를 외부 파일에 저장합니다. 
        /// </summary>
        public void SaveDataToFile()
        {
            string directoryPath = System.Windows.Forms.Application.StartupPath + $"\\{DATA_FILE_DIRECTORY}";
            string filePath = directoryPath + $"\\{DATA_FILE_NAME}";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            Stream writeStream = new FileStream(filePath, FileMode.Create);
            BinaryFormatter serializer = new BinaryFormatter();

            serializer.Serialize(writeStream, memos);
            writeStream.Close();
        }
        /// <summary>
        /// 외부 파일에 저장된 데이터를 배열에 불러옵니다. 
        /// </summary>
        public void LoadDataFromFile()
        {
            string directoryPath = System.Windows.Forms.Application.StartupPath + $"\\{DATA_FILE_DIRECTORY}";
            string filePath = directoryPath + $"\\{DATA_FILE_NAME}";

            if (!Directory.Exists(directoryPath))
            {
                memos = new Dictionary<DateTime, Memo>();
                return;
            }

            if (!File.Exists(filePath))
            {
                memos = new Dictionary<DateTime, Memo>();
                return;
            }

            Stream readStream = new FileStream(filePath, FileMode.Open);
            BinaryFormatter deserializer = new BinaryFormatter();

            try
            {
                memos = (Dictionary<DateTime, Memo>)deserializer.Deserialize(readStream);
            }
            catch (SerializationException e)
            {
                memos = new Dictionary<DateTime, Memo>();
            }
            readStream.Close();
        }
    }
}
