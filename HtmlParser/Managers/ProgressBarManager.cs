using HtmlParser.EventArgs;

namespace HtmlParser.Managers
{
    class ProgressBarManager
    {
        public delegate void ProgressBarManagerHandler(object sender, NewProgressBarValueEventsArgs e);
        public event ProgressBarManagerHandler NewProgressBarValue;

        private static ProgressBarManager _instance;

        private ProgressBarManager() {}

        public static ProgressBarManager Instance
        {
            get => _instance ?? new ProgressBarManager();
        }

        public void ChangeProgressBarValue(double newValue)
        {
            NewProgressBarValue?.Invoke(this, new NewProgressBarValueEventsArgs(newValue));
        }
    }
}
