using System;
using System.Windows.Forms;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;

namespace MSaleOrder.X_Class
{
    public class cTextBoxAppender : AppenderSkeleton
    {
        public TextBox otbLogTextBox { get; set; }
        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            try
            {
                if (otbLogTextBox != null)
                {
                    Action operation = () => { this.otbLogTextBox.AppendText(RenderLoggingEvent(loggingEvent)); };
                    this.otbLogTextBox.Invoke(operation);
                }
            }
            catch (Exception)
            {

            }
        }             
    }
}
