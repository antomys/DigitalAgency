using System.Linq;
using DigitalAgency.Bll.DTOs;
using FluentValidation;

namespace DigitalAgency.Api.Validate
{
    public class ServiceOrderValidator : AbstractValidator<OrderDTO>
    {
        public ServiceOrderValidator()
        {
            RuleFor(x => x.CarColor)
           .NotEmpty()
           .MinimumLength(3)
           .MaximumLength(30);
            RuleFor(x => x.CarMake)
           .NotEmpty()
           .MinimumLength(2)
           .MaximumLength(30);
            RuleFor(x => x.CarPlate)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(12);
            RuleFor(x => x.CarModel)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(30);
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
            RuleFor(x => x.ClientPhone)
            .NotEmpty()
            .MinimumLength(11)
            .MaximumLength(15);
            RuleFor(x => x.ClientPhone)
             .Must(IsPhoneValid)
             .WithMessage("Not valid phone number");
        }

        public bool IsPhoneValid(string PhoneNumber)
        {
            return !(!PhoneNumber.StartsWith("+") || !PhoneNumber.Substring(1).All(c => char.IsDigit(c)));
        }
    }
}

