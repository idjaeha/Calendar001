using CalendarWPF.Model;
using CalendarWPF.Src;
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

namespace CalendarWPF
{
    /// <summary>
    /// DailyMemo.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DailyMemo : UserControl
    {
        private DateTime date;
        private string memo;
        private MainWindow mainWindow;

        public DailyMemo()
        {
            InitializeComponent();
            ChangeShowMode();
        }

        public DailyMemo(MainWindow mainWindow, int year, int month, int day) : this()
        {
            date = new DateTime(year, month, day);
            TextBlock_Day.Text = day.ToString();
            this.mainWindow = mainWindow;
        }

        public void SetMemo(string text)
        {
            memo = text;
            TextBlock_ShowText.Text = memo;
        }

        public void SetBackground(Brush brush)
        {
            Canvas_ShowText.Background = brush;
        }

        private void doubleClick()
        {
            TextBox_EditText.Text = memo;
            ChangeEditMode();
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                doubleClick();
            }
        }

        private void button_SaveClick(object sender, RoutedEventArgs e)
        {
            SaveText();
            ChangeShowMode();
        }

        private void SaveText()
        {
            memo = TextBox_EditText.Text;
            TextBlock_ShowText.Text = memo;
            MemoManager.Instance.SaveMemo(date, memo);
            ChangeShowMode();
        }

        private void ChangeEditMode()
        {
            Canvas_EditText.Visibility = Visibility.Visible;
            Canvas_ShowText.Visibility = Visibility.Hidden;
            Panel.SetZIndex(TextBox_EditText, 10);
        }

        private void ChangeShowMode()
        {
            Canvas_EditText.Visibility = Visibility.Hidden;
            Canvas_ShowText.Visibility = Visibility.Visible;
        }

        private void TextBox_EditText_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ((TextBox)sender).Focus();
        }

        /// <summary>
        /// 각종 폰트 크기, 스타일, 글씨체 등을 받아와 적용시킵니다.
        /// </summary>
        public void SetMemoFont()
        {
            if(mainWindow.CurrentSetting.FontFamilyName == null)
            {
                return;
            }
            FontFamily newFontFamily = new FontFamily(mainWindow.CurrentSetting.FontFamilyName);
            TextBox_EditText.FontFamily = newFontFamily;
            TextBlock_ShowText.FontFamily = newFontFamily;
            TextBox_EditText.FontSize = double.Parse(mainWindow.CurrentSetting.FontSize);
            TextBlock_ShowText.FontSize = double.Parse(mainWindow.CurrentSetting.FontSize);
            TextBox_EditText.FontStyle = mainWindow.CurrentSetting.FontStyle;
            TextBlock_ShowText.FontStyle = mainWindow.CurrentSetting.FontStyle;
            TextBox_EditText.FontWeight = mainWindow.CurrentSetting.FontWeight;
            TextBlock_ShowText.FontWeight = mainWindow.CurrentSetting.FontWeight;
            TextBox_EditText.TextDecorations = mainWindow.CurrentSetting.TextDecoration;
            TextBlock_ShowText.TextDecorations = mainWindow.CurrentSetting.TextDecoration;
        }

        internal void SetMemoForeground()
        {
            TextBlock_ShowText.Foreground = mainWindow.CurrentSetting.MemoForeground;
        }

        internal void SetOptionForeground()
        {
            TextBlock_Day.Foreground = mainWindow.CurrentSetting.OptionForeground;
        }
    }
}
