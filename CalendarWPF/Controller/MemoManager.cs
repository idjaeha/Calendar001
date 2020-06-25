using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarWPF.Src
{
    public class MemoManager
    {
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

        public void AddMemos(DateTime date, string memo)
        {
            // 빈칸이면 저장하지 않음
            if (memo.Trim() == "")
            {
                return;
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
        }
    }
}
