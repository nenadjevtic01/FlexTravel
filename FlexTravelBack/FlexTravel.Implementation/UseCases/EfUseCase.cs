using FlexTravel.DataAccess;

namespace FlexTravel.Implementation.UseCases
{
    public abstract class EfUseCase
    {
        protected EfUseCase(FlexTravelContext context)
        {
            Context = context;
        }

        protected FlexTravelContext Context { get; }
    }
}
