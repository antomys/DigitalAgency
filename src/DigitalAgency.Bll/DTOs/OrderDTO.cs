using System;

namespace DigitalAgency.Bll.DTOs
{
    public class OrderDTO
    {
        //create
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string ClientPhone { get; set; }
        public string CarMake { get; set; }
        public string CarModel { get; set; }
        public string CarPlate { get; set; }
        public string CarColor { get; set; }
        public DateTime ScheduledDate { get; set; }
    }
}
