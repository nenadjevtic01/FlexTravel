using FlexTravel.Application.Dto;
using FlexTravel.DataAccess;
using FluentValidation;

namespace FlexTravel.Implementation.Validators
{
    public class SaveCountryValidator : AbstractValidator<SaveCountryDto>
    {
        public SaveCountryValidator(FlexTravelContext context)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Enter the name of the country.")
                .MaximumLength(60)
                .Must((x, name) => !context.Country.Any(c => c.CountryName == name.Trim() && c.CountryId != x.Id))
                .When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage("This country already exists.");
            RuleFor(x => x.ContinentId)
                .Must(id => context.Continent.Any(c => c.ContinentId == id))
                .WithMessage("Choose a continent.");
            RuleFor(x => x.CapitalCity).MaximumLength(60);
            RuleFor(x => x.Currency).MaximumLength(50);
            RuleFor(x => x.Language).MaximumLength(60);
            RuleFor(x => x.Population).GreaterThan(0).When(x => x.Population.HasValue);
            RuleFor(x => x.TotalArea).GreaterThan(0).When(x => x.TotalArea.HasValue);

            When(x => !string.IsNullOrWhiteSpace(x.VisaType), () =>
            {
                RuleFor(x => x.VisaType).MaximumLength(100);
                RuleFor(x => x.VisaDuration).NotEmpty().WithMessage("Enter how long the visa is valid.").MaximumLength(100);
                RuleFor(x => x.VisaRequirements).NotEmpty().WithMessage("Describe the visa requirements.").MaximumLength(2000);
            });

