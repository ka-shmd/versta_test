using FluentValidation;
using VerstaDelivery.Api.DTOs;

namespace VerstaDelivery.Api.Validation;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.SenderCity).Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ValidationMessages.FieldEmptyMessage)
            .MaximumLength(100).WithMessage(ValidationMessages.FieldTooLongMessage);

        RuleFor(x => x.SenderAddress).Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ValidationMessages.FieldEmptyMessage)
            .MaximumLength(255).WithMessage(ValidationMessages.FieldTooLongMessage);

        RuleFor(x => x.RecipientCity).Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ValidationMessages.FieldEmptyMessage)
            .MaximumLength(100).WithMessage(ValidationMessages.FieldTooLongMessage);

        RuleFor(x => x.RecipientAddress).Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ValidationMessages.FieldEmptyMessage)
            .MaximumLength(255).WithMessage(ValidationMessages.FieldTooLongMessage);

        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("Вес груза должен быть больше нуля");

        RuleFor(x => x.PickupDate)
            .Must(x => x >= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Забор груза нельзя оформить задним числом");
    }
}
