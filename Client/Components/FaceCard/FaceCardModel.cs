using BlazorFaceRecog.Shared;
using FluentValidation;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorFaceRecog.Client.Components.FaceCard;

public class FaceCardModel
{
    public string? Name { get; set; }
    public IBrowserFile? File { get; set; }
    public FaceCardState State { get; set; } = FaceCardState.Unselected;
}

public class FaceModelFluentValidator : AbstractValidator<FaceCardModel>
{
    public FaceModelFluentValidator()
    {
        RuleFor(y => y.Name)
            .NotNull()
            .NotEmpty();

        RuleFor(y => y.File)
            .NotNull()
            .NotEmpty();
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<FaceCardModel>.CreateWithOptions((FaceCardModel)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return [];

        return result.Errors.Select(e => e.ErrorMessage);
    };
}
