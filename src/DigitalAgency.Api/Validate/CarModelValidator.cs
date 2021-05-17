using DigitalAgency.Dal.Entities;
using FluentValidation;

namespace DigitalAgency.Api.Validate
{
    public class CarModelValidator : AbstractValidator<Project>
    {
        public CarModelValidator()
        {
            RuleFor(x => x.ProjectDescription)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(30);
            RuleFor(x => x.ProjectName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(30);
            RuleFor(x => x.ProjectLink)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(15);
        }
    }
}
