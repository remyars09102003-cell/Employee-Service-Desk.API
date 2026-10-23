using FluentValidation;
using ServiceDesk.Application.Tickets.Dtos;

namespace ServiceDesk.Application.Tickets.Validators;

public class CreateTicketRequestValidator : AbstractValidator<CreateTicketRequest>
{
    public CreateTicketRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.ServiceItemId).GreaterThan(0);
        RuleFor(x => x.Priority).IsInEnum();
        RuleFor(x => x.AssetId).GreaterThan(0).When(x => x.AssetId.HasValue);
    }
}

public class UpdateTicketRequestValidator : AbstractValidator<UpdateTicketRequest>
{
    public UpdateTicketRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.Priority).IsInEnum();
    }
}

public class TicketQueryValidator : AbstractValidator<TicketQuery>
{
    public TicketQueryValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Priority).IsInEnum();
        RuleFor(x => x.Search).MaximumLength(100);
        RuleFor(x => x).Must(q => !q.CreatedFrom.HasValue || !q.CreatedTo.HasValue || q.CreatedFrom <= q.CreatedTo)
            .WithMessage("CreatedFrom must be on or before CreatedTo.");
    }
}
