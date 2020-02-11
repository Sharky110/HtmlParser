using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HtmlParser.Models
{
    class Page : INotifyPropertyChanged
    {
        private string _url;
        private int _amountOfTags;
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
    }
}
