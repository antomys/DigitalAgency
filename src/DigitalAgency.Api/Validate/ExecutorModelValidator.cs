using System.Linq;
using DigitalAgency.Bll.Models;
using FluentValidation;

namespace DigitalAgency.Api.Validate;

public class ExecutorModelValidator : AbstractValidator<ExecutorModel>
{
    public ExecutorModelValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(20);
        RuleFor(x => x.MiddleName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(20);
        RuleFor(x => x.LastName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(20);
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MinimumLength(11)
            .MaximumLength(15);
        RuleFor(x => x.PhoneNumber)
            .Must(IsPhoneValid)
            .WithMessage("Not valid phone number");
    }

    static bool IsPhoneValid(string phoneNumber)
    {
        return !(!phoneNumber.StartsWith("+") || !phoneNumber.Substring(1).All(char.IsDigit));
    }
}