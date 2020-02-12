namespace HtmlParser.Models
{
    class Status
    {
        const string status = "Статус: ";

        public static string Ready { get => status + "к работе готов."; }
        public static string GettingSources { get => status + "получение исходных кодов страниц."; }
        public static string CountTags { get => status + "подсчет тегов."; }
        public static string SettingMaxTagAmount { get => status + "выделение максимального количество тегов."; }
        public static string TaskPaused { get => status + "задача приостановлена."; }
        public static string TaskAborted { get => status + "задача отменена."; }
        public static string Done { get => status + "подсчет завершен."; }

        public static bool IsWorking(string status)
        {
            if (status == GettingSources || status == CountTags || status == SettingMaxTagAmount)
                return true;
            return false;
        }
    }
}
