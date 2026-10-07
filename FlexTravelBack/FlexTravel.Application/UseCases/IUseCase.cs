namespace FlexTravel.Application.UseCases
{
    public interface IUseCase
    {
        public int UseCaseId { get; }
        public string UseCaseName { get; }
        public string UseCaseDescription { get; }
    }
}
