using System.Collections.Generic;

namespace DigitalAgency.Bll.Models
{
    public class ProjectModel
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public string ProjectLink { get; set; }
        
        public ClientModel Client { get; set; }
        public List<OrderModel> Orders { get; set; }
    }
}