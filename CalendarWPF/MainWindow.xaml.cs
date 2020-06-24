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
        private List<MenuItemWithID> menuItems;
        private List<DailyMemo> dayItems;
        private int cnt;
        private DateTime now;
        private int selectedYear;
        private int selectedMonth;
        private int[] numberOfDays;

        public MainWindow()
        {
            InitializeComponent();
            InitWindow();
            LoadMonth(selectedYear, selectedMonth);
        }

        private void InitWindow()
        {
            canDrag = false;
            SelectCulture("ko-KR");
            menuItems = new List<MenuItemWithID>();
            dayItems = new List<DailyMemo>();
            this.AllowsTransparency = true;
            cnt = 0;
            now = DateTime.Now;
            selectedMonth = now.Month;
            selectedYear = now.Year;
            numberOfDays = new int[13] {0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            Label_Year.Content = $"{selectedYear}";
            Label_Month.Content = $"{selectedMonth}";
            Label_Today.Content = $"{now.Year}. {now.Month}. {now.Day}";
        }

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
            notify.Icon = Properties.Resources.sampleIcon;
            notify.Visible = true;
            notify.ContextMenu = menu;

            notify.DoubleClick += Notify_DoubleClick;

            AddMenuItem(0, "ProgramExit".ToString(),
                (object click, EventArgs eClick) =>
                {
                    System.Windows.Application.Current.Shutdown();
                    notify.Dispose();
                });

            AddMenuItem(0, "ProgramHiding".ToString(),
                (object click, EventArgs eClick) =>
                {
                    HideProgram();
                });

            AddMenuItem(0, "Dragging".ToString(),
                (object click, EventArgs eClick) =>
                {
                    canDrag = !canDrag;
                    ((System.Windows.Forms.MenuItem)click).Text = canDrag == true ? FindResource("noDragging").ToString() : FindResource("Dragging").ToString();
                });

            // TODO : 언어 설정 코드
            //AddMenuItem(0, "ChangeKorean".ToString(),
            //    (object click, EventArgs eClick) =>
            //    {
            //        SelectCulture("ko-KR");
            //        RefreshMenuItem();
            //    });

            //AddMenuItem(0, "ChangeEnglish".ToString(),
            //    (object click, EventArgs eClick) =>
            //    {
            //        SelectCulture("eu-US");
            //        RefreshMenuItem();
            //    });
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
        }

        protected void HideProgram()
        {
            // 윈도우를 숨겨줍니다.
            this.Hide();
        }

        private void Window_MouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            if (canDrag)
            {
                //드래그가 가능할 때, 드래그 합니다.
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

        // 언어 설정 관련 코드
        //private void RefreshMenuItem()
        //{
        //    foreach (MenuItemWithID menuItem in menuItems)
        //    {
        //        menuItem.Text = FindResource(menuItem.ID).ToString();
        //    }
        //}

        private class MenuItemWithID : System.Windows.Forms.MenuItem
        {
            public string ID { get; set; }
            public MenuItemWithID() : base() { }
        }

        private void LoadDay(int day)
        {
            DailyMemo newDaysItem = new DailyMemo(selectedYear, selectedMonth, day);
            dayItems.Add(newDaysItem);
            Grid.SetColumnSpan(newDaysItem, 1);
            Grid.SetRow(newDaysItem, cnt / 7);
            Grid.SetColumn(newDaysItem, cnt % 7);
            cnt++;
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

        private void LoadMonth(int year, int month)
        {
            DateTime firstDayOfMonth = new DateTime(selectedYear, selectedMonth, 1);
            cnt = (int)firstDayOfMonth.DayOfWeek;
            int countDays = numberOfDays[selectedMonth];
            countDays += selectedMonth == 2 && selectedYear % 4 == 0 ? 1 : 0;
            for(int day = 1; day <= countDays; day++)
            {
                LoadDay(day);
            }
        }

        private void CleanCalendar()
        {
            foreach(DailyMemo days in dayItems)
            {
                Calendar_Days.Children.Remove(days);
            }
            dayItems.Clear();
        }

        // 해당 개발은 해야할 것들을 표기한 것이고, 순서는 의미가 없다.
        // 개발 1 : 로컬 저장소에 메모를 저장, 불러오기
        // 개발 2 : 로컬 저장소와 서버와 연결하여 메모 사용
        // 개발 3 : 각종 달력 설정 구현 ( 배경화면 색 변경, 폰트 색 변경 )
        // 개발 4 : 달력 사이즈 변경
        // 개발 5 : 달력의 Topmost 설정을 따로 주고, 배경화면에 얹어있는 식으로 표현하는 것을 구현

        // Detail Develop
        // TODO : 메모 편집 중 다른 곳을 눌렀을 때 저장이 되는 기능
        // TODO : 달력의 크기가 변경될 때마다 자동으로 높이와 너비가 조절되는 기능

    }


}
