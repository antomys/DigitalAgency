using System;
using System.Collections.Generic;
using DigitalAgency.Bll.Models.Enums;

namespace DigitalAgency.Bll.Models;

public class OrderModel
{
    public int Id { get; set; }
    public virtual ClientModel Client { get; set; }
    public virtual ExecutorModel Executor { get; set; }
    public virtual ProjectModel Project { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public DateTimeOffset ScheduledTime { get; set; }
    public OrderStateEnum StateEnum { get; set; }

    public List<CardModel> Tasks { get; set; }
}