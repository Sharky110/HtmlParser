using HtmlParser.Commands;
using HtmlParser.EventArgs;
using HtmlParser.Managers;
using HtmlParser.Models;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HtmlParser.ViewModels
{
    class HtmlParserVM : HtmlParserVMBase
    {
        #region Constants

        private const string DialogFilter = "FileDialogFilter";
        private const string Pause = "Приостановить";
        private const string Resume = "Продолжить";

        #endregion

        #region Commands

        public ICommand OpenFileCommand { get; }
        public ICommand FindTagsCommand { get; }
        public ICommand StartStopCommand { get; }
        public ICommand AbortCommand { get; }

        #endregion

        #region Private variables

        private ObservableCollection<Page> _listOfPages;
        private string _filePath;
        private string _status;
        private double _progressBarValue;
        private double _itemProgressValue;
        private double percent;

        private ManualResetEvent _manualEvent;

        private CancellationTokenSource source;
        private CancellationToken token;
        private string _startStopButtonName;

        #endregion

        #region Properties

        public ObservableCollection<Page> ListOfPages
        {
            get
            {
                return _listOfPages;
            }
            set
            {
                SetProperty(ref _listOfPages, value);
            }
        }

        public string FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public double ProgressBarValue
        {
            get => _progressBarValue;
            set => SetProperty(ref _progressBarValue, value);
        }

        public double ItemProgressValue
        {
            get => _itemProgressValue;
            set => SetProperty(ref _itemProgressValue, value);
        }

        public string StartStopButtonName
        {
            get => _startStopButtonName;
            set => SetProperty(ref _startStopButtonName, value);
        }

        public ProgressBarManager ProgressBarManager { get; } = ProgressBarManager.Instance;

        #endregion

        public HtmlParserVM()
        {
            OpenFileCommand = new RelayCommand(c => OpenFile());
            FindTagsCommand = new RelayCommand(c => FindTags());
            StartStopCommand = new RelayCommand(c => StartStop());
            AbortCommand = new RelayCommand(c => Abort());

            Status = Models.Status.Ready;

            ProgressBarManager.Increase += IncreaseProgress;

            _manualEvent = new ManualResetEvent(true);

            StartStopButtonName = Pause;
    }

        private void Abort()
        {
            if (Models.Status.IsWorking(Status))
            {
                source?.Cancel();
            }
        }

        private void StartStop()
        {
            
            if (Status == Models.Status.TaskPaused)
            {
                _manualEvent.Set();
                StartStopButtonName = Pause;
            }
            else if (Models.Status.IsWorking(Status))
            {

                _manualEvent.Reset();
                Status = Models.Status.TaskPaused;
                StartStopButtonName = Resume;
            }

        }

        public async void OpenFile()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = ConfigurationManager.AppSettings.Get(DialogFilter)
            };

            openFileDialog.ShowDialog();
            if (openFileDialog.FileName == string.Empty)
                return;

            FilePath = openFileDialog.FileName;

            var sourceFile = await Task.Run(() => File.ReadAllText(FilePath, Encoding.GetEncoding(1252)));

            ListOfPages = new ObservableCollection<Page>(sourceFile.Split('\n').Select(x => new Page(x)).ToList());
            if(ListOfPages.Count != 0)
                percent = 50d / ListOfPages.Count;
        }

        public async void FindTags()
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                System.Windows.MessageBox.Show("Файл не открыт.", "Ошибка");
                return;
            }

            if (Models.Status.IsWorking(Status))
                return;

            source = new CancellationTokenSource();
            token = source.Token;

            ProgressBarValue = 0;

            try
            {
                await Task.Run(() => GetSources(), token);
                await Task.Run(() => CountTags(), token);
                await Task.Run(() => SetBiggestUrl(), token);
            }
            catch (OperationCanceledException)
            {
                Status = Models.Status.TaskAborted;
                ProgressBarValue = 0;
            }
            finally
            {
                if(Status != Models.Status.TaskAborted)
                    Status = Models.Status.Done;
            }
        }

        public void GetSources()
        {
            Status = Models.Status.GettingSources;
            HttpWebRequest request;
            HttpWebResponse responce;
            foreach (var page in ListOfPages)
            {
                if (token.IsCancellationRequested)
                {
                    Status = Models.Status.TaskAborted;
                    return;
                }

                _manualEvent.WaitOne();
                Status = Models.Status.GettingSources;
                try
                {
                    request = (HttpWebRequest)WebRequest.Create(page.URL);
                    responce = (HttpWebResponse)request.GetResponse();
                }
                catch
                {
                    ProgressBarManager.IncreaseProgress(percent);
                    continue;
                }

                using (var sr = new StreamReader(responce.GetResponseStream(), Encoding.GetEncoding("windows-1251")))
                    page.SourceCode = sr.ReadToEnd();

                ProgressBarManager.IncreaseProgress(percent);
            }
        }

        public void CountTags()
        {
            Status = Models.Status.CountTags;
            foreach (var page in ListOfPages)
            {
                if (string.IsNullOrEmpty(page.SourceCode))
                {
                    ProgressBarManager.IncreaseProgress(percent);
                    continue;
                }
                    
                if (token.IsCancellationRequested)
                {
                    Status = Models.Status.TaskAborted;
                    return;
                }

                _manualEvent.WaitOne();
                Status = Models.Status.CountTags;

                page.AmountOfTags = page.SourceCode.Split(new string[] { "</a>" }, StringSplitOptions.None).Count() - 1;
                page.AmountOfTags += page.SourceCode.Split(new string[] { "</A>" }, StringSplitOptions.None).Count() - 1;
                ProgressBarManager.IncreaseProgress(percent);
            }
        }

        public void SetBiggestUrl()
        {
            foreach (var page in ListOfPages)
            {
                if (token.IsCancellationRequested)
                {
                    Status = Models.Status.TaskAborted;
                    return;
                }
                _manualEvent.WaitOne();
                if (page.AmountOfTags == ListOfPages.Max(p => p.AmountOfTags))
                {
                    page.IsBiggest = true;
                }
            }
        }

        private void IncreaseProgress(object sender, NewProgressBarValueEventsArgs e)
        {
            ProgressBarValue += e.NewValue;
        }
    }
}
