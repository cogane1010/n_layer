using FluentValidation;
using App.BookingOnline.Api.ViewModels;

namespace App.BookingOnline.Api.Validators
{
    public class SaveArtistResourceValidator : AbstractValidator<SaveArtistResource>
    {
        public SaveArtistResourceValidator()
        {
            RuleFor(a => a.Name)
                .NotEmpty()
                .MaximumLength(50);
        }
    }
}