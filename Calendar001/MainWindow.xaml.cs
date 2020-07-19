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
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon notify;
        private System.Windows.Forms.ContextMenu menu;
        private bool canDrag;
        private List<MenuItemWithID> menuItems; // 사용되고 있는 menuItem을 저장한 배열
        private List<DailyMemo> dailyMemos; // 현재 보여지고 있는 DailyMemo를 저장한 배열
        private DateTime now;
        private int selectedYear; // 현재 선택된 년
        private int selectedMonth; // 현재 선택된 월
        private static readonly int[] numberOfDays = new int[13] { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        private static readonly int DAY_SPAN = 1;
        private static readonly int DAY_WEEK = 7;

        public MainWindow()
        {
            InitData();
            InitializeComponent();
            LoadMonth(selectedYear, selectedMonth);
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
            SelectCulture("ko-KR");
            canDrag = false;

            now = DateTime.Now;

            selectedMonth = now.Month;
            selectedYear = now.Year;

            menuItems = new List<MenuItemWithID>();
            dailyMemos = new List<DailyMemo>();

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

        private void InitNotify()
        {
            menu = new System.Windows.Forms.ContextMenu();
            notify = new NotifyIcon();
            notify.Icon = Properties.Resources.notifyIcon;
            notify.Visible = true;
            notify.ContextMenu = menu;

            notify.DoubleClick += Notify_DoubleClick;

            AddMenuItem(0, "ProgramExit",
                (object click, EventArgs eClick) =>
                {
                    QuitProgram();
                });

            AddMenuItem(0, "ProgramHiding",
                (object click, EventArgs eClick) =>
                {
                    HideProgram();
                });

            AddMenuItem(0, "Dragging",
                (object click, EventArgs eClick) =>
                {
                    ControlDragging(click, eClick);
                });

            AddMenuItem(0, "Setting",
                (object click, EventArgs eClick) =>
                {
                    ShowSetting();
                });
        }
        #endregion

        private void AddMenuItem(int index, string ID, EventHandler clickEvent)
        {
            MenuItemWithID item = new MenuItemWithID();
            menuItems.Add(item);
            menu.MenuItems.Add(item);
            item.Index = index;
            item.Text = FindResource(ID).ToString();
            item.Click += clickEvent;
            item.ID = ID;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitNotify();
        }

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

        /// <summary>
        /// 프로그램을 종료합니다.
        /// </summary>
        private void QuitProgram()
        {
            notify.Dispose();
            System.Windows.Application.Current.Shutdown();
        }

        /// <summary>
        /// Setting 윈도우를 보여줍니다.
        /// </summary>
        private void ShowSetting()
        {
            ControlWindow dlg = new ControlWindow(this);
            dlg.Owner = this;

            dlg.ShowDialog();
        }

        private void ShowWindow()
        {
            // 윈도우를 보여줍니다.
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Visibility = Visibility.Visible;
            this.Topmost = true;
            this.Topmost = false;
        }

        private void HideProgram()
        {
            // 윈도우를 숨겨줍니다.
            this.Hide();
        }

        private void SelectCulture(string culture)
        {
            List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();
            foreach (ResourceDictionary dictionary in System.Windows.Application.Current.Resources.MergedDictionaries)
            {
                dictionaryList.Add(dictionary);
            }
            string requestedCulture = string.Format("/Resources/StringResources.{0}.xaml", culture);
            ResourceDictionary resourceDictionary = dictionaryList.FirstOrDefault(
                d => d.Source.OriginalString == requestedCulture);

            if (resourceDictionary == null)
            {
                requestedCulture = "/Resources/StringResources.ko-kr.xaml";
                resourceDictionary = dictionaryList.FirstOrDefault(
                    d => d.Source.OriginalString == requestedCulture);
            }
            else
            {
                System.Windows.Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                System.Windows.Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }

            //지역화 설정
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
        }

        /// <summary>
        /// 해당하는 날짜를 불러옵니다.
        /// </summary>
        /// <param name="day"></param>
        /// <param name="index"></param>
        /// <param name="memo"></param>
        private DailyMemo LoadDay(int day, int index, string memo)
        {
            DailyMemo newDaysItem = new DailyMemo(this, selectedYear, selectedMonth, day);
            newDaysItem.SetMemo(memo);
            newDaysItem.SetMemoFont();
            dailyMemos.Add(newDaysItem);
            Grid.SetColumnSpan(newDaysItem, DAY_SPAN);
            Grid.SetRow(newDaysItem, index / DAY_WEEK);
            Grid.SetColumn(newDaysItem, index % DAY_WEEK);
            Calendar_Days.Children.Add(newDaysItem);

            return newDaysItem;
        }
        /// <summary>
        /// 인자로 받은 년, 월에 맞게 달을 불러옵니다.
        /// </summary>
        /// <param name="year">the year you want to load</param>
        /// <param name="month">the month you want to load</param>
        private void LoadMonth(int year, int month)
        {
            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            int idx = (int)firstDayOfMonth.DayOfWeek;
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
                DailyMemo newDay = LoadDay(day, idx, memo);

                // 해당 날짜가 오늘이면 하이라이팅한다.
                if (now.Day == day && now.Month == month && now.Year == year)
                {
                    newDay.ShowBorder();
                }
                idx++;
            }
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

        #region EventHandler
        /// <summary>
        /// drag 가능한 state에 따라 값을 조절해줍니다.
        /// </summary>
        /// <param name="click">클릭 당한 대상</param>
        /// <param name="eClick">클릭에 대한 인자</param>
        private void ControlDragging(object click, EventArgs eClick)
        {
            if (canDrag)
            {
                DisableDragging(click, eClick);
            }
            else
            {
                EnableDragging(click, eClick);
            }
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
            ((System.Windows.Forms.MenuItem)click).Text = FindResource("Dragging").ToString();
        }

        private void EnableDragging(object click, EventArgs eClick)
        {
            canDrag = true;
            this.BorderThickness = new Thickness(2);
            this.Background.Opacity = 0.5;
            this.ResizeMode = ResizeMode.CanResize;
            ((System.Windows.Forms.MenuItem)click).Text = FindResource("noDragging").ToString();
        }

        private void Notify_DoubleClick(object sender, EventArgs e)
        {
            // Notify를 더블 클릭했을 경우 발동되는 이벤트
            ShowWindow();
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
            selectedMonth--;
            if (selectedMonth < 1)
            {
                selectedYear--;
                selectedMonth = 12;
            }
            Label_Year.Content = $"{selectedYear}";
            Label_Month.Content = $"{selectedMonth}";
            CleanCalendar();
            LoadMonth(selectedYear, selectedMonth);
            SetAllSetting();
        }

        private void Button_ClickNextMonth(object sender, RoutedEventArgs e)
        {
            selectedMonth++;
            if (selectedMonth > 12)
            {
                selectedYear++;
                selectedMonth = 1;
            }
            Label_Year.Content = $"{selectedYear}";
            Label_Month.Content = $"{selectedMonth}";
            CleanCalendar();
            LoadMonth(selectedYear, selectedMonth);
            SetAllSetting();
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
        #endregion


        // 해당 개발은 해야할 것들을 표기한 것이고, 순서는 의미가 없다.
        // 개발 1 : 달력의 Topmost 설정을 따로 주고, 배경화면에 얹어있는 식으로 표현하는 것을 구현

    }


}
