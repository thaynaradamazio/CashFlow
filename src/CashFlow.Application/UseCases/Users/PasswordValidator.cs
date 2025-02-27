﻿using CashFlow.Exception;
using FluentValidation;
using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace CashFlow.Application.UseCases.Users
{
    public partial class PasswordValidator<T> : PropertyValidator<T, string>
    {
        private const string ERROR_MESSAGE_KEY = "ErrorMessage";
        public override string Name => "PasswordValidator";

        public override bool IsValid(ValidationContext<T> context, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                context.MessageFormatter.AppendArgument(ERROR_MESSAGE_KEY, ResourceErrorMessages.INVALID_PASSWORD);
                return false;
            }

            if(password.Length < 8)
            {
                context.MessageFormatter.AppendArgument(ERROR_MESSAGE_KEY, ResourceErrorMessages.INVALID_PASSWORD);
                return false;
            }

            if (MyRegex().IsMatch(password) == false)
            {
                context.MessageFormatter.AppendArgument(ERROR_MESSAGE_KEY, ResourceErrorMessages.INVALID_PASSWORD);
                return false;
            }

            if (Regex.IsMatch(password, @"[A-Z]+") == false)
            {
                context.MessageFormatter.AppendArgument(ERROR_MESSAGE_KEY, ResourceErrorMessages.INVALID_PASSWORD);
                return false;
            }

            if (Regex.IsMatch(password, @"[0-9]+") == false)
            {
                context.MessageFormatter.AppendArgument(ERROR_MESSAGE_KEY, ResourceErrorMessages.INVALID_PASSWORD);
                return false;
            }

            if (Regex.IsMatch(password, @"[\!\?\*\.]+") == false)
            {
                context.MessageFormatter.AppendArgument(ERROR_MESSAGE_KEY, ResourceErrorMessages.INVALID_PASSWORD);
                return false;
            }

            return true;
        }

        [GeneratedRegex(@"[a-z]+")]
        private static partial Regex MyRegex();
    }
}
