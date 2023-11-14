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
using System.Windows.Shapes;

namespace SoundRecorder
{
    /// <summary>
    /// Interaction logic for TimerForm.xaml
    /// </summary>
    public partial class TimerForm : Window
    {
        public TimerForm()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TimeSpan ts = MainWindow.RecordingTime;
            stopHour.Value = ts.Hours;
            stopMinute.Value = ts.Minutes;
            stopSecond.Value = ts.Seconds;
        }

        public delegate void TimerEventHandler(bool IsTimerRecord);
        public event TimerEventHandler TimerRec;

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if ((int)stopHour.Value > 0 || (int)stopMinute.Value > 0 || (int)stopSecond.Value > 0)
            {
                MainWindow.RecordingTime = new TimeSpan((int)stopHour.Value, (int)stopMinute.Value, (int)stopSecond.Value);
                TimerRec(true);
            }
            else
            {
                MainWindow.RecordingTime = new TimeSpan();
                TimerRec(false);
            }
            this.Close();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            TimerRec(false);
            this.Close();
        }

        


    }
}
