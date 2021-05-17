namespace DigitalAgency.Bll.Models
{
    public class CarModel
    {
        public int Id { get; set; }
        public string Makes { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public string LicensePlate { get; set; }
        
        public override string ToString()
        {
            return $"Manufacturer: {Makes}\n" +
                   $"Model: {Model}\n" +
                   $"Color: {Color}\n" +
                   $"Licence Plate: {LicensePlate}";
        }
    }
}