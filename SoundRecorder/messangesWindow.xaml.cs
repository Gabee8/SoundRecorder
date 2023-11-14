using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace SoundRecorder
{
    /// <summary>
    /// Interaction logic for messangesWindow.xaml
    /// </summary>
    public partial class messangesWindow : Window
    {
        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter,
            int x, int y, int width, int height, uint flags);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hwnd, uint msg,
            IntPtr wParam, IntPtr lParam);

        const int GWL_EXSTYLE = -20;
        const int WS_EX_DLGMODALFRAME = 0x0001;
        const int SWP_NOSIZE = 0x0001;
        const int SWP_NOMOVE = 0x0002;
        const int SWP_NOZORDER = 0x0004;
        const int SWP_FRAMECHANGED = 0x0020;
        const uint WM_SETICON = 0x0080;

        public static void RemoveIcon(Window window)
        {
            // Get this window's handle
            IntPtr hwnd = new WindowInteropHelper(window).Handle;

            // Change the extended window style to not show a window icon
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_DLGMODALFRAME);

            // Update the window's non-client area to reflect the changes
            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE |
                  SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
        }

        public messangesWindow(string caption, string message, bool dialog)
        {
            
            InitializeComponent();
            RemoveIcon(this);
            this.Title = caption;
            tbMessageBoxMessage.Text = message;
            if (!dialog)
            {
                btnMessageBoxClose.Visibility = Visibility.Visible;
                btnMessageBoxNo.Visibility = Visibility.Collapsed;
                btnMessageBoxYes.Visibility = Visibility.Collapsed;
            }
        }

        private void btnMessageBoxNo_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void imgMessageBoxCancel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void btnMessageBoxYes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btnMessageBoxClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        internal static System.Windows.Forms.DialogResult ShowCustomMessageBox(string message, string caption = "Default caption", bool dialog = true)
        {
            messangesWindow mb = new messangesWindow(caption, message, dialog);
            mb.Topmost = true;
            mb.ShowDialog();


            if (mb.DialogResult == null)
                return System.Windows.Forms.DialogResult.None;
            else if (mb.DialogResult == true)
                return System.Windows.Forms.DialogResult.Yes;
            else
                return System.Windows.Forms.DialogResult.No;
        }

       
    }
}

   
    
    
