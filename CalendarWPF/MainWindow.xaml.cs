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
        private int cnt;

        public MainWindow()
        {
            this.WindowStyle = WindowStyle.None;

            InitializeComponent();
            canDrag = false;
            SelectCulture("ko-KR");
            menuItems = new List<MenuItemWithID>();
            cnt = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
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

            AddMenuItem(0, "ProgramSetting".ToString(),
                (object click, EventArgs eClick) =>
                {
                    ShowWindow();
                });

            AddMenuItem(0, "Dragging".ToString(),
                (object click, EventArgs eClick) =>
                {
                    canDrag = !canDrag;
                    ((System.Windows.Forms.MenuItem)click).Text = canDrag == true ? FindResource("noDragging").ToString() : FindResource("Dragging").ToString();
                });

            AddMenuItem(0, "ChangeKorean".ToString(),
                (object click, EventArgs eClick) =>
                {
                    SelectCulture("ko-KR");
                    RefreshMenuItem();
                });

            AddMenuItem(0, "ChangeEnglish".ToString(),
                (object click, EventArgs eClick) =>
                {
                    SelectCulture("eu-US");
                    RefreshMenuItem();
                });

            this.Left = SystemParameters.WorkArea.Width - this.Width;
            this.Top = 0;
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
            ShowWindow();
        }

        private void ShowWindow()
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Visibility = Visibility.Visible;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            base.OnClosing(e);
        }

        private void Window_MouseLeftDown(object sender, MouseButtonEventArgs e)
        {
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

        private void RefreshMenuItem()
        {
            foreach(MenuItemWithID menuItem in menuItems)
            {
                menuItem.Text = FindResource(menuItem.ID).ToString();
            }
        }

        private class MenuItemWithID : System.Windows.Forms.MenuItem
        {
            public string ID { get; set; }
            public MenuItemWithID() : base() { }
        }

        private void AddDay(object sender, RoutedEventArgs e)
        {
            Days newDaysItem = new Days();

            Grid.SetColumnSpan(newDaysItem, 1);
            Grid.SetRow(newDaysItem, cnt / 7);
            Grid.SetColumn(newDaysItem, cnt % 7);
            cnt++;
            Calendar_Days.Children.Add(newDaysItem);
        }

        // 기능
        // 1. 날짜를 클릭하면 메모를 할 수 있다.
        // 2. 오늘 날짜를 하이라이팅한다,
        // 3. 날짜를 확인할 수 있다.
        // 4. 로컬 저장소에 메모를 저장한다.
        // 5. 배경화면 색을 변경 할 수 있다.


        // 메모 기능
        // --> 더블 클릭을 하면 해당 박스가 켜진다.
        // --> 

    }


}
