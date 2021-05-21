using System.Collections.Generic;

namespace DigitalAgency.Bll.TelegramBot
{
    public class BotConfiguration
    {
        public string BotToken { get; set; }
        public IEnumerable<string> ClientMenu { get; set; }
        public IEnumerable<string> ExecutorMenu { get; set; }
    }
}