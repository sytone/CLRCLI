using System;
using System.Timers;

namespace CLRCLI.Widgets
{
    public class Label : Widget
    {

        public int AutoUpdateSchedule { get; set; }
        public Func<string> UpdateAction { get; set; }
        private Timer timer;
        internal Label()
        {
        }

        public Label(Widget parent) : base(parent)
        {
            Background = parent.Background;
            Foreground = ConsoleColor.White;
            AutoUpdateSchedule = 0;
        }

        

        public void StartUpdate()
        {
            if (AutoUpdateSchedule != 0)
            {
                timer = new Timer(AutoUpdateSchedule);
                timer.Elapsed += timer_Elapsed;
                timer.Start();
            }
        }

        public void StopUpdate()
        {
            timer.Stop();
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //this.Text = DateTime.Now.ToLongTimeString()
            try
            {
                this.Text = (string)UpdateAction?.Invoke();
            }
            catch (System.Exception ex)
            {
                this.Text = ex.Message;
            }
            
            Draw();
        }

        internal override void Render()
        {
            var lines = Text.Split(new string[] { "\n" }, StringSplitOptions.None);

            for (var i = 0; i < lines.Length; i++)
            {
                ConsoleHelper.DrawText(DisplayLeft, DisplayTop + i, Foreground, Background, lines[i]);
            }
        }
    }
}