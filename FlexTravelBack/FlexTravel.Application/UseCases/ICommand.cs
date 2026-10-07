namespace FlexTravel.Application.UseCases
{
    public interface ICommand<TRequest> : IUseCase
    {
        public void Execute(TRequest request);
    }

    public interface ICommand<TRequest, TResponse> : IUseCase
    {
        public TResponse Execute(TRequest request);
    }
}
