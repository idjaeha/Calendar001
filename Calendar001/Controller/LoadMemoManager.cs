using Calendar001.Src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar001.Controller
{
    class LoadMemoManager
    {
        private static readonly int[] numberOfDays = new int[13] { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        /// <summary>
        /// 해당하는 날짜를 불러옵니다.
        /// </summary>
        /// <param name="selectedTime"></param>
        /// <param name="day"></param>
        /// <param name="memo"></param>
        private DailyMemo LoadDay(DateTime selectedTime, int day, string memo)
        {
            DailyMemo newDaysItem = new DailyMemo(selectedTime.Year, selectedTime.Month, day);
            newDaysItem.SetMemo(memo);
            newDaysItem.SetMemoFont();

            return newDaysItem;
        }

        /// <summary>
        /// 인자로 받은 년, 월에 맞게 달을 불러옵니다.
        /// </summary>
        /// <param name="selectedTime">the selectedTime you want to load</param>
        public List<DailyMemo> LoadMonth(DateTime selectedTime)
        {
            int year = selectedTime.Year;
            int month = selectedTime.Month;
            DateTime now = DateTime.Now;
            List<DailyMemo> dailyMemos = new List<DailyMemo>();

            
            int countDays = numberOfDays[month];
            Dictionary<DateTime, Memo> loadedMemos = MemoManager.Instance.GetMemos(year, month);

            // 2월의 경우, 윤년이면 1을 더해줍니다.
            countDays += month == 2 && year % 4 == 0 ? 1 : 0;

            for (int day = 1; day <= countDays; day++)
            {
                string memo = "";
                DateTime currentDate = new DateTime(year, month, day);
                if (loadedMemos.ContainsKey(currentDate))
                {
                    memo = loadedMemos[currentDate].Content.ToString();
                }
                DailyMemo newDay = LoadDay(selectedTime, day, memo);
                dailyMemos.Add(newDay);

                // 해당 날짜가 오늘이면 하이라이팅한다.
                if (now.Day == day && now.Month == month && now.Year == year)
                {
                    newDay.ShowBorder();
                }
            }
            return dailyMemos;
        }
    }
}
