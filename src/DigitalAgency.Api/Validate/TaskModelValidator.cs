﻿using DigitalAgency.Bll.Models;
using FluentValidation;

namespace DigitalAgency.Api.Validate;

public class TaskModelValidator : AbstractValidator<CardModel>
{
    public TaskModelValidator()
    {
        RuleFor(x => x.DaysDeadline)
            .NotEmpty();
        RuleFor(x => x.Name)
            .NotEmpty();
        RuleFor(x => x.StateEnum)
            .NotEmpty();
    }
}