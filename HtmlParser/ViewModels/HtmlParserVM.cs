using HtmlParser.Commands;
using HtmlParser.EventArgs;
using HtmlParser.Managers;
using HtmlParser.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HtmlParser.ViewModels
{
    class HtmlParserVM : HtmlParserVMBase
    {
        #region Constants

        private const string DialogFilter = "FileDialogFilter";

        #endregion

        #region Commands

        public ICommand OpenFileCommand { get; }
        public ICommand FindTagsCommand { get; }

        #endregion

        #region Private variables

        private List<Page> _listOfPages;
        private string _filePath;
        private string _status;
        private double _progressBarValue;
        private double _itemProgressValue;
        private double _count;

        #endregion

        #region Properties

        public List<Page> ListOfPages
        {
            get => _listOfPages;
            set => SetProperty(ref _listOfPages, value);
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

        public ProgressBarManager ProgressBarManager { get; } = ProgressBarManager.Instance;

        #endregion

        public HtmlParserVM()
        {
            OpenFileCommand = new RelayCommand(c => OpenFile());
            FindTagsCommand = new RelayCommand(c => FindTags());
            Status = Models.Status.Ready;
            ProgressBarManager.NewProgressBarValue += ChangeSlider;
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

            ListOfPages = sourceFile.Split('\n').ToList().Select(x => new Page(x)).ToList();
            _count = ListOfPages.Count;
        }

        public async void FindTags()
        {
            if (FilePath == null)
            {
                System.Windows.MessageBox.Show("Файл не открыт.", "Ошибка");
                return;
            }
            ProgressBarManager.ChangeProgressBarValue(0);
            await Task.Run(() => GetSources());
            await Task.Run(() => CountTags());
            Status = Models.Status.Done;
        }

        public void CountTags()
        {
            Status = Models.Status.CountTags;
            foreach (var page in ListOfPages)
            {
                page.AmountOfTags = page.SourceCode.Split(new string[] { "</a>" }, StringSplitOptions.None).Count() - 1;
                ProgressBarManager.ChangeProgressBarValue(50 / _count);
            }
        }

        public void GetSources()
        {
            Status = Models.Status.GettingSources;
            foreach (var page in ListOfPages)
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(page.URL);
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

                using (var sr = new StreamReader(resp.GetResponseStream(), Encoding.GetEncoding("windows-1251")))
                    page.SourceCode = sr.ReadToEnd();

                ProgressBarManager.ChangeProgressBarValue(50 / _count);
            }
        }

        private void ChangeSlider(object sender, NewProgressBarValueEventsArgs e)
        {
            var newValue = e.NewValue;
            ItemProgressValue += newValue;
            ProgressBarValue = ItemProgressValue;
        }
    }
}
