using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.ViewModels.Systems.Users
{
    public class LoginRequestValidation : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidation()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is Required!");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is Required!")
                .MinimumLength(6).WithMessage("Password is at least 6 characters!");
        }
    }
}