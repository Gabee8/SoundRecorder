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
using Ini;
using System.IO;

namespace SoundRecorder
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        static string RootPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        IniFile ini = new IniFile(RootPath + "\\Settings.ini");
        public Settings()
        {
            InitializeComponent();
            try
            {
                if (ini.IniReadValue("Settings", "Bitrate") != "")
                {
                    comboBox1.SelectedValue = int.Parse(ini.IniReadValue("Settings", "Bitrate"));
                }

                string[] cnt = ini.GetEntryNames("Devices");
                List<string> cnt2 = new List<string>();

                int SelectedID = 0;
                if (ini.IniReadValue("Settings", "DefaultDevID") != "")
                {
                    SelectedID = int.Parse(ini.IniReadValue("Settings", "DefaultDevID"));
                }

                if (SelectedID > cnt.Count() - 1)
                {
                    SelectedID = 0;
                    ini.IniWriteValue("Settings", "DefaultDevID", SelectedID.ToString());
                }

                for (int i = 0; i < cnt.Count(); i++)
                {
                    cnt2.Add(ini.IniReadValue("Devices", cnt[i]));
                }
                
                for (int i = 0; i < cnt.Count(); i++)
                {
                    ComboBoxItem itm = new ComboBoxItem();
                    itm.Content = cnt2[i];
                    comboBox2.Items.Add(itm);
                }

                if (ini.IniReadValue("Settings", "DefaultDevID") != "")
                {
                    comboBox2.SelectedIndex = int.Parse(ini.IniReadValue("Settings", "DefaultDevID"));
                }

                if (ini.IniReadValue("Settings", "HiddenMode") != "")
                {
                    if (ini.IniReadValue("Settings", "HiddenMode") == "1")
                    {
                        hiddenMode.IsChecked = true;
                    }
                    else
                    {
                        hiddenMode.IsChecked = false;
                    }
                }

            }
            catch (Exception)
            {

            }
        }
        public delegate void TextAvailableEventHandler(string BitRate, int SelectedID);
        public event TextAvailableEventHandler SettingsChange;

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ini.IniWriteValue("Settings", "DefaultDevID", comboBox2.SelectedIndex.ToString());
            if (hiddenMode.IsChecked == true)
            {
                ini.IniWriteValue("Settings", "HiddenMode", "1");
            }
            else
            {
                ini.IniWriteValue("Settings", "HiddenMode", "0");
            }

            ini.IniWriteValue("Settings", "ProgressBar", comboBox3.SelectedIndex.ToString());
            try
            {
                var selectedTag = ((ComboBoxItem)comboBox4.SelectedItem).Tag.ToString();
                var selectedName = ((ComboBoxItem)comboBox4.SelectedItem).Content.ToString();
                string filename = selectedTag;
                MainWindow.mylangs = new ResourceDictionary { Source = new Uri(String.Concat(RootPath + filename)) };
                
                ini.IniWriteValue("Settings", "Language", selectedName);
            }
            catch (Exception ex)
            {
            }
            SettingsChange(comboBox1.SelectedValue.ToString(), comboBox2.SelectedIndex);
           
            Close();
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                if (button1 != null)
                {
                    button1.IsEnabled = true;
                }
                
            }
        }

        private void settwnd_Loaded(object sender, RoutedEventArgs e)
        {
            if (button1 != null)
            {
                button1.IsEnabled = false;
            }
            if (ini.IniReadValue("Settings", "ProgressBar") != "")
            {
                int ProgressID = int.Parse(ini.IniReadValue("Settings", "ProgressBar"));
                comboBox3.SelectedValue = ProgressID;
                comboBox3.SelectedIndex = ProgressID;
            }

            try
            {
                string SelectedLang = "";
                if (ini.IniReadValue("Settings", "Language") != "")
                {
                    SelectedLang = ini.IniReadValue("Settings", "Language");
                }
                foreach (string f in Directory.GetFiles(RootPath + "\\Languages", "*.xaml"))
                {
                    string filename = Path.GetFileName(f);
                    string Onlyfilename = Path.GetFileNameWithoutExtension(f);
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = Onlyfilename;
                    item.Tag = "/Languages/" + filename;
                    comboBox4.Items.Add(item);
                    if (SelectedLang == Onlyfilename)
                    {
                        comboBox4.SelectedIndex = comboBox4.Items.Count -1;
                    }
                }
               
                

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void hiddenMode_Checked(object sender, RoutedEventArgs e)
        {
            if (button1 != null)
            {
                button1.IsEnabled = true;
            }
        }

        private void hiddenMode_Unchecked(object sender, RoutedEventArgs e)
        {
            if (button1 != null)
            {
                button1.IsEnabled = true;
            }
        }

        private void comboBox3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (button1 != null)
            {
                button1.IsEnabled = true;
            }
        }

        private void comboBox4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (button1 != null)
            {
                button1.IsEnabled = true;
                
            }
        }
    }
}
