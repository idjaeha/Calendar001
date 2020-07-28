using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Diagnostics.Tracing;
using System.ComponentModel;
using System.Threading;
using System.Globalization;
using Calendar001.Src;
using Calendar001.Model;
using Calendar001.Controls;
using Calendar001.Controller;

namespace Calendar001
{
    /// <summary>
    /// Calendar001 프로그램의 Main 창을 관리합니다.
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyMenu notify;
        private List<DailyMemo> dailyMemos; // 현재 보여지고 있는 DailyMemo를 저장한 배열
        private DateTime now;
        private CultureManager cultureManager;
        private LoadMemoManager loadMemoManager;
        private bool canDrag;
        private int selectedYear; // 현재 선택된 년
        private int selectedMonth; // 현재 선택된 월
        private const int DAY_SPAN = 1;
        private const int DAY_WEEK = 7;

        public MainWindow()
        {
            InitData();
            InitializeComponent();
            DrawDays();
            SetAllSetting();
        }

        internal void SetMemosFont()
        {
            foreach (DailyMemo item in dailyMemos)
            {
                item.SetMemoFont();
            }
        }

        /// <summary>
        /// 캘린더 내에 존재하는 지정된 요소들의 Background를 변경합니다.
        /// </summary>
        internal void SetBackground()
        {
            if (SettingManager.CurrentSetting.Background == null)
            {
                return;
            }

            this.Resources["BackgroundColor"] = ConvertHelper.CBrush.ConvertFromString(SettingManager.CurrentSetting.Background);
            foreach (DailyMemo memo in dailyMemos)
            {
                memo.SetBackground(ConvertHelper.CBrush.ConvertFromString(SettingManager.CurrentSetting.Background) as Brush);
            }
        }

        internal void SetMemoForeground()
        {
            if (SettingManager.CurrentSetting.MemoForeground == null)
            {
                return;
            }

            foreach (DailyMemo item in dailyMemos)
            {
                item.SetMemoForeground();
            }
        }

        internal void SetOptionForeground()
        {
            if (SettingManager.CurrentSetting.OptionForeground == null)
            {
                return;
            }

            this.Resources["OptionBrush"] = ConvertHelper.CBrush.ConvertFromString(SettingManager.CurrentSetting.OptionForeground);
            foreach (DailyMemo item in dailyMemos)
            {
                item.SetOptionForeground();
            }
        }

        #region Initialized
        private void InitData()
        {
            cultureManager = new CultureManager();
            cultureManager.SelectCulture("ko-KR");
            canDrag = false;

            now = DateTime.Now;

            selectedMonth = now.Month;
            selectedYear = now.Year;

            notify = new NotifyMenu(this);
            dailyMemos = new List<DailyMemo>();
            loadMemoManager = new LoadMemoManager();

            MemoManager.Instance.LoadDataFromFile();
            SettingManager.LoadCurrentSetting();            
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            this.AllowsTransparency = true;
            this.Background = new SolidColorBrush(Colors.Black);
            this.Background.Opacity = 0;
        }

        private void Label_Year_Initialized(object sender, EventArgs e)
        {
            Label_Year.Content = $"{selectedYear}";
        }

        private void Label_Month_Initialized(object sender, EventArgs e)
        {
            Label_Month.Content = $"{selectedMonth}";
        }

        private void Label_Today_Initialized(object sender, EventArgs e)
        {
            Label_Today.Content = $"{now.Year}. {now.Month}. {now.Day}";
        }


        #endregion

        /// <summary>
        /// Setting 값을 토대로 모든 값을 설정합니다.
        /// </summary>
        private void SetAllSetting()
        {
            SetMemosFont();
            SetMemoForeground();
            SetOptionForeground();
            SetBackground();
            SetTransform();
        }

        /// <summary>
        /// 위치와 크기를 Setting 값을 토대로 설정합니다.
        /// </summary>
        private void SetTransform()
        {
            if (SettingManager.CurrentSetting.Top != -1)
            {
                this.Top = SettingManager.CurrentSetting.Top;
            }

            if (SettingManager.CurrentSetting.Left != -1)
            {
                this.Left = SettingManager.CurrentSetting.Left;
            }

            if (SettingManager.CurrentSetting.Width != -1)
            {
                this.Width = SettingManager.CurrentSetting.Width;
            }

            if (SettingManager.CurrentSetting.Height != -1)
            {
                this.Height = SettingManager.CurrentSetting.Height;
            }

            // 오른쪽 상단에 해당 윈도우를 붙입니다.
            if (SettingManager.CurrentSetting.Left == -1)
            {
                this.Left = SystemParameters.WorkArea.Width - this.Width;
            }
            if (SettingManager.CurrentSetting.Top == -1)
            {
                this.Top = 0;
            }
        }

