namespace FlexTravel.Implementation.Exceptions
{
    public class ForbiddenUseCaseExecutionException : Exception
    {
        public ForbiddenUseCaseExecutionException(string UseCaseName, string user)
            : base($"User {user} has tried to execute {UseCaseName} without being authorized to do so.")
        {

        }
    }
}
