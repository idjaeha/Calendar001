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
using CalendarWPF.Src;
using CalendarWPF.Model;
using CalendarWPF.Controls;

namespace CalendarWPF
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
        private List<DailyMemo> dayItems; // 현재 보여지고 있는 DailyMemo를 저장한 배열
        private DateTime now;
        private int selectedYear; // 현재 선택된 년
        private int selectedMonth; // 현재 선택된 월
        private static readonly int[] numberOfDays = new int[13] { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        private static readonly int DAY_SPAN = 1;
        private static readonly int DAY_WEEK = 7;
        private Setting currentSetting;
        internal Setting CurrentSetting {
            get 
            {
                if (currentSetting == null) 
                {
                    currentSetting = new Setting();
                }
                return currentSetting;
            }
        }

        public MainWindow()
        {
            InitData();
            InitializeComponent();
            LoadMonth(selectedYear, selectedMonth);
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
            dayItems = new List<DailyMemo>();

            MemoManager.Instance.LoadDataFromFile();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            this.AllowsTransparency = true;
            this.Background = new SolidColorBrush(Colors.Black);
            this.Background.Opacity = 0;
        }

        internal void SetMemosFont()
        {
            foreach(DailyMemo item in dayItems)
            {
                item.SetMemoFont();
            }
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitNotify();

            this.Left = SystemParameters.WorkArea.Width - this.Width;
            this.Top = 0;
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
                    System.Windows.Application.Current.Shutdown();
                    notify.Dispose();
                });

            AddMenuItem(0, "ProgramHiding",
                (object click, EventArgs eClick) =>
                {
                    HideProgram();
                });

            AddMenuItem(0, "Dragging",
                (object click, EventArgs eClick) =>
                {
                    canDrag = !canDrag;
                    this.BorderThickness = canDrag == true ? new Thickness(2) : new Thickness(0);
                    this.Background.Opacity = canDrag == true ? 0.5 : 0;
                    this.ResizeMode = canDrag == true ? ResizeMode.CanResize : ResizeMode.NoResize;
                    ((System.Windows.Forms.MenuItem)click).Text = canDrag == true ? FindResource("noDragging").ToString() : FindResource("Dragging").ToString();
                });

            AddMenuItem(0, "Setting",
                (object click, EventArgs eClick) =>
                {
                    ControlWindow dlg = new ControlWindow(this);
                    dlg.Owner = this;

                    dlg.ShowDialog();
                });
        }

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

        private void Notify_DoubleClick(object sender, EventArgs e)
        {
            // Notify를 더블 클릭했을 경우 발동되는 이벤트
            ShowWindow();
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

        protected void HideProgram()
        {
            // 윈도우를 숨겨줍니다.
            this.Hide();
        }

        private void Window_MouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            //드래그가 가능할 때, 드래그 합니다.
            if (canDrag)
            {
                this.DragMove();
            }
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
        private void LoadDay(int day, int index, string memo)
        {
            DailyMemo newDaysItem = new DailyMemo(this, selectedYear, selectedMonth, day);
            newDaysItem.SetMemo(memo);
            newDaysItem.SetMemoFont();
            dayItems.Add(newDaysItem);
            Grid.SetColumnSpan(newDaysItem, DAY_SPAN);
            Grid.SetRow(newDaysItem, index / DAY_WEEK);
            Grid.SetColumn(newDaysItem, index % DAY_WEEK);
            Calendar_Days.Children.Add(newDaysItem);
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

            countDays += month == 2 && year % 4 == 0 ? 1 : 0;

            for (int day = 1; day <= countDays; day++)
            {
                string memo = "";
                DateTime currentDate = new DateTime(year, month, day);
                if (loadedMemos.ContainsKey(currentDate))
                {
                    memo = loadedMemos[currentDate].Content.ToString();
                }
                LoadDay(day, idx, memo);
                idx++;
            }
        }

        private void CleanCalendar()
        {
            // Calendar에 존재하는 날짜 관련 컨트롤을 삭제합니다.
            foreach (DailyMemo days in dayItems)
            {
                Calendar_Days.Children.Remove(days);
            }
            dayItems.Clear();
        }



        // 해당 개발은 해야할 것들을 표기한 것이고, 순서는 의미가 없다.
        // 개발 1 : 로컬 저장소와 서버와 연결하여 메모 사용
        // 개발 2 : 각종 달력 설정 구현 ( 배경화면 색 변경, 폰트 색 변경 )
        // 개발 3 : 달력의 Topmost 설정을 따로 주고, 배경화면에 얹어있는 식으로 표현하는 것을 구현
        // 개발 4 : 시작 시에 현재 적용된 크기와 배경색, 폰트 등을 불러오는 기능 구현

        // Detail Develop
        // TODO : 메모 편집 중 다른 곳을 눌렀을 때 저장이 되는 기능

    }


}
