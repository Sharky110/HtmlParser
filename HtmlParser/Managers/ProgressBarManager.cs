using HtmlParser.EventArgs;

namespace HtmlParser.Managers
{
    class ProgressBarManager
    {
        public delegate void ProgressBarManagerHandler(object sender, NewProgressBarValueEventsArgs e);
        public event ProgressBarManagerHandler Increase;

        private static readonly ProgressBarManager _instance;

        private ProgressBarManager() {}

        public static ProgressBarManager Instance
        {
            get => _instance ?? new ProgressBarManager();
        }

        public void IncreaseProgress(double newValue)
        {
            Increase?.Invoke(this, new NewProgressBarValueEventsArgs(newValue));
        }
    }
}
