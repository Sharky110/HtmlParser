using HtmlParser.Commands;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
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
        public ICommand SaveFileCommand { get; }

        #endregion

        #region Private variables

        private List<string> _listOfSites;
        private string _filePath;

        #endregion

        #region Properties

        public List<string> ListOfSites
        {
            get => _listOfSites;
            set => SetProperty(ref _listOfSites, value);
        }

        public string FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }

        #endregion

        public HtmlParserVM()
        {
            OpenFileCommand = new RelayCommand(c => OpenFile());
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

            ListOfSites = sourceFile.Split('\n').ToList();
        }
    }
}
