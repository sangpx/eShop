using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.ViewModels.Systems.Users
{
    public class RegisterRequestValidation : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidation()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName is Required!");
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("FirstName is Required!")
                .MaximumLength(200).WithMessage("FirstName cannot over 200 characters!");
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("LastName is Required!")
                .MaximumLength(200).WithMessage("LastName cannot over 200 characters!");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Email format not match!");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("PhoneNumber is Required!");
            RuleFor(x => x.DateOfBirth).GreaterThan(DateTime.Now.AddYears(-100)).WithMessage("DateOfBirth cannot greater 100 years!");
            RuleFor(x => x.Password).MinimumLength(6).WithMessage("Password is at least 6 characters!");
            RuleFor(x => x).Custom((request, context) =>
            {
                if (request.Password != request.ConfirmPassword)
                {
                    context.AddFailure("Confirm Password is not match!");
                }
            });
        }
    }
}