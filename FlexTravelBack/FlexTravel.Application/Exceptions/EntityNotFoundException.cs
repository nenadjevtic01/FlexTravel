namespace FlexTravel.Application.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string entity, int id)
            : base($"{entity} with id {id} was not found.")
        {
        }
    }
}
