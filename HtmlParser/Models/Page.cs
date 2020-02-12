using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HtmlParser.Models
{
    class Page : INotifyPropertyChanged, IComparable
    {
        private string _url;
        private int _amountOfTags;
        private bool _isBiggest;
        public bool IsBiggest
        {
            get { return _isBiggest; }
            set
            {
                _isBiggest = value;
                OnPropertyChanged("IsBiggest");
            }
        }
        public string URL 
        {
            get { return _url; }
            set
            {
                _url = value;
                OnPropertyChanged("URL");
            }
        }
        public int AmountOfTags
        {
            get { return _amountOfTags; }
            set
            {
                _amountOfTags = value;
                OnPropertyChanged("AmountOfTags");
            }
        }
        public string SourceCode { get; set; }

        public Page(string url)
        {
            URL = url;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public int CompareTo(object obj)
        {
            if (obj == null) 
                return 1;
            return AmountOfTags.CompareTo((obj as Page).AmountOfTags);
        }
    }
}
