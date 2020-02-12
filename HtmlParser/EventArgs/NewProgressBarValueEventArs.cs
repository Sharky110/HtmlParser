namespace HtmlParser.EventArgs
{
    class NewProgressBarValueEventsArgs : System.EventArgs
    {
        public double NewValue { get; }

        public NewProgressBarValueEventsArgs(double newValue)
        {
            NewValue = newValue;
        }
    }
}
