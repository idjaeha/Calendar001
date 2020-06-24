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
    /// Days.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Days : UserControl
    {
        public Days()
        {
            InitializeComponent();
            ChangeShowMode();
        }

        private void doubleClick()
        {
            textBox_EditText.Text = textBlock_ShowText.Text;
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
            textBlock_ShowText.Text = textBox_EditText.Text;
            ChangeShowMode();
        }

        private void ChangeEditMode()
        {
            canvas_EditText.Visibility = Visibility.Visible;
            canvas_ShowText.Visibility = Visibility.Hidden;
        }

        private void ChangeShowMode()
        {
            canvas_EditText.Visibility = Visibility.Hidden;
            canvas_ShowText.Visibility = Visibility.Visible;
        }

    }
}
