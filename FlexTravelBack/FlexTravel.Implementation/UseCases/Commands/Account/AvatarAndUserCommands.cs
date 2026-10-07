using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.Exceptions;
using FlexTravel.Application.Files;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.DataAccess;
using FlexTravel.Implementation.Validators;
using FluentValidation;

namespace FlexTravel.Implementation.UseCases.Commands.Account
{
    public class EfChangeMyAvatarCommand : EfUseCase, IChangeMyAvatarCommand
    {
        private readonly IUser _user;
        private readonly IFileStorage _storage;
        private readonly UploadImageValidator _validator;

        public EfChangeMyAvatarCommand(FlexTravelContext context, IUser user, IFileStorage storage, UploadImageValidator validator)
            : base(context)
        {
            _user = user;
            _storage = storage;
            _validator = validator;
        }

        public int UseCaseId => UseCaseIds.ChangeMyAvatar;
        public string UseCaseName => "Change my avatar";
        public string UseCaseDescription => "Upload or remove own profile photo";

        public string Execute(UploadImageDto request)
        {
            var user = Context.User.FirstOrDefault(u => u.UserId == _user.Id)
                ?? throw new EntityNotFoundException("User", _user.Id);

            if (request?.Content == null || request.Content.Length == 0)
            {
                user.Avatar = null;
                Context.SaveChanges();
                return null;
            }

            _validator.ValidateAndThrow(request);
            user.Avatar = _storage.Save(UploadImageValidator.Extension(request), request.Content);
            Context.SaveChanges();
            return user.Avatar;
        }
    }

    public class EfRemoveUserAvatarCommand : EfUseCase, IRemoveUserAvatarCommand
    {
        public EfRemoveUserAvatarCommand(FlexTravelContext context) : base(context)
        {
        }

        public int UseCaseId => UseCaseIds.RemoveUserAvatar;
        public string UseCaseName => "Remove user avatar";
        public string UseCaseDescription => "Admin removes an unsuitable profile photo";

        public void Execute(int userId)
        {
            var user = Context.User.FirstOrDefault(u => u.UserId == userId)
                ?? throw new EntityNotFoundException("User", userId);

            user.Avatar = null;
            Context.SaveChanges();
        }
    }

    public class EfUpdateUserCommand : EfUseCase, IUpdateUserCommand
    {
        private readonly UpdateUserValidator _validator;

        public EfUpdateUserCommand(FlexTravelContext context, UpdateUserValidator validator) : base(context)
        {
            _validator = validator;
        }

        public int UseCaseId => UseCaseIds.UpdateUser;
        public string UseCaseName => "Update user";
        public string UseCaseDescription => "Admin corrects a user's name, e-mail, phone and address";

        public void Execute(UpdateUserDto request)
        {
            var user = Context.User.FirstOrDefault(u => u.UserId == request.UserId)
                ?? throw new EntityNotFoundException("User", request.UserId);

            request.Email = request.Email?.Trim().ToLowerInvariant();
            _validator.ValidateAndThrow(request);

            user.FirstName = request.FirstName.Trim();
            user.LastName = request.LastName.Trim();
            user.Email = request.Email;
            user.Phone = request.Phone.Trim();
            user.Address = request.Address.Trim();
            user.City = request.City.Trim();
            Context.SaveChanges();
        }
    }

    public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserValidator(FlexTravelContext context)
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Enter the first name.").MaximumLength(50);
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Enter the last name.").MaximumLength(50);
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Enter the e-mail address.")
                .EmailAddress().WithMessage("Enter a valid e-mail address.")
                .MaximumLength(254)
                .Must((dto, email) => !context.User.Any(u => u.Email == email && u.UserId != dto.UserId))
                .WithMessage("Another account already uses this e-mail address.");
            RuleFor(x => x.Phone).NotEmpty().WithMessage("Enter a phone number.").MaximumLength(20);
            RuleFor(x => x.Address).NotEmpty().WithMessage("Enter the address.").MaximumLength(100);
            RuleFor(x => x.City).NotEmpty().WithMessage("Enter the city.").MaximumLength(50);
        }
    }
}
