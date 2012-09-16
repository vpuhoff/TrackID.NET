using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Resources;
using System.Reflection;
using System.Diagnostics;

namespace WindowsFormsApplication4
{
    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();
        }

        bool ready = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripProgressBar1.Value = 0;
            string s = System.Environment.GetCommandLineArgs()[1];
            if (File.Exists(s)==true )
            {
                GetInfo(s);
            }
            else
            {
                Application.ExitThread();
            }
            
        }

        public void GetInfo(string filename)
        {
            toolStripProgressBar1.Value = 10;
            SongInfo si = new SongInfo();
            File.Copy(filename,@"C:\codegen\test.mp3",true );
            toolStripStatusLabel1.Text = "Анализирование трека ...";
            run(@"C:\codegen\codegen.exe", " \"" + filename + "\" 20 45");
            toolStripProgressBar1.Value = 30;
            //run(@"C:\codegen\codegen.exe", @" C:\codegen\test.mp3 20 45");


            //richTextBox1.Text = "";

            

           //ResourceManager resourceManager = new ResourceManager("AssemblyName.Resources", Assembly.GetExecutingAssembly());
            //System.Diagnostics.Process proc2 = new System.Diagnostics.Process();
            //proc2.StartInfo.WorkingDirectory = @"C:\codegen\";
            ///proc2.StartInfo.FileName = @"C:\codegen\curl.exe";
            //proc2.StartInfo.Arguments = "-F \"api_key=K6ESC9GPDLWQNERPZ\" -F \"query=@C:\\codegen\\json_string.json\" \"http://developer.echonest.com/api/v4/song/identify\" >result.inf";
            //proc2.Start();
            //proc2.WaitForExit();

        }
        private void sortOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            //используем делегат для доступа к элементу формы из другого потока
            BeginInvoke(new MethodInvoker(delegate
            {
                if (!String.IsNullOrEmpty(outLine.Data))
                {
                    //выводим результат в консоль
                    string s = outLine.Data + Environment.NewLine;
                    if (s.Contains("metadata")==true )
                    {
                     File.WriteAllText(@"C:\codegen\json_string.json",s );
                     toolStripProgressBar1.Value = 23;
                    }
                    
                }
            }));
        }

        private void whenExitProcess(Object sender, EventArgs e)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                ready = true;
                //File.WriteAllText(@"C:\codegen\json_string.json", richTextBox1.Text);
                toolStripProgressBar1.Value = 28;
            }));
        }

        private void sortOutputHandler2(object sendingProcess, DataReceivedEventArgs outLine)
        {
            //используем делегат для доступа к элементу формы из другого потока
            BeginInvoke(new MethodInvoker(delegate
            {
                if (!String.IsNullOrEmpty(outLine.Data))
                {
                    //выводим результат в консоль
                    richTextBox1.AppendText(DosToUtf(outLine.Data + Environment.NewLine));
                    toolStripProgressBar1.Value = 60;
                }
            }));
        }

        private void whenExitProcess2(Object sender, EventArgs e)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                File.WriteAllText(@"C:\codegen\result.txt", richTextBox1.Text);
                toolStripProgressBar1.Value = 70;
            }));
        }

        private string DosToUtf(string src)
        {
            try
            {
                //создаем кодовую таблицу для кодировки DOS
                Encoding dos866 = Encoding.GetEncoding(866);
                //получаем массив байтов для строки
                byte[] srcBytes = dos866.GetBytes(src);
                //конвертируем из кодировки DOS в UTF8
                byte[] dstBytes = Encoding.Convert(dos866, UTF8Encoding.UTF8, srcBytes);
                //получем результирующую строку
                string source = UTF8Encoding.UTF8.GetString(dstBytes);
                return source;
            }
            catch (Exception error) //задаем исключение, если что-то пойдет не так
            {
                MessageBox.Show(error.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return "";
        }

        private void run(string utilityName, string arguments)
        {
            try
            {
                //создаем новый процесс, который будет работать с консолью
                Process pr = new Process();
                //задаем имя запускного файла
                toolStripProgressBar1.Value = 12;
                pr.StartInfo.FileName = utilityName;
                //задаем аргументы для этого файла
                pr.StartInfo.Arguments = arguments;
                //отключаем использование оболочки, чтобы можно было читать данные вывода
                pr.StartInfo.UseShellExecute = false;
                //перенаправляем данные вовода
                pr.StartInfo.RedirectStandardOutput = true;
                //задаем кодировку, чтобы читать кириллические символы
                pr.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(866);
                //запрещаем создавать окно для запускаемой программы                
                pr.StartInfo.CreateNoWindow = true;
                //подписываемся на событие, которые возвращает данные
                pr.OutputDataReceived += new DataReceivedEventHandler(sortOutputHandler);
                //включаем возможность определять когда происходит выход из программы, которую будем запускать
                pr.EnableRaisingEvents = true;
                pr.StartInfo.WorkingDirectory = @"C:\codegen\";
                //подписываемся на событие, когда процесс завершит работу
                toolStripProgressBar1.Value = 18;
                pr.Exited += new EventHandler(whenExitProcess);
                //запускаем процесс
                pr.Start();
                //начинаем читать стандартный вывод
                pr.BeginOutputReadLine();
                toolStripProgressBar1.Value = 0;
            }
            catch (Exception error)
            {
                MessageBox.Show("Ошибка при запуске!\n" + error.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void run2(string utilityName, string arguments)
        {
            try
            {
                toolStripProgressBar1.Value = 45;
                //создаем новый процесс, который будет работать с консолью
                Process pr = new Process();
                //задаем имя запускного файла
                pr.StartInfo.FileName = utilityName;
                //задаем аргументы для этого файла
                pr.StartInfo.Arguments = arguments;
                //отключаем использование оболочки, чтобы можно было читать данные вывода
                pr.StartInfo.UseShellExecute = false;
                //перенаправляем данные вовода
                pr.StartInfo.RedirectStandardOutput = true;
                //задаем кодировку, чтобы читать кириллические символы
                pr.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(866);
                //запрещаем создавать окно для запускаемой программы                
                pr.StartInfo.CreateNoWindow = true;
                //подписываемся на событие, которые возвращает данные
                pr.OutputDataReceived += new DataReceivedEventHandler(sortOutputHandler2);
                //включаем возможность определять когда происходит выход из программы, которую будем запускать
                pr.EnableRaisingEvents = true;
                pr.StartInfo.WorkingDirectory = @"C:\codegen\";
                //подписываемся на событие, когда процесс завершит работу
                pr.Exited += new EventHandler(whenExitProcess2);
                //запускаем процесс
                pr.Start();
                //начинаем читать стандартный вывод
                pr.BeginOutputReadLine();
                toolStripProgressBar1.Value = 50;
            }
            catch (Exception error)
            {
                MessageBox.Show("Ошибка при запуске!\n" + error.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public char SaySym(object code) {
          return (char)((int)code);
}// SaySym

        class SongInfo
        {
           public string artist_name="";
           public string title = "";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (ready==true )
            {
                ready = false;
                timer1.Enabled = false;
                toolStripStatusLabel1.Text = "Загрузка информации о треке ...";
                toolStripProgressBar1.Value = 35;
                run2(@"C:\codegen\curl.exe", "-F \"api_key=K6ESC9GPDLWQNERPZ\" -F \"query=@C:\\codegen\\json_string.json\" \"http://developer.echonest.com/api/v4/song/identify\" >result.inf");
            }
        }
          
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            tryparsejson(richTextBox1.Text);
            toolStripProgressBar1.Value = 100;
            progressBar1.Value = 100;
            progressBar1.Style = ProgressBarStyle.Continuous;
        }

        void tryparsejson(string text)
        {
            toolStripProgressBar1.Value = 80;
            string title = "";
            string artist_name = "";
            toolStripStatusLabel1.Text = "Нет информации о данном треке ...";
            int n1, n2;
            n1 = text.IndexOf("title");
            if (n1!=-1)
            {
                n2 = text.IndexOf("message", n1);
                if (n2!=-1)
                {
                    string s1;
                    s1 = text.Substring(n1 + 9, n2 - n1 - 13);
                    title = s1;
                    //MessageBox.Show(s1);
                    toolStripProgressBar1.Value = 90;
                }
            }
            n1 = text.IndexOf("artist_name");
            if (n1 != -1)
            {
                n1 = text.IndexOf(":",n1);
                n1 = text.IndexOf("\"", n1);
                n2 = text.IndexOf("\"", n1+1);
                if (n2 != -1)
                {
                    string s1;
                    s1 = text.Substring(n1 +1, n2 - n1 -1);
                    artist_name = s1;
                    //MessageBox.Show(s1);
                    toolStripStatusLabel1.Text = "Анализ завершен успешно.";
                    string s = "Исполнитель: " + artist_name + "\n";
                    s += "Название трека: " + title + "\n\n";
                    richTextBox1.Text = s;
                    toolStripProgressBar1.Value = 100;
                }
            }
        }
      
    }
}
