using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

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
            var thread_id = Thread.CurrentThread.ManagedThreadId;

            await Task.Yield().ConfigureAwait(false);

            var thread_id2 = Thread.CurrentThread.ManagedThreadId;

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
                    //await Task.Delay(1, Cancel).ConfigureAwait(false);

                    foreach (var word in words)
                        if (dict.ContainsKey(word))
                            dict[word]++;
                        else
                            dict.Add(word, 1);

                    Progress?.Report(reader.BaseStream.Position / (double)reader.BaseStream.Length);
                }
            }

            var thread_id3 = Thread.CurrentThread.ManagedThreadId;

            await App.Current.Dispatcher;

            var thread_id4 = Thread.CurrentThread.ManagedThreadId;

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