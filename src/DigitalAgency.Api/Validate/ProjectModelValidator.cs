using DigitalAgency.Bll.Models;
using FluentValidation;

namespace DigitalAgency.Api.Validate
{
    public class ProjectModelValidator : AbstractValidator<ProjectModel>
    {
        public ProjectModelValidator()
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
