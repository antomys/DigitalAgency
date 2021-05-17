namespace DigitalAgency.Bll.Models
{
    public class AutoPartModel
    {
        public int Id { get; set; }
        public string PartName { get; set; }
        public decimal Price { get; set; }
        public string State { get; set; }

        public override string ToString()
        {
            return $"Part: {PartName}\n" +
                   $"Price: {Price}\n" +
                   $"State: {State}";
        }
    }
}