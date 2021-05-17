using System;
using System.Collections.Generic;
using System.Globalization;
using DigitalAgency.Dal.Entities;

namespace DigitalAgency.Bll.Models
{
    public class ServiceOrderModel
    {
        public int Id { get; set; }
        public Client Client { get; set; }
        public Client Mechanic { get; set; }
        public Project Project { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ScheduledTime { get; set; }
        public string State { get; set; }
        public List<OrderTask> OrderParts { get; set; }

        public override string ToString()
        {
            return $"Car: {Project.ProjectName +" "+ Project.ProjectDescription}\n" +
                   $"Mechanic: {Mechanic.FirstName+" "+Mechanic.LastName}\n" +
                   $"Creation Date: {CreationDate.ToString(CultureInfo.CurrentCulture)}\n" +
                   $"Scheduled Time: {ScheduledTime.ToString(CultureInfo.CurrentCulture)}\n" +
                   $"Order state: {State}";
        }
    }
}