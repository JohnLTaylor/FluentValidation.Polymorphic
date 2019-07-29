using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FluentValidation.Validators.Polymorphic
{
    public class PolymorphicValidator<TBase>
        : AbstractValidator<TBase>
    {
        readonly Dictionary<Type, IValidator> _validators = new Dictionary<Type, IValidator>();

        public PolymorphicValidator<TBase> Add<TDerived>(IValidator<TDerived> validator)
            where TDerived : TBase
        {
            _validators[typeof(TDerived)] = validator;
            return this;
        }

        public PolymorphicValidator<TBase> Add<TDerived, TDerivedValidator>()
            where TDerived : TBase
            where TDerivedValidator : IValidator<TDerived>, new()
        {
            return Add(new TDerivedValidator());
        }

        public override Task<ValidationResult> ValidateAsync(ValidationContext<TBase> context, CancellationToken cancellation = default)
        {

            if (context.InstanceToValidate == null || !_validators.TryGetValue(context.InstanceToValidate.GetType(), out var validator))
            {
                return Task.FromResult(new ValidationResult());
            }

            return validator.ValidateAsync(context, cancellation);
        }

    }
}
