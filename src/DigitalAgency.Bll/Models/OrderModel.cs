using System;
using System.Collections.Generic;
using DigitalAgency.Bll.AutoMapper;
using DigitalAgency.Dal.Entities.Enums;

namespace DigitalAgency.Bll.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        
        public virtual ClientModel Client { get; set; }
        public virtual ExecutorModel Executor { get; set; }
        public virtual ProjectModel Project { get; set; }
        
        public DateTime CreationDate { get; set; }
        public DateTime ScheduledTime { get; set; }
        public OrderStateEnum StateEnum { get; set; }
        
        public List<TaskModel> Tasks { get; set; }
    }
}