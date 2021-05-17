using System.Linq;
using FluentValidation;

namespace DigitalAgency.Api.Validate
{
    public class ClientModelValidator : AbstractValidator<DigitalAgency.Dal.Entities.Client>
    {
        public ClientModelValidator()
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

        public bool IsPhoneValid(string PhoneNumber)
        {
            return !(!PhoneNumber.StartsWith("+")|| !PhoneNumber.Substring(1).All(c => char.IsDigit(c)));
        }
    }
}