        internal void QuitProgram()
        {
            notify.Notify.Dispose();
            System.Windows.Application.Current.Shutdown();
        }

        internal void ShowSetting()
        {
            ControlWindow dlg = new ControlWindow(this);
            dlg.Owner = this;

            dlg.ShowDialog();
        }

        internal void ShowWindow()
        {
            // 윈도우를 보여줍니다.
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Visibility = Visibility.Visible;
            this.Topmost = true;
            this.Topmost = false;
        }

        internal void HideProgram()
        {
            // 윈도우를 숨겨줍니다.
            this.Hide();
        }

        private void CleanCalendar()
        {
            // Calendar에 존재하는 날짜 관련 컨트롤을 삭제합니다.
            foreach (DailyMemo days in dailyMemos)
            {
                Calendar_Days.Children.Remove(days);
            }
            dailyMemos.Clear();
        }

        private void MoveMonth(int idx)
        {
            selectedMonth += idx;
            if (selectedMonth > 12)
            {
                selectedYear++;
                selectedMonth = 1;
            }
            if (selectedMonth < 1)
            {
                selectedYear--;
                selectedMonth = 12;
            }
            Label_Year.Content = $"{selectedYear}";
            Label_Month.Content = $"{selectedMonth}";
            CleanCalendar();
            // DEL : LoadMonth(selectedYear, selectedMonth);
            DrawDays();
            SetAllSetting();
        }

        // 메모를 불러와 배열에 저장하여 그립니다.
        private void DrawDays()
        {
            DateTime firstDayOfMonth = new DateTime(selectedYear, selectedMonth, 1);
            int idx = (int)firstDayOfMonth.DayOfWeek;

            dailyMemos = loadMemoManager.LoadMonth(new DateTime(selectedYear, selectedMonth, 1));
            for (int day = 0; day < dailyMemos.Count; day++)
            {
                Grid.SetColumnSpan(dailyMemos[day], DAY_SPAN);
                Grid.SetRow(dailyMemos[day], idx / DAY_WEEK);
                Grid.SetColumn(dailyMemos[day], idx % DAY_WEEK);
                Calendar_Days.Children.Add(dailyMemos[day]);
                idx++;
            }
        }

        #region EventHandler
        /// <summary>
        /// drag 가능한 state에 따라 값을 조절해줍니다.
        /// </summary>
        /// <param name="click">클릭 당한 대상</param>
        /// <param name="eClick">클릭에 대한 인자</param>
        public void ControlDragging(object click, EventArgs eClick)
        {
            if (canDrag)
            {
                DisableDragging(click, eClick);
            }
            else
            {
                EnableDragging(click, eClick);
            }
            notify.ChangeDraggingText(canDrag);
        }

        /// <summary>
        /// 드래그를 금지합니다.
        /// </summary>
        /// <param name="click">클릭 당한 대상</param>
        /// <param name="eClick">클릭 이벤트 발생 시에 생기는 인자</param>
        private void DisableDragging(object click, EventArgs eClick)
        {
            canDrag = false;
            this.BorderThickness = new Thickness(0);
            this.Background.Opacity = 0;
            this.ResizeMode = ResizeMode.NoResize;
        }

        private void EnableDragging(object click, EventArgs eClick)
        {
            canDrag = true;
            this.BorderThickness = new Thickness(2);
            this.Background.Opacity = 0.5;
            this.ResizeMode = ResizeMode.CanResize;
        }

        private void Window_MouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            //드래그가 가능할 때, 드래그 합니다.
            if (canDrag)
            {
                this.DragMove();
            }
        }

        private void Button_ClickPrevMonth(object sender, RoutedEventArgs e)
        {
            MoveMonth(-1);
        }

        private void Button_ClickNextMonth(object sender, RoutedEventArgs e)
        {
            MoveMonth(1);
        }

        private void Window_Main_Closed(object sender, EventArgs e)
        {
            SettingManager.SaveCurrentSetting();
        }

        private void Window_Main_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SettingManager.CurrentSetting.Width = this.ActualWidth;
            SettingManager.CurrentSetting.Height = this.ActualHeight;
        }

        private void Window_Main_LocationChanged(object sender, EventArgs e)
        {
            SettingManager.CurrentSetting.Top = this.Top;
            SettingManager.CurrentSetting.Left = this.Left;
        }

        private void Button_Setting_Click(object sender, RoutedEventArgs e)
        {
            ShowSetting();
        }

        private void Button_Drag_Click(object sender, RoutedEventArgs e)
        {
            ControlDragging(sender, e);
        }
        #endregion


        // 해당 개발은 해야할 것들을 표기한 것이고, 순서는 의미가 없다.
        // 개발 1 : 달력의 Topmost 설정을 따로 주고, 배경화면에 얹어있는 식으로 표현하는 것을 구현

    }


}
