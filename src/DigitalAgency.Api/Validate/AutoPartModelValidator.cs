using DigitalAgency.Dal.Entities;
using FluentValidation;

namespace DigitalAgency.Api.Validate
{
    public class AutoPartModelValidator : AbstractValidator<Task>
    {
        public AutoPartModelValidator()
        {
            RuleFor(x => x.DaysDeadline)
            .NotEmpty();
            RuleFor(x => x.Name)
            .NotEmpty();
            RuleFor(x => x.State)
            .NotEmpty();

        }
    }
}