            When(x => x.SafetyLevel.HasValue, () =>
            {
                RuleFor(x => x.SafetyLevel).InclusiveBetween(1, 5).WithMessage("Safety level is from 1 (risky) to 5 (very safe).");
                RuleFor(x => x.SafetyAdvices).NotEmpty().WithMessage("Add safety advice for travelers.").MaximumLength(1000);
            });
        }
    }

    public class SaveDestinationValidator : AbstractValidator<SaveDestinationDto>
    {
        public const int MaxImages = 12;

        public SaveDestinationValidator(FlexTravelContext context)
        {
            RuleFor(x => x.CountryId)
                .Must(id => context.Country.Any(c => c.CountryId == id))
                .WithMessage("Choose a country.");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Enter the name of the destination.")
                .MaximumLength(60)
                .Must((x, name) => !context.Destination.Any(d => d.CountryId == x.CountryId
                    && d.DestinationName == name.Trim() && d.DestinationId != x.Id))
                .When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage("This destination already exists in the country.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Describe the destination.").MaximumLength(1000);
            RuleFor(x => x.Climate).MaximumLength(1000);
            RuleFor(x => x.Latitude).InclusiveBetween(-90, 90).When(x => x.Latitude.HasValue);
            RuleFor(x => x.Longitude).InclusiveBetween(-180, 180).When(x => x.Longitude.HasValue);
            RuleFor(x => x)
                .Must(x => x.Latitude.HasValue == x.Longitude.HasValue)
                .WithName("Latitude")
                .WithMessage("Set both coordinates, or neither.");
            RuleFor(x => x.Images)
                .Must(images => images == null || images.Count <= MaxImages)
                .WithMessage($"At most {MaxImages} images.");
            RuleForEach(x => x.Images).ChildRules(image =>
            {
                image.RuleFor(i => i.Src)
                    .NotEmpty()
                    .MaximumLength(255)
                    .Must(src => !src.Contains("..") && !src.Contains("://") && !src.StartsWith("/"))
                    .WithMessage("Invalid image path.");
                image.RuleFor(i => i.Alt).MaximumLength(100);
            });

            RuleForEach(x => x.Hotels).SetValidator(new HotelFieldsValidator(context));
            RuleForEach(x => x.Sights).SetValidator(new SightFieldsValidator(context));
            RuleFor(x => x.Hotels)
                .Must(hotels => hotels.Select(h => h.Name?.Trim()).Distinct().Count() == hotels.Count)
                .When(x => x.Hotels != null)
                .WithMessage("Two hotels have the same name.");
            RuleFor(x => x.Sights)
                .Must(sights => sights.Select(s => s.Name?.Trim()).Distinct().Count() == sights.Count)
                .When(x => x.Sights != null)
                .WithMessage("Two sights have the same name.");
        }
    }

    public class SaveHotelValidator : AbstractValidator<SaveHotelDto>
    {
        public SaveHotelValidator(FlexTravelContext context)
        {
            RuleFor(x => x.DestinationId)
                .Must(id => context.Destination.Any(d => d.DestinationId == id && d.IsActive))
                .WithMessage("Choose a destination.");
            Include(new HotelFieldsValidator(context));
            RuleFor(x => x.Name)
                .Must((x, name) => !context.Accommodation.Any(a => a.DestinationId == x.DestinationId
                    && a.AccommodationName == name.Trim() && a.AccommodationId != x.Id))
                .When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage("This destination already has a hotel with that name.");
        }
    }

    public class HotelFieldsValidator : AbstractValidator<SaveHotelDto>
    {
        public HotelFieldsValidator(FlexTravelContext context)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Enter the name of the hotel.").MaximumLength(100);
            RuleFor(x => x.TypeId)
                .Must(id => context.AccommodationsType.Any(t => t.AccommodationTypeId == id))
                .WithMessage("Choose the type of accommodation.");
            RuleFor(x => x.Stars)
                .InclusiveBetween(1, 5)
                .Must(stars => stars * 2 == Math.Floor(stars * 2))
                .WithMessage("Stars are from 1 to 5, in steps of 0.5.");
            RuleFor(x => x.Rooms)
                .NotEmpty().WithMessage("Add at least one room category.")
                .Must(rooms => rooms.Any(r => r.NumberOfRooms > 0)).WithMessage("At least one room category needs rooms.")
                .Must(rooms => rooms.Select(r => r.RoomTypeId).Distinct().Count() == rooms.Count)
                .WithMessage("Each room type can be listed only once.");
            RuleForEach(x => x.Rooms).ChildRules(room =>
            {
                room.RuleFor(r => r.RoomTypeId)
                    .Must(id => context.RoomType.Any(t => t.RoomTypeId == id))
                    .WithMessage("Choose a room type.");
                room.RuleFor(r => r.NumberOfRooms).InclusiveBetween(0, 1000);
                room.RuleFor(r => r.PricePerNight)
                    .GreaterThan(0).WithMessage("Every room category needs a price per night.")
                    .LessThan(100000);
            });
        }
    }

    public class SaveSightValidator : AbstractValidator<SaveSightDto>
    {
        public SaveSightValidator(FlexTravelContext context)
        {
            RuleFor(x => x.DestinationId)
                .Must(id => context.Destination.Any(d => d.DestinationId == id))
                .WithMessage("Choose a destination.");
            Include(new SightFieldsValidator(context));
            RuleFor(x => x.Name)
                .Must((x, name) => !context.Attraction.Any(a => a.DestinationId == x.DestinationId
                    && a.AttractionName == name.Trim() && a.AttractionId != x.Id))
                .When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage("This destination already has a sight with that name.");
        }
    }

    public class SightFieldsValidator : AbstractValidator<SaveSightDto>
    {
        public SightFieldsValidator(FlexTravelContext context)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Enter the name of the sight.").MaximumLength(100);
            RuleFor(x => x.Description).NotEmpty().WithMessage("Describe the sight.").MaximumLength(1000);
            RuleFor(x => x.CategoryId)
                .Must(id => context.AttractionCategory.Any(c => c.AttractionCategoryId == id))
                .WithMessage("Choose a category.");
        }
    }

    public class UploadImageValidator : AbstractValidator<UploadImageDto>
    {
        public const int MaxBytes = 5 * 1024 * 1024;

        public UploadImageValidator()
        {
            RuleFor(x => x.Content)
                .Must(c => c != null && c.Length > 0).WithMessage("Choose an image.")
                .Must(c => c == null || c.Length <= MaxBytes).WithMessage("The image can be at most 5 MB.");
            RuleFor(x => x)
                .Must(x => Extension(x) != null)
                .When(x => x.Content != null && x.Content.Length > 0)
                .WithName("Content")
                .WithMessage("Only JPEG, PNG and WebP images can be uploaded.");
        }

        public static string Extension(UploadImageDto image)
        {
            var c = image.Content;
            if (c == null || c.Length < 12)
            {
                return null;
            }

            if (c[0] == 0xFF && c[1] == 0xD8 && c[2] == 0xFF)
            {
                return ".jpg";
            }

            if (c[0] == 0x89 && c[1] == 0x50 && c[2] == 0x4E && c[3] == 0x47)
            {
                return ".png";
            }

            if (c[0] == 'R' && c[1] == 'I' && c[2] == 'F' && c[3] == 'F' && c[8] == 'W' && c[9] == 'E' && c[10] == 'B' && c[11] == 'P')
            {
                return ".webp";
            }

            return null;
        }
    }
}
