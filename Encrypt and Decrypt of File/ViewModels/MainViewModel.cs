using Encrypt_and_Decrypt_of_File.Commands;
using Encrypt_and_Decrypt_of_File.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Encrypt_and_Decrypt_of_File.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public MainWindow MainWindows { get; set; }

        public ObservableCollection<Texts> Texts { get; set; }

        private Texts _Text;
        public Texts Text { get { return _Text; } set { _Text = value; OnPropertyChanged(); } }

        public Stopwatch watch = Stopwatch.StartNew();

        public RelayCommand SelectFileCommand { get; set; }
        public RelayCommand StartCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

        string text = string.Empty;
        string path = string.Empty;

        double p = 0.0;
        double pMax = 0.0;
        double pMin = 0.0;
        long length = 0;
        bool hasEncrypt= false;
        bool hasDecrypt = false;
        string passwordtext = string.Empty;
        bool useHashing = false;
        int count = 100;
        int currentcount = 0;
        int count2 = 50;
        bool cancel = false;


        private double _pv;
        public double Pv { get { return _pv; } set { _pv = value; OnPropertyChanged(); } }



        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        
        DispatcherTimer timer = new DispatcherTimer();


        public MainViewModel()
        {

            timer.Interval = TimeSpan.FromMilliseconds(5);
            timer.Tick += Timer_Tick;
            timer.Start();


            SelectFileCommand = new RelayCommand((sender) =>
            {

                OpenFileDialog openFile = new OpenFileDialog();

                //openFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                openFile.InitialDirectory = Path.GetFullPath(Environment.CurrentDirectory + @"../../../../");
                openFile.Filter = "Text files (*.txt)|*.txt";

                if (openFile.ShowDialog() == true)
                {
                    MainWindows.FilePathTextBox.Text = openFile.FileName;

                }


            });


            StartCommand = new RelayCommand((sender) =>
            {
                Text = new Texts();
                timer.Start();
                try
                {
                    if (MainWindows.FilePathTextBox.Text != "Path")
                    {
                        if (MainWindows.PasswordTextBox.Text != string.Empty)
                        {
                            if (MainWindows.EncryptRadioButton.IsChecked != false || MainWindows.DecryptRadioButton.IsChecked != false)
                            {

                                if (cancellationTokenSource != null)
                                {
                                    ThreadPool.QueueUserWorkItem((j) => { EncryptandDecrypt(passwordtext, Text, useHashing, cancellationTokenSource.Token); });
                                    if (MainWindows.EncryptRadioButton.IsChecked == true || MainWindows.DecryptRadioButton.IsChecked == true)
                                    {
                                        MainWindows.PasswordTextBox.IsEnabled = false;
                                    }
                                }

                            }
                            else
                            {
                                MessageBox.Show($"Select radiobutton. ");
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Enter password: ");
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Select file. ");
                    }

                cancel = true;

                }
                catch (Exception)
                {


                }

    
            });



            CancelCommand = new RelayCommand((sender) =>
            {

                try
                {
                    if (cancel == true)
                    {
                        if (cancellationTokenSource != null)
                        {
                            ThreadPool.QueueUserWorkItem((i) => { EncryptandDecrypt(passwordtext, Text, useHashing, cancellationTokenSource.Token); });


                            cancellationTokenSource.Cancel();

                        }
                    }
                }
                catch (Exception)
                {


                }

            });

        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            
                MainWindows.StartButtun.Dispatcher.BeginInvoke(new Action(() =>
                {

                    path = MainWindows.FilePathTextBox.Text;
                    p = MainWindows.PorcessProgressBar.Value;
                    pMax = MainWindows.PorcessProgressBar.Maximum;
                    pMin = MainWindows.PorcessProgressBar.Minimum;
                    hasEncrypt = (bool)MainWindows.EncryptRadioButton.IsChecked;
                    hasDecrypt = (bool)MainWindows.DecryptRadioButton.IsChecked;
                    passwordtext = MainWindows.PasswordTextBox.Text;
                    

                }));
            

        }
        private void EncryptandDecrypt(string passwordtext, Texts text, bool useHashing, CancellationToken token)
        {

            if (path != "Path")
            {
                length = new System.IO.FileInfo(path).Length;

                string t = string.Empty;
                string t2 = string.Empty;

                if (!token.IsCancellationRequested)
                {
                    if (hasEncrypt == true)
                    {
                        MessageBox.Show($"hasEncrypt");


                        for (int i = 0; i < count; i++)
                        {
                            Thread.Sleep(count2);
                            if (i == count - 1)
                            {

                                for (int j = 0; j < 101; j++)
                                {
                                    Thread.Sleep(500);


                                    Pv = j;


                                    if (length >= 0.005)
                                    {
                                        MessageBox.Show("File is not empty");

                                        break;
                                    }

                                    else
                                    {

                                        if (Pv == 100)
                                        {
                                            try
                                            {
                                                t = EncryptString(passwordtext, Text.words());

                                                FileStreamWrite(t, path);
                                                FileStreamRead(t, path);



                                                Thread.Sleep(500);

                                                Pv = 0;

                                            }
                                            catch (Exception)
                                            {

                                            }
                                        }

                                        if (Pv != 100)
                                        {
                                            try
                                            {

                                                t = EncryptString(passwordtext, Text.words().Substring(0, Convert.ToInt32(100 * (j++))));

                                                FileStreamWrite(t, path);
                                                FileStreamRead(t, path);


                                                Thread.Sleep(500);

                                                // Pv = 0;
                                            }
                                            catch (Exception)
                                            {


                                            }

                                        }

                                        currentcount = int.Parse(Pv.ToString());

                                    }
                                }
                            }
                        }


                    }

                    if (hasDecrypt == true)
                    {
                        MessageBox.Show($"hasDecrypt");


                        for (int i = 0; i < count; i++)
                        {
                            Thread.Sleep(count2);
                            if (i == count - 1)
                            {

                                for (int j = 0; j < 101; j++)
                                {
                                    Thread.Sleep(50);
                                    Pv = j;

                                    if (length >= 0.005)
                                    {
                                        MessageBox.Show("File is not empty");

                                        break;
                                    }

                                    else
                                    {

                                        if (Pv == 100)
                                        {
                                            try
                                            {
                                                t2 = DecryptString(passwordtext, EncryptString(passwordtext, Text.words()));

                                                FileStreamWrite(t2, path);
                                                FileStreamRead(t2, path);



                                                Thread.Sleep(500);

                                                Pv = 0;

                                            }
                                            catch (Exception)
                                            {

                                            }
                                        }

                                        if (Pv != 100)
                                        {
                                            try
                                            {

                                                t2 = DecryptString(passwordtext, EncryptString(passwordtext, Text.words().Substring(0, Convert.ToInt32(100 * (j++)))));

                                                FileStreamWrite(t2, path);
                                                FileStreamRead(t2, path);



                                                Thread.Sleep(500);

                                                // Pv = 0;
                                            }
                                            catch (Exception)
                                            {


                                            }

                                        }

                                        currentcount = int.Parse(Pv.ToString());

                                    }
                                }
                            }
                        }


                    }

                  
                }

                if(token.IsCancellationRequested)
                {





                    if (File.Exists(path))
                    {
                        File.Delete(path);
                        File.Create(path).Close();
                    }
                 
                    for (int j = currentcount; j >= 0; j--)
                    {
                        Thread.Sleep(50);
                        Pv = j;
                    }
                       
                    MessageBox.Show($"cryption canceled. And File Recreated.");
     
                  
                }
            }
       


        }

        static void FileStreamWrite(string text, string filepath)
        {
            try
            {
                using (FileStream fs = new FileStream($" {filepath}", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {

                    byte[] bytes = Encoding.UTF8.GetBytes(text);
                    fs.Write(bytes, 0, bytes.Length);

                }

            }
            catch (Exception)
            {


            }

        }

        static void FileStreamRead(string text, string filepath)
        {
            try
            {
                // FileSecurity fileSecurity = new FileSecurity();

                using (FileStream fs = new FileStream($"{filepath}", FileMode.OpenOrCreate, FileAccess.Read, FileShare.None))
                {
                    byte[] bytes = new byte[(int)fs.Length];
                    fs.Read(bytes, 0, bytes.Length);
                    text = Encoding.UTF8.GetString(bytes);

                }

            }
            catch
            {

            }

        }

        public static string EncryptString(string key2, string plainText)
        {
            TripleDESCryptoServiceProvider TDC = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] byteHash, byteText;

            byteHash = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key2));
            byteText = UTF8Encoding.UTF8.GetBytes(plainText);

            md5.Clear();

            TDC.Key = byteHash;
            TDC.Mode = CipherMode.ECB;

            return Convert.ToBase64String(TDC.CreateEncryptor().TransformFinalBlock(byteText, 0, byteText.Length));
        }

        public static string DecryptString(string key2, string cipherText)
        {
            TripleDESCryptoServiceProvider TDC = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] byteHash, byteText;

            byteHash = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key2));
            byteText = Convert.FromBase64String(cipherText);

            md5.Clear();

            TDC.Key = byteHash;
            TDC.Mode = CipherMode.ECB;

            return UnicodeEncoding.UTF8.GetString(TDC.CreateDecryptor().TransformFinalBlock(byteText, 0, byteText.Length));
        }
    }
}
