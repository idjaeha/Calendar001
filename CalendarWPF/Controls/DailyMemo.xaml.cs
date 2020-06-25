﻿using CalendarWPF.Src;
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
            MemoManager.Instance.AddMemos(date, memo);
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
            ((Canvas)sender).Background = new SolidColorBrush(Colors.AliceBlue);
            ((Canvas)sender).Background.Opacity = 0.2;
        }

        private void Canvas_MouseLeaveMemo(object sender, MouseEventArgs e)
        {
            ((Canvas)sender).Background = new SolidColorBrush(Colors.Black);
            ((Canvas)sender).Background.Opacity = 0.8;
        }

        private void TextBox_EditText_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ((TextBox)sender).Focus();
        }
    }
}