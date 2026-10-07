using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.Exceptions;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FluentValidation;
using FluentValidation.Results;

namespace FlexTravel.Implementation.UseCases.Commands.Account
{
    public class EfGetMyProfileQuery : EfUseCase, IGetMyProfileQuery
    {
        private readonly IUser _user;

        public EfGetMyProfileQuery(FlexTravelContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public int UseCaseId => UseCaseIds.GetMyProfile;
        public string UseCaseName => "Get my profile";
        public string UseCaseDescription => "Account details of the signed-in user";

        public ProfileDto Execute(int request)
        {
            var profile = Context.User
                .Where(u => u.UserId == _user.Id)
                .Select(u => new ProfileDto
                {
                    Id = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Phone = u.Phone,
                    Address = u.Address,
                    City = u.City,
                    Avatar = u.Avatar,
                    Role = u is Domain.Admin ? "Admin" : u is AgencyEmployee ? "AgencyEmployee" : "Customer",
                    Agency = u is AgencyEmployee ? ((AgencyEmployee)u).Agency.AgencyName : null,
                    MemberSince = u.CreatedAt
                })
                .FirstOrDefault();

            return profile ?? throw new EntityNotFoundException("User", _user.Id);
        }
    }

    public class UpdateProfileValidator : AbstractValidator<UpdateProfileDto>
    {
        public UpdateProfileValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Enter your first name.").MaximumLength(50);
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Enter your last name.").MaximumLength(50);
            RuleFor(x => x.Phone).NotEmpty().WithMessage("Enter a phone number.").MaximumLength(20);
            RuleFor(x => x.Address).NotEmpty().WithMessage("Enter your address.").MaximumLength(100);
            RuleFor(x => x.City).NotEmpty().WithMessage("Enter your city.").MaximumLength(50);
        }
    }

    public class EfUpdateMyProfileCommand : EfUseCase, IUpdateMyProfileCommand
    {
        private readonly IUser _user;
        private readonly UpdateProfileValidator _validator;

        public EfUpdateMyProfileCommand(FlexTravelContext context, IUser user, UpdateProfileValidator validator) : base(context)
        {
            _user = user;
            _validator = validator;
        }

        public int UseCaseId => UseCaseIds.UpdateMyProfile;
        public string UseCaseName => "Update my profile";
        public string UseCaseDescription => "Change own name, phone and address";

        public void Execute(UpdateProfileDto request)
        {
            _validator.ValidateAndThrow(request);

            var user = Context.User.FirstOrDefault(u => u.UserId == _user.Id)
                ?? throw new EntityNotFoundException("User", _user.Id);

            user.FirstName = request.FirstName.Trim();
            user.LastName = request.LastName.Trim();
            user.Phone = request.Phone.Trim();
            user.Address = request.Address.Trim();
            user.City = request.City.Trim();
            Context.SaveChanges();
        }
    }

    public class ChangePasswordValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("Enter your current password.");
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(8).WithMessage("The new password needs at least 8 characters.")
                .Matches("[0-9]").WithMessage("The new password must contain at least one digit.")
                .Matches("[A-Za-z]").WithMessage("The new password must contain at least one letter.")
                .NotEqual(x => x.CurrentPassword).WithMessage("The new password must be different from the current one.");
        }
    }

    public class EfChangePasswordCommand : EfUseCase, IChangePasswordCommand
    {
        private readonly IUser _user;
        private readonly ChangePasswordValidator _validator;

        public EfChangePasswordCommand(FlexTravelContext context, IUser user, ChangePasswordValidator validator) : base(context)
        {
            _user = user;
            _validator = validator;
        }

        public int UseCaseId => UseCaseIds.ChangePassword;
        public string UseCaseName => "Change password";
        public string UseCaseDescription => "Change own password";

        public void Execute(ChangePasswordDto request)
        {
            _validator.ValidateAndThrow(request);

            var user = Context.User.FirstOrDefault(u => u.UserId == _user.Id)
                ?? throw new EntityNotFoundException("User", _user.Id);

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password))
            {
                throw new ValidationException(new[] { new ValidationFailure("CurrentPassword", "The current password is not correct.") });
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            FlexTravel.Implementation.Account.SessionStore.RevokeAll(Context, user.UserId, request.KeepRefreshToken);
            Context.SaveChanges();
        }
    }
}
