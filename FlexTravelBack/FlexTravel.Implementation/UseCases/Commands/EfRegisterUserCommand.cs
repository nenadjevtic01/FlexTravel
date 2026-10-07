using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Validators;
using FluentValidation;

namespace FlexTravel.Implementation.UseCases.Commands
{
    public class EfRegisterUserCommand : EfUseCase, IRegisterUserCommand
    {
        private readonly RegisterUserValidator _validator;

        public EfRegisterUserCommand(FlexTravelContext context, RegisterUserValidator validator) : base(context)
        {
            _validator = validator;
        }

        public int UseCaseId => UseCaseIds.RegisterUser;
        public string UseCaseName => "Register user";
        public string UseCaseDescription => "Create a customer account";

        public void Execute(RegisterUserDto request)
        {
            _validator.ValidateAndThrow(request);

            var user = new RegularUser
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = request.Email.Trim(),
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Phone = request.Phone.Trim(),
                Address = request.Address.Trim(),
                City = request.City.Trim()
            };

            foreach (var useCaseId in UserUseCaseConstants.RegularUserUseCaseIds)
            {
                user.UseCases.Add(new UserUseCase { UseCaseId = useCaseId });
            }

            Context.User.Add(user);
            Context.SaveChanges();
        }
    }
}
