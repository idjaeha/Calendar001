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

        public DailyMemo()
        {
            InitializeComponent();
            ChangeShowMode();
        }

        public DailyMemo(int year, int month, int day) : this()
        {
            date = new DateTime(year, month, day);
            TextBlock_day.Text = day.ToString();
        }

        public void SetMemo(string text)
        {
            memo = text;
            TextBlock_ShowText.Text = memo;
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

        private void Canvas_MouseEnterMemo(object sender, MouseEventArgs e)
        {
            ((Canvas)sender).Background.Opacity = 0.5;
        }

        private void Canvas_MouseLeaveMemo(object sender, MouseEventArgs e)
        {
            ((Canvas)sender).Background.Opacity = 1;
        }

        private void TextBox_EditText_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ((TextBox)sender).Focus();
        }

        public void SetMemoFont(FontInformation fontInformation)
        {
            if(fontInformation == null)
            {
                return;
            }
            FontFamily newFontFamily = new FontFamily(fontInformation.FontFamilyName);
            TextBox_EditText.FontFamily = newFontFamily;
            TextBlock_ShowText.FontFamily = newFontFamily;
            TextBox_EditText.FontSize = double.Parse(fontInformation.FontSize);
            TextBlock_ShowText.FontSize = double.Parse(fontInformation.FontSize);
        }
    }
}
