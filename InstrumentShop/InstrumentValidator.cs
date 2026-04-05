using FluentValidation;
using InstrumentShop.Shared;

public class InstrumentValidator : AbstractValidator<InstrumentDto>
{
    public InstrumentValidator()
    {
        RuleFor(x => x.Name).NotEmpty().Length(3, 50);
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Cena mora biti veća od nule.");
    }
}
