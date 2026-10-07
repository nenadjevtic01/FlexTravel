namespace FlexTravel.Application.UseCases
{
    public interface IQuery<TRequest, TResponse> : IUseCase
    {
        public TResponse Execute(TRequest request);
    }
}
