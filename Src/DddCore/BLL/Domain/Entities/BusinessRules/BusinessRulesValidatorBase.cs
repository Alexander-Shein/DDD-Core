using System;
using System.Threading.Tasks;
using DddCore.Contracts.BLL.Domain.Entities.BusinessRules;
using DddCore.Contracts.BLL.Domain.Entities.Model;
using DddCore.Contracts.BLL.Errors;
using FluentValidation;
using FluentValidation.Results;

namespace DddCore.BLL.Domain.Entities.BusinessRules
{
    public abstract class BusinessRulesValidatorBase<T> : AbstractValidator<T>, IBusinessRulesValidator<T> where T : ICrudState
    {
        #region Public Methods

        public async Task<OperationResult> ValidateAsync(T instance)
        {
            var validationResult = await base.ValidateAsync(instance);
            return Map(validationResult);
        }

        public new OperationResult Validate(T instance)
        {
            var validationResult = base.Validate(instance);
            return Map(validationResult);
        }

        #endregion

        #region Private Methods

        OperationResult Map(ValidationResult validationResult)
        {
            if (validationResult.IsValid) return OperationResult.Succeed;

            var result = new OperationResult();

            foreach (var validationFailure in validationResult.Errors)
            {
                result.Errors.Add(new Error
                {
                    Code = Int32.Parse(validationFailure.ErrorCode),
                    Description = validationFailure.ErrorMessage
                });
            }

            return result;
        }

        #endregion
    }
}