using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calendar001.Model
{
    class NotifyMenu
    {
        private System.Windows.Forms.ContextMenu notify;
        private MainWindow mainWindow;
        private NotifyIcon notifyIcon;
        private List<MenuItemWithID> notifyItems; // 사용되고 있는 menuItem을 저장한 배열

        public NotifyMenu(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            notifyItems = new List<MenuItemWithID>();
            notify = new System.Windows.Forms.ContextMenu();
            InitNotify();
        }

        public System.Windows.Forms.ContextMenu Notify{ get { return notify; } }

        private void InitNotify()
        {
            if (notify == null)
            {
                notify = new System.Windows.Forms.ContextMenu();
            }
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = Properties.Resources.notifyIcon;
            notifyIcon.Visible = true;
            notifyIcon.ContextMenu = notify;

            notifyIcon.DoubleClick += Notify_DoubleClick;

            AddMenuItem(0, "ProgramExit",
                (object click, EventArgs eClick) =>
                {
                    mainWindow.QuitProgram();
                });

            AddMenuItem(0, "ProgramHiding",
                (object click, EventArgs eClick) =>
                {
                    mainWindow.HideProgram();
                });

            AddMenuItem(0, "Dragging",
                (object click, EventArgs eClick) =>
                {
                    mainWindow.ControlDragging(click, eClick);
                });

            AddMenuItem(0, "Setting",
                (object click, EventArgs eClick) =>
                {
                    mainWindow.ShowSetting();
                });
        }


        private void AddMenuItem(int index, string ID, EventHandler clickEvent)
        {
            MenuItemWithID item = new MenuItemWithID();
            notifyItems.Add(item);
            notify.MenuItems.Add(item);
            item.Index = index;
            item.Text = mainWindow.FindResource(ID).ToString();
            item.Click += clickEvent;
            item.ID = ID;
        }

        private void Notify_DoubleClick(object sender, EventArgs e)
        {
            // Notify를 더블 클릭했을 경우 발동되는 이벤트
            mainWindow.ShowWindow();
        }
    }

    
}
