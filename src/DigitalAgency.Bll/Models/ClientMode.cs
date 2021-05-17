namespace DigitalAgency.Bll.Models
{
    public class ClientModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsMechanic { get; set; }
        public long TelegramId { get; set; }
        public long ChatId { get; set; }

        public override string ToString()
        {
            return $"First Name: {FirstName}\n" +
                   $"Last Name: {LastName}\n" +
                   $"Phone Number: {PhoneNumber}\n" +
                   $"Telegram Nickname: {MiddleName}";
        }
    }
}