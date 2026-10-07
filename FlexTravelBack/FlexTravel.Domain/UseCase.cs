namespace FlexTravel.Domain
{
    public class UseCase
    {
        public int UseCaseId { get; set; }
        public string UseCaseName { get; set; }
        public string UseCaseDescription { get; set; }

        public virtual ICollection<UserUseCase> Users { get; set; } = new List<UserUseCase>();
    }
}
