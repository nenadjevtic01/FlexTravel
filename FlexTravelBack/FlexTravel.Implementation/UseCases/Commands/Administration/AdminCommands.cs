using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.Exceptions;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Exceptions;
using FlexTravel.Implementation.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace FlexTravel.Implementation.UseCases.Commands.Administration
{
    public class EfUpdateUserUseCasesCommand : EfUseCase, IUpdateUserUseCasesCommand
    {
        private readonly IUser _user;
        private readonly UpdateUserUseCasesValidator _validator;

        public EfUpdateUserUseCasesCommand(FlexTravelContext context, IUser user, UpdateUserUseCasesValidator validator)
            : base(context)
        {
            _user = user;
            _validator = validator;
        }

        public int UseCaseId => UseCaseIds.UpdateUserUseCases;
        public string UseCaseName => "Update user use cases";
        public string UseCaseDescription => "Grant or revoke use cases of a user";

        public void Execute(UpdateUserUseCasesDto request)
        {
            _validator.ValidateAndThrow(request);

            var user = Context.User.Include(u => u.UseCases).FirstOrDefault(u => u.UserId == request.UserId);
            if (user == null)
            {
                throw new EntityNotFoundException("User", request.UserId);
            }

            var wanted = request.UseCaseIds.Distinct().ToList();

            if (user.UserId == _user.Id && UserUseCaseConstants.AdminLockoutGuard.Any(id => !wanted.Contains(id)))
            {
                throw new ConflictException("You cannot remove your own permission to manage users.");
            }

            foreach (var removed in user.UseCases.Where(u => !wanted.Contains(u.UseCaseId)).ToList())
            {
                user.UseCases.Remove(removed);
            }

            foreach (var added in wanted.Where(id => user.UseCases.All(u => u.UseCaseId != id)))
            {
                user.UseCases.Add(new UserUseCase { UseCaseId = added });
            }

            Context.SaveChanges();
        }
    }

    public class EfSetUserActiveCommand : EfUseCase, ISetUserActiveCommand
    {
        private readonly IUser _user;

        public EfSetUserActiveCommand(FlexTravelContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public int UseCaseId => UseCaseIds.SetUserActive;
        public string UseCaseName => "Set user active";
        public string UseCaseDescription => "Block or unblock a user account";

        public void Execute(SetUserActiveDto request)
        {
            if (request.UserId == _user.Id)
            {
                throw new ConflictException("You cannot block your own account.");
            }

            var user = Context.User.FirstOrDefault(u => u.UserId == request.UserId);
            if (user == null)
            {
                throw new EntityNotFoundException("User", request.UserId);
            }

            user.IsActive = request.IsActive;

            if (request.IsActive)
            {
                user.LockedUntil = null;
                user.FailedLoginAttempts = 0;
            }
            else
            {
                FlexTravel.Implementation.Account.SessionStore.RevokeAll(Context, user.UserId);
            }

            Context.SaveChanges();
        }
    }

    public class EfCreateAgencyCommand : EfUseCase, ICreateAgencyCommand
    {
        private readonly CreateAgencyValidator _validator;

        public EfCreateAgencyCommand(FlexTravelContext context, CreateAgencyValidator validator) : base(context)
        {
            _validator = validator;
        }

        public int UseCaseId => UseCaseIds.CreateAgency;
        public string UseCaseName => "Create agency";
        public string UseCaseDescription => "Register a new partner agency";

        public int Execute(CreateAgencyDto request)
        {
            _validator.ValidateAndThrow(request);

            var agency = new Agency
            {
                AgencyName = request.Name.Trim(),
                Address = request.Address.Trim(),
                Phone = request.Phone.Trim()
            };

            Context.Agency.Add(agency);
            Context.SaveChanges();
            return agency.AgencyId;
        }
    }

    public class EfCreateAgencyEmployeeCommand : EfUseCase, ICreateAgencyEmployeeCommand
    {
        private readonly CreateAgencyEmployeeValidator _validator;

        public EfCreateAgencyEmployeeCommand(FlexTravelContext context, CreateAgencyEmployeeValidator validator)
            : base(context)
        {
            _validator = validator;
        }

        public int UseCaseId => UseCaseIds.CreateAgencyEmployee;
        public string UseCaseName => "Create agency employee";
        public string UseCaseDescription => "Create an account for an agency employee";

        public int Execute(CreateAgencyEmployeeDto request)
        {
            _validator.ValidateAndThrow(request);

            var employee = new AgencyEmployee
            {
                AgencyId = request.AgencyId,
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = request.Email.Trim(),
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Phone = request.Phone.Trim(),
                Address = request.Address.Trim(),
                City = request.City.Trim()
            };

            foreach (var useCaseId in UserUseCaseConstants.AgencyEmployeeUseCaseIds)
            {
                employee.UseCases.Add(new UserUseCase { UseCaseId = useCaseId });
            }

            Context.User.Add(employee);
            Context.SaveChanges();
            return employee.UserId;
        }
    }
}
