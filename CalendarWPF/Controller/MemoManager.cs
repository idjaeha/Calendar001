using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace CalendarWPF.Src
{
    // 캘린더가 사용하는 메모들을 관리하기 위해 존재함
    public class MemoManager
    {
        private static readonly string DATA_FILE_NAME = "memos.dat";
        private static readonly string DATA_FILE_DIRECTORY = "data";
        private Dictionary<DateTime, Memo> memos;

        private static MemoManager memoManager;

        public static MemoManager Instance
        {
            get
            {
                if (memoManager == null)
                {
                    memoManager = new MemoManager();
                }
                return memoManager;
            }
        }

        public MemoManager()
        {
            memos = new Dictionary<DateTime, Memo>();
        }

        public void LoadAllMemos()
        {
            // Data.dat에서 데이터를 불러와 memos에 저장, 이때 중복된 값이 있다면 더 늦게 생성된 값을 memos에 저장
        }

        public void SaveAllMemos()
        {
            // 현재 memos에 저장된 값을 Data.dat에 저장
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

        // 배열에 저장된 데이터를 외부 파일에 저장합니다.
        public void SaveDataToFile()
        {
            string filePath = System.Windows.Forms.Application.StartupPath + $"\\{DATA_FILE_DIRECTORY}";
            if(!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            Stream writeStream = new FileStream(filePath + $"\\{DATA_FILE_NAME}", FileMode.Create);
            BinaryFormatter serializer = new BinaryFormatter();

            serializer.Serialize(writeStream, memos);
            writeStream.Close();
        }

        // 외부 파일에 저장된 데이터를 배열에 불러옵니다.
        public void LoadDataFromFile()
        {
            string filePath = System.Windows.Forms.Application.StartupPath + $"\\{DATA_FILE_DIRECTORY}";
            if (!Directory.Exists(filePath))
            {
                memos = new Dictionary<DateTime, Memo>();
                return;
            }
            Stream readStream = new FileStream(filePath + $"\\{DATA_FILE_NAME}", FileMode.Open);
            BinaryFormatter deserializer = new BinaryFormatter();

            memos = (Dictionary < DateTime, Memo >)deserializer.Deserialize(readStream);
            readStream.Close();
        }
    }
}
