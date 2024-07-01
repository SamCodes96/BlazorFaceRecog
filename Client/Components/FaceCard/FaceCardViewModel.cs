using FluentValidation;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorFaceRecog.Client.Components.FaceCard;

public class FaceCardViewModel
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string? Name { get; set; }
    public IBrowserFile? SelectedFile { get; set; }
    public string? Thumbnail { get; set; }
    public CardState State { get; set; }
}

public class FaceModelFluentValidator : AbstractValidator<FaceCardViewModel>
{
    public FaceModelFluentValidator()
    {
        When(x => x.State != CardState.Deleted, () =>
            RuleFor(x => x.Name)
                .NotEmpty());

        When(x => x.State == CardState.Unsaved, () =>
            RuleFor(y => y.SelectedFile)
                .NotNull()
                .WithMessage("A face is required"));

        RuleFor(x => x.Thumbnail)
            .NotEmpty();
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<FaceCardViewModel>.CreateWithOptions((FaceCardViewModel)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return [];

        return result.Errors.Select(e => e.ErrorMessage);
    };
}
