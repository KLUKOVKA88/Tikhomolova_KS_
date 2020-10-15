using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WPFTests
{
    public partial class MainWindow
    {
        public MainWindow() => InitializeComponent();

        private async void OnOpenFileClick(object sender, RoutedEventArgs e)
        {
            // Мы находились в ThreadID == 1
            await Task.Yield(); //Даем время на обработку сообщений пользовательского интерфейса
            // Мы снова в ThreadID == 1

            var dialog = new OpenFileDialog
            {
                Title = "Выбор файла для чтения",
                Filter = "Тектовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*",
                RestoreDirectory = true
            };

            if (dialog.ShowDialog() != true) return;

            //var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);


            //using (var reader = new StreamReader(dialog.FileName))
            //{
            //    while (!reader.EndOfStream) 
            //    {
            //        var line = await reader.ReadLineAsync().ConfigureAwait(false);
            //        var words = line.Split(' ');
            //        //Thread.Sleep(100);
            //        //await Task.Delay(1);

            //        foreach (var word in words)
            //            if (dict.ContainsKey(word))
            //                dict[word]++;
            //            else
            //                dict.Add(word, 1);

            //        //ProgressInfo.Value = reader.BaseStream.Position / (double)reader.BaseStream.Length;
            //        ProgressInfo.Dispatcher.Invoke(() =>
            //        ProgressInfo.Value = reader.BaseStream.Position / (double)reader.BaseStream.Length);
            //    }
            //}


            //var count = dict.Count;
            //Result.Text = $"Число слов {count}");
            //Result.Dispatcher.Invoke(() => Result.Text = $"Число слов {count}");

            StartAction.IsEnabled = false;
            CancelAction.IsEnabled = true;

            _ReadeingFileCancellation = new CancellationTokenSource();

            var cancel = _ReadeingFileCancellation.Token;
            IProgress<double> progress = new Progress<double>(p => ProgressInfo.Value = p);

            try 
            {
                
                var count = await GetWordsCountAsync(dialog.FileName, progress, cancel).ConfigureAwait(true);
                Result.Text = $"Число слов {count}";
            }
            catch(OperationCanceledException)
            {
                Debug.WriteLine("Операция чтения файла юыла отменена");
                Result.Text = "---";
                progress.Report(0);

            }           

            CancelAction.IsEnabled = false;
            StartAction.IsEnabled = true;
        }


        private static async Task<int> GetWordsCountAsync(string FileName, IProgress<double> Progress = null, CancellationToken Cancel = default)
        {
            //Мы находимся в ThreadID == 7 (например)
            await Task.Yield();
            // Тепрь мы в ThreadID == 12 (например, а возможно и обратно в 7)

            var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            Cancel.ThrowIfCancellationRequested();
            using (var reader = new StreamReader(FileName))
            {
                while (!reader.EndOfStream)
                {
                    Cancel.ThrowIfCancellationRequested();
                    var line = await reader.ReadLineAsync().ConfigureAwait(false);
                    //ConfigureAwait(false); - требование вернуться в произвольный поток из пула потоков
                    var words = line.Split(' ');
                    //Thread.Sleep(100);
                    await Task.Delay(1);

                    foreach (var word in words)
                        if (dict.ContainsKey(word))
                            dict[word]++;
                        else
                            dict.Add(word, 1);

                    Progress?.Report(reader.BaseStream.Position / (double)reader.BaseStream.Length);
                }
            }

            return dict.Count;

        }

        private CancellationTokenSource _ReadeingFileCancellation;

        private void OnCancelReadenClick(object sender, RoutedEventArgs e)
        {
            _ReadeingFileCancellation?.Cancel();
        }

        //private void ComputeResultButton_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    new Thread(() =>
        //    {
        //       var result = GetResultHard();
        //       UpdateResultValue(result);
        //    }){ IsBackground = true }.Start();
        //}

        //private void UpdateResultValue(string Result)
        //{
        //    if (Dispatcher.CheckAccess())
        //        ResultText.Text = Result;
        //    else
        //        Dispatcher.Invoke(() => UpdateResultValue(Result));
        //}

        //private string GetResultHard()
        //{
        //    for(var i = 0; i < 500; i++)
        //    {
        //        Thread.Sleep(10);
        //    }

        //    return "Hello World";
        //}
    }
}                                                  