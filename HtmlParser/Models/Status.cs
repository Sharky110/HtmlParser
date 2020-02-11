using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlParser.Models
{
    class Status
    {
        const string status = "Статус: ";

        public static string Ready { get => status + "к работе готов."; }
        public static string GettingSources { get => status + "получение исходных кодов страниц."; }
        public static string CountTags { get => status + "подсчет тегов."; }
        public static string Done { get => status + "подсчет завершен."; }
    }
}
