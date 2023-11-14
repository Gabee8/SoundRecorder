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
using System.Windows.Navigation;
using System.Windows.Threading;
using NAudio.MediaFoundation;
using NAudio.FileFormats;
using NAudio.Codecs;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Lame;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;
using System.Threading;
using Ini;

namespace SoundRecorder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IWaveIn _captureInstance;
        LameMP3FileWriter wtr;
        MMDeviceEnumerator devEnum = new MMDeviceEnumerator();
        MMDevice RecDevice;
        DispatcherTimer VolumeMeter = new DispatcherTimer();
        DispatcherTimer RecordingTimer = new DispatcherTimer();
        Stopwatch stopWatch;
        bool isStartRec = false;
        bool isPauseRec = false;
        string temp = Path.GetTempPath();
        string TempRecordFile = "";
        string Extension = ".mp3";
        static string RootPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        string docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        int Bitrate = 192;
        MMDeviceCollection devices;
        //Config file
        IniFile ini = new IniFile(RootPath + "\\Settings.ini");

        private System.Windows.Forms.NotifyIcon notifier = new System.Windows.Forms.NotifyIcon();

        public static TimeSpan RecordingTime;

        bool IsTimer = false;
        int LastStateRecButton = -1;
        DispatcherTimer RecordingScheduleTimer = new DispatcherTimer();
        //Language Res
        public static ResourceDictionary mylangs;


        public MainWindow()
        {
            InitializeComponent();
            try
            {
                if (ini.IniReadValue("Settings", "Bitrate") != "")
                    Bitrate = int.Parse(ini.IniReadValue("Settings", "Bitrate"));
            }
            catch (Exception)
            {
            }
           

            VolumeMeter.Tick += VolumeMeterTimer_Tick;
            VolumeMeter.Interval = new TimeSpan(0, 0, 0, 0, 10);
            VolumeMeter.Start();


            this.notifier.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(notifier_MouseDown);
            this.notifier.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().ManifestModule.Name);
            RecordingScheduleTimer.Tick += StoppingTimer_Tick;

            //Language
            mylangs = Application.Current.Resources.MergedDictionaries[1];
            
           
           
        }

        void notifier_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (this.Visibility == System.Windows.Visibility.Hidden)
            {
                this.Visibility = System.Windows.Visibility.Visible;
                return;
            }
            if (this.Visibility == System.Windows.Visibility.Visible)
            {
                this.Visibility = System.Windows.Visibility.Hidden;
                return;
            }
            
            
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ini.IniReadValue("Settings", "ProgressBar") != "")
            {
                int progressStyleID = int.Parse(ini.IniReadValue("Settings", "ProgressBar"));
                Style styleW7 = this.FindResource("W7Progressbar") as Style;
                Style style = this.FindResource("VolumeMeter") as Style;
                if (progressStyleID == 0)
                {
                    VolumeL.ClearValue(ProgressBar.StyleProperty);
                    VolumeR.ClearValue(ProgressBar.StyleProperty);
                    VolumeR.Style = styleW7;
                    VolumeL.Style = styleW7;
                }
                else if (progressStyleID == 1)
                {
                    VolumeL.ClearValue(ProgressBar.StyleProperty);
                    VolumeL.Style = style;
                    VolumeR.ClearValue(ProgressBar.StyleProperty);
                    VolumeR.Style = style;
                }
            }

            ini.DeleteSection("Devices");
            devices = devEnum.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
            List<string> cnt2 = new List<string>();
            for (int i = 0; i < devices.Count; i++)
            {
                string sDevLowName = devices[i].FriendlyName;
                ini.IniWriteValue("Devices", "Dev" + i.ToString(), sDevLowName);
                cnt2.Add(sDevLowName);
            }

            if (ini.IniReadValue("Settings", "DefaultDevID") != "")
            {
                int DevID = 0;
                try
                {
                    DevID = int.Parse(ini.IniReadValue("Settings", "DefaultDevID"));
                    RecDevice = devices[DevID];
                }
                catch (Exception)
                {
                    try
                    {
                        RecDevice = devEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                    }
                    catch (Exception)
                    {
                        
                    }
                   
                }
            }

            string SelectedLang = "";
            if (ini.IniReadValue("Settings", "Language") != "")
            {
                SelectedLang = ini.IniReadValue("Settings", "Language");
                mylangs = new ResourceDictionary { Source = new Uri(String.Concat(RootPath + "\\Languages\\" + SelectedLang + ".xaml")) };
                this.Resources.MergedDictionaries.Add
                        (
                            new ResourceDictionary { Source = new Uri(String.Concat(RootPath + "\\Languages\\" + SelectedLang + ".xaml")) }
                        );
                Application.Current.Resources.MergedDictionaries[1] = mylangs;
            }
            
            
        }

        
       

        private void RecImage(int ID)
        {
            LastStateRecButton = ID;
            BitmapImage RecIcon = new BitmapImage();
            BitmapImage RecStopIcon = new BitmapImage();
            BitmapImage RecPauseIcon = new BitmapImage();
            BitmapImage RecPauseIcon2 = new BitmapImage();
            RecIcon.BeginInit();
            RecIcon.UriSource = new Uri("pack://application:,,,/Images/rec24_icon.png");
            RecIcon.EndInit();

            RecStopIcon.BeginInit();
            RecStopIcon.UriSource = new Uri("pack://application:,,,/Images/stop-24.png");
            RecStopIcon.EndInit();

            RecPauseIcon.BeginInit();
            RecPauseIcon.UriSource = new Uri("pack://application:,,,/Images/pauseN.png");
            RecPauseIcon.EndInit();

            RecPauseIcon2.BeginInit();
            RecPauseIcon2.UriSource = new Uri("pack://application:,,,/Images/pause2.png");
            RecPauseIcon2.EndInit();


            if (ID == 0)
            {
                recimg.Source = RecStopIcon;
                Reclabel.Text = mylangs["stop"].ToString();
            }
            if (ID == 1)
            {
                Reclabel.Text = mylangs["rec"].ToString();
                recimg.Source = RecIcon;
            }
            if (ID == 2)
            {
                pauseImg.Source = RecPauseIcon;
                pauseTxt.Text = mylangs["pause"].ToString();
            }
            if (ID == 3)
            {
                pauseImg.Source = RecPauseIcon2;
                pauseTxt.Text = mylangs["continue"].ToString();
            }
        }

        private void recBt_Click(object sender, RoutedEventArgs e)
        {
           

            if (isPauseRec)
            {
                _captureInstance.DataAvailable += waveSource_DataAvailable;
                isPauseRec = false;
                VolumeMeter.Start();
                stopWatch.Start();
                RecordingTimer.Start();
                RecImage(0);
                return;
            }

            if (isStartRec == false)
            {
                RecImage(0);
                
               
                var mediaType = MediaFoundationEncoder.SelectMediaType(AudioSubtypes.MFAudioFormat_MP3,
                        new WaveFormat(16000, 196, 2), 16000);
               

                // Set the device ID
                var capt = WaveIn.GetCapabilities(0);
                string ID = capt.ProductName;
                //string deviceID = ID;
                // Get Device from specified ID

                _captureInstance = RecDevice.DataFlow == DataFlow.Render ?
                            new WasapiLoopbackCapture(RecDevice) : new WasapiCapture(RecDevice);
                var waveFormatToUse = _captureInstance.WaveFormat;
                var sampleRateToUse = waveFormatToUse.SampleRate;
                var channelsToUse = waveFormatToUse.Channels;

                if (sampleRateToUse > 48000)        // LameMP3FileWriter doesn't support a rate more than 48000Hz
                {
                    sampleRateToUse = 48000;
                }
                else if (sampleRateToUse < 8000)    // LameMP3FileWriter doesn't support a rate less than 8000Hz
                {
                    sampleRateToUse = 8000;
                }

                if (channelsToUse > 2)              // LameMP3FileWriter doesn't support a number of channels more than 2
                {
                    channelsToUse = 2;
                }
                waveFormatToUse = WaveFormat.CreateCustomFormat(_captureInstance.WaveFormat.Encoding,
                                                    sampleRateToUse,
                                                    channelsToUse,
                                                    _captureInstance.WaveFormat.AverageBytesPerSecond,
                                                    _captureInstance.WaveFormat.BlockAlign,
                                                    _captureInstance.WaveFormat.BitsPerSample);
                _captureInstance.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
                _captureInstance.RecordingStopped += audio_Stopped;

                RecordingTimer.Tick += RecordingTimer_Tick;
                RecordingTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
               

                Random rnd = new Random();
                int num = rnd.Next(1000,9999);
                stopWatch = new Stopwatch();

                TempRecordFile = temp + "ART" + num.ToString() + ".tmp";
                wtr = new LameMP3FileWriter(TempRecordFile, waveFormatToUse, Bitrate);
                _captureInstance.StartRecording();
                stopWatch.Start();
                RecordingTimer.Start();
                if (ini.IniReadValue("Settings","HiddenMode") == "1")
                {
                    this.notifier.Visible = true;
                    this.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            if (isStartRec == true)
            {
                SaveFile();
               
            }
            if (IsTimer)
            {
                RecordingScheduleTimer.Start(); 
            }
            timeLB.Foreground = Brushes.Red;
            timerBt.IsEnabled = false;
        }

        private void AudioRecStop()
        {
            RecImage(1);
            _captureInstance.StopRecording();
            this.wtr.Close();
            this._captureInstance.Dispose();
            notifier.Visible = false;

        }
        void audio_Stopped(object sender, StoppedEventArgs e)
        {
            if (wtr != null)
            {
                RecImage(1);
            }
        }
        private void SaveFile()
        {
            int count = 1;
            string fullPath = "";
            stopWatch.Stop();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = mylangs["filename"] as string;
            saveFileDialog.Filter = mylangs["mp3file"].ToString() + " | *.mp3";
            saveFileDialog.DefaultExt = "mp3";
            saveFileDialog.InitialDirectory = docs;
            fullPath = Path.GetFullPath(saveFileDialog.InitialDirectory + "\\" + saveFileDialog.FileName + Extension);
            saveFileDialog.OverwritePrompt = false;
           
            string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            string newFullPath = fullPath;

            while (File.Exists(newFullPath))
            {
                string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);

                newFullPath = Path.Combine(docs, tempFileName + Extension);
                saveFileDialog.FileName = tempFileName;
            }

            if (saveFileDialog.ShowDialog() == true)
            {

                AudioRecStop();
                File.Copy(TempRecordFile, saveFileDialog.FileName);
                File.Delete(TempRecordFile);
                isStartRec = false;
                stopWatch.Reset();
                RecImage(1);
                Dispatcher.BeginInvoke(new ThreadStart(() => {
                timeLB.Foreground = new SolidColorBrush(Colors.Black);
                timerBt.IsEnabled = true;
                }));
            }
            else
            {
                System.Media.SystemSounds.Exclamation.Play();
                _captureInstance.StopRecording();
                if (messangesWindow.ShowCustomMessageBox(mylangs["questionWExit"].ToString(), mylangs["mainTitle"].ToString(), true) == System.Windows.Forms.DialogResult.Yes)
                {
                    AudioRecStop();
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        File.Copy(TempRecordFile, saveFileDialog.FileName);
                    }
                    
                    File.Delete(TempRecordFile);
                }
                else
                {
                    AudioRecStop();
                    wtr.Close();
                    File.Delete(TempRecordFile);
                }
                
                isStartRec = false;
                stopWatch.Reset();
                RecImage(1);
                Dispatcher.BeginInvoke(new ThreadStart(() =>
                {
                    timeLB.Foreground = new SolidColorBrush(Colors.Black);
                    timerBt.IsEnabled = true;
                }));

            }
            
           
        }
        void waveSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (wtr != null)
            {
                wtr.Write(e.Buffer, 0, e.BytesRecorded);
                isStartRec = true;
            }
        }
         private void RecordingTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
            timeLB.Content = elapsedTime;
        }

        private void VolumeMeterTimer_Tick(object sender, EventArgs e)
        {
            if (RecDevice != null)
            {
                VolumeL.Value = (int)(Math.Round(RecDevice.AudioMeterInformation.PeakValues[0] * 100));
                VolumeR.Value = (int)(Math.Round(RecDevice.AudioMeterInformation.PeakValues[1] * 100));
            }
            else
            {
                
                try
                {
                    RecDevice = devEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                    VolumeL.Value = (int)(Math.Round(RecDevice.AudioMeterInformation.PeakValues[0] * 100));
                    VolumeR.Value = (int)(Math.Round(RecDevice.AudioMeterInformation.PeakValues[1] * 100));
                }
                catch (Exception)
                {

                }


            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_captureInstance != null)
            {
                if (wtr.CanWrite)
                {
                    System.Media.SystemSounds.Exclamation.Play();
                    _captureInstance.StopRecording();
                    if (messangesWindow.ShowCustomMessageBox(mylangs["questionWExit"].ToString(), mylangs["mainTitle"].ToString(), true) == System.Windows.Forms.DialogResult.Yes)
                    {
                        SaveFile();
                    }
                    else
                    {
                        wtr.Close();
                        File.Delete(TempRecordFile);
                    }
                }
               
               
                
            }
            
        }

        private void settingsBt_Click(object sender, RoutedEventArgs e)
        {
            Settings sett = new Settings();
            sett.Owner = this;
            sett.SettingsChange += new Settings.TextAvailableEventHandler(Settings_Text);
            sett.Show();
        }
        private void Settings_Text(string BitrateText, int SelectedId)
        {
            Bitrate = int.Parse(BitrateText);
            ini.IniWriteValue("Settings", "Bitrate", BitrateText);
            RecDevice = devices[SelectedId];
            if (ini.IniReadValue("Settings", "ProgressBar") != "")
            {
                int progressStyleID = int.Parse(ini.IniReadValue("Settings", "ProgressBar"));
                Style styleW7 = this.FindResource("W7Progressbar") as Style;
                Style style = this.FindResource("VolumeMeter") as Style;
                if (progressStyleID == 0)
                {
                    VolumeL.ClearValue(ProgressBar.StyleProperty);
                    VolumeR.ClearValue(ProgressBar.StyleProperty);
                    VolumeR.Style = styleW7;
                    VolumeL.Style = styleW7;
                }
                else if (progressStyleID == 1)
                {
                    VolumeL.ClearValue(ProgressBar.StyleProperty);
                    VolumeL.Style = style;
                    VolumeR.ClearValue(ProgressBar.StyleProperty);
                    VolumeR.Style = style;
                }
            }
            Application.Current.Resources.MergedDictionaries[1].Clear();
            this.Resources.MergedDictionaries.Add
                    (
                        mylangs
                    );
            Application.Current.Resources.MergedDictionaries[1] = mylangs;
            if (LastStateRecButton > -1)
            {
                RecImage(LastStateRecButton);
            }
           
           
        }

        private void aboutBt_Click(object sender, RoutedEventArgs e)
        {
            AboutWnd aboutwnd = new AboutWnd();
            aboutwnd.Owner = this;
            aboutwnd.ShowDialog();
        }

        private void pauseBt_Click(object sender, RoutedEventArgs e)
        {
            if (isStartRec == true)
            {
                stopWatch.Stop();
                if (isPauseRec)
                {
                    _captureInstance.DataAvailable += waveSource_DataAvailable;
                    isPauseRec = false;
                    VolumeMeter.Start();
                    stopWatch.Start();
                    RecordingTimer.Start();
                    //RecImage(0);
                    RecImage(2);
                    return;
                }
                _captureInstance.DataAvailable -= waveSource_DataAvailable;
                isPauseRec = true;

                RecImage(3);
            }
        }

        

        private void timerBt_Click(object sender, RoutedEventArgs e)
        {
            TimerForm tmfrm = new TimerForm();
            tmfrm.Owner = Window.GetWindow(this);
            tmfrm.TimerRec += new TimerForm.TimerEventHandler(timerFrm_Events);
            tmfrm.ShowDialog();
        }

        private void timerFrm_Events(bool IsTimerRecord)
        {
            IsTimer = IsTimerRecord;
            RecordingScheduleTimer.Interval = RecordingTime;
            if (IsTimerRecord)
            {
                timerBt.IsChecked = true;
            }
            else
            {
                timerBt.IsChecked = false;
            }
        }

        private void StoppingTimer_Tick(object sender, EventArgs e)
        {
            string dates = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss");
            string path = docs + "\\" + mylangs["scheduledFilename"].ToString() + "_" + dates + Extension;
            AudioRecStop();
            File.Copy(TempRecordFile, path);
            File.Delete(TempRecordFile);
            isStartRec = false;
            stopWatch.Reset();
            RecImage(1);
            RecordingScheduleTimer.Stop();
            this.Close();
        }
      
    }
}
