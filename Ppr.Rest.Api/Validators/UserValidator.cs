using FluentValidation;
using Ppr.Rest.Api.Models;

namespace Ppr.Rest.Api.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(user => user.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(user => user.Email).NotEmpty().WithMessage("Email is required.")
                                       .EmailAddress().WithMessage("Email is not valid.");
            RuleFor(user => user.Password).NotEmpty().WithMessage("Password is required.")
                                          .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        }
    }
}
