using System.ComponentModel;

namespace MSystemSimulationEngine.Classes.Tools
{
    /// <summary>
    /// Notify message is used as a tool for distributing messages from inside of simulator.
    /// </summary>
    public class NotificationMessage : INotifyPropertyChanged
    {
        private string v_Message;

        /// <summary>
        /// Notification message.
        /// </summary>
        public string Message
        {
            get
            {
                return v_Message;
            }
            set
            {
                v_Message = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Message"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged(this, e);
        }
    }
}
