using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Hotkeys;

namespace OpenRtssText
{
    public partial class Form1 : Form
    {
        private Hotkeys.GlobalHotkey up;
        private Hotkeys.GlobalHotkey down;
        private Hotkeys.GlobalHotkey right;
        private Hotkeys.GlobalHotkey left;
        public static int wordlength = 50;
        public Form1()
        {
            InitializeComponent();
            up = new Hotkeys.GlobalHotkey(Constants.NOMOD, Keys.Up, this);
            down = new Hotkeys.GlobalHotkey(Constants.NOMOD, Keys.Down, this);
            right = new Hotkeys.GlobalHotkey(Constants.NOMOD, Keys.Right, this);
            left = new Hotkeys.GlobalHotkey(Constants.NOMOD, Keys.Left, this);
        }
        private void HandleHotkey()
        {
          //  Console.WriteLine("Up Hotkey pressed!");
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Hotkeys.Constants.WM_HOTKEY_MSG_ID)
            {
                Console.WriteLine(m.LParam);
                if (m.LParam.ToInt32() == 2490368)
                {
                    Console.WriteLine("Up Hotkey pressed!");
                    positionY = positionY - 10;
                    RTSSHandler.Update(positionX, positionY);
                }
                if (m.LParam.ToInt32() == 2621440)
                {
                    Console.WriteLine("Down Hotkey pressed!");
                    positionY = positionY + 10;
                    RTSSHandler.Update(positionX, positionY);
                }
                if (m.LParam.ToInt32() == 2424832)
                {
                    Console.WriteLine("Left Hotkey pressed!");
                    positionX = positionX - 10;
                    RTSSHandler.Update(positionX, positionY);
                }
                if (m.LParam.ToInt32() == 2555904)
                {
                    Console.WriteLine("Right Hotkey pressed!");
                    positionX = positionX + 10;
                    RTSSHandler.Update(positionX, positionY);
                }
               // HandleHotkey();
            }
                
            base.WndProc(ref m);
        }

        static uint positionX = 0;
        static uint positionY = 0;
        static string TEXT = "";

        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
        private void loadSetting()
        {
            string settingText = System.IO.File.ReadAllText(@"setting.json");
            JObject setting = JObject.Parse(settingText);
            positionX = (uint)setting["x"];
            positionY = (uint)setting["y"];
            TEXT = (string)setting["text"];
            wordlength = (int)setting["word_length"];
        }
        private void saveSetting()
        {
            string settingText = System.IO.File.ReadAllText(@"setting.json");
            JObject setting = JObject.Parse(settingText);
            setting["x"] = positionX;
            setting["y"] = positionY;
            setting["text"] = TEXT;
            setting["word_length"] = wordlength;
            System.IO.File.WriteAllText("setting.json",JsonConvert.SerializeObject(setting));
        }

        private static void hooker()
        {

            var watcher = new FileSystemWatcher(System.IO.Path.GetDirectoryName(Application.ExecutablePath));
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "text_temp.json";
            watcher.EnableRaisingEvents = true;
            watcher.Changed += OnChanged;
            Console.WriteLine("Hooked");
        }
        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
             Console.WriteLine($"Changed: {e.FullPath}");
            updateRTSS2();
        }



        private void Form1_Load_1(object sender, EventArgs e)
        {
            RTSSHandler.RunRTSS();     
            loadSetting();
            hooker();
            updateRTSS2();
            up.Register();
            down.Register();
            left.Register();
            right.Register();
        }
        private static void updateRTSS()
        {
            RTSSHandler.Print(TEXT);
            RTSSHandler.Update(positionX,positionY);
        }
        private static void updateRTSS2()
        {
            while (!IsFileReady(@"text_temp.json"))
            {
            }
            string translatedtext = System.IO.File.ReadAllText(@"text_temp.json");
            JObject trans = JObject.Parse(translatedtext);
            TEXT = (string)trans["ttext"];
            string updatedtext = wordwrapper(TEXT);
            RTSSHandler.Print(updatedtext);
            RTSSHandler.Update(positionX, positionY);
            Console.WriteLine("Updated text");
        }

        private static string wordwrapper(string sentence)
        {
            int myLimit = wordlength;
          //  string sentence = "this is a long sentence that needs splitting to fit";
            string[] words = sentence.Split(' ');

            StringBuilder newSentence = new StringBuilder();


            string line = "";
            foreach (string word in words)
            {
                if ((line + word).Length > myLimit)
                {
                    newSentence.AppendLine(line);
                    line = "";
                }

                line += string.Format("{0} ", word);
            }

            if (line.Length > 0)
                newSentence.AppendLine(line);

            Console.WriteLine(newSentence.ToString());

            return newSentence.ToString();
        }

        public static bool IsFileReady(string filename)
        {
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try
            {
                using (FileStream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                    return inputStream.Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RTSSHandler.Print("");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            loadSetting();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            // x--
            positionX--;
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //x++
            positionX++;
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //y--
            positionY--;
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //y++
            positionY++;
            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            saveSetting();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            updateRTSS2();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //x-10
            positionX = positionX - 10;
           
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //x+10
            positionX = positionX + 10;
            
        }

        private void button11_Click(object sender, EventArgs e)
        {
            //y-10
            positionY = positionY - 10;
           
        }

        private void button12_Click(object sender, EventArgs e)
        {
            //y+10
            positionY = positionY + 10;
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            TEXT = textBox1.Text;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            try
            {
                wordlength = int.Parse(textBox2.Text);
            }
            catch
            {
                MessageBox.Show("Only numbers allowed >:(", "I'm disappointed in you...");
            }

            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
           
            
        }
    }
}
