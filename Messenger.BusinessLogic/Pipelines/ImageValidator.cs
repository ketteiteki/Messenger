using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Messenger.BusinessLogic.Pipelines;

public class ImageValidator : AbstractValidator<IFormFile>
{
    private readonly List<string> _allowedExtensions = new()
    {
        "jpg", "JPG", "jpeg", "JPEG", "png", "PNG"
    };

    public ImageValidator()
    {
        RuleFor(x => x.Length)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0)
            .LessThanOrEqualTo(4 * 1024 * 1024)
            .WithMessage("The image must be no larger than 4 MB.");

        RuleFor(x => x.FileName)
            .Cascade(CascadeMode.Stop)
            .Must(CheckExtension)
            .WithMessage($"Allowed extensions: {string.Join(", ", _allowedExtensions)}")
            .Length(1, 70);
    }

    private bool CheckExtension(string fileName)
    {
        var extension = fileName.Split(".")[^1];

        return _allowedExtensions.Contains(extension);
    }
}