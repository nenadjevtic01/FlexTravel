using FlexTravel.Application.Constants;
using FlexTravel.Application.UseCases;

namespace FlexTravel.Api.Core
{
    public class JwtUser : IUser
    {
        public string Identity { get; set; }

        public int Id { get; set; }

        public IEnumerable<int> UseCaseIds { get; set; } = new List<int>();
        public string Email { get; set; }
    }

    public class AnonymousUser : IUser
    {
        public string Identity => "Anonymous";
        public int Id => 0;
        public IEnumerable<int> UseCaseIds => UserUseCaseConstants.AnonymousUserUseCaseIds;
        public string Email => "anonymous@localhost";
    }
}
