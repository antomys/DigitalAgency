using System.Collections.Generic;

namespace DigitalAgency.Bll
{
    public class BotConfiguration
    {
        public string BotToken { get; set; }
        public IEnumerable<string> CommandsClient { get; set; }
        public IEnumerable<string> CommandsMechanic { get; set; }
    }
}