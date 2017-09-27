using System;
using System.Collections.ObjectModel;

namespace Novadaq.UI.Model
{
    public class Session
    {
        public Session(string inputFolder)
        {
            Messages.Add(new Message()
            {
                Timestamp = DateTime.Now,
                Content = $"New session started with folder {inputFolder}"
            });
        }

        public ObservableCollection<Message> Messages { get; } = new ObservableCollection<Message>();
    }
}