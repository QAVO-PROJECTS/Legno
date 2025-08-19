using FluentValidation;
using System;

namespace Legno.Application.Dtos.Contact
{
    public class CreateContactDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
    }

    public class CreateContactDtoValidator : AbstractValidator<CreateContactDto>
    {
        public CreateContactDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name cannot be empty.");

            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage("Surname cannot be empty.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email cannot be empty.")
                .EmailAddress().WithMessage("Email must be valid.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number cannot be empty.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description cannot be empty.");
        }
    }
}
