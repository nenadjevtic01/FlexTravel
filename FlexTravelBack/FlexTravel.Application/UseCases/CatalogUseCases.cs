using FlexTravel.Application.Dto;

namespace FlexTravel.Application.UseCases.Commands
{
    public interface ISaveCountryCommand : ICommand<SaveCountryDto, int>
    {
    }

    public interface ISaveDestinationCommand : ICommand<SaveDestinationDto, int>
    {
    }

    public interface ISaveHotelCommand : ICommand<SaveHotelDto, int>
    {
    }

    public interface ISaveSightCommand : ICommand<SaveSightDto, int>
    {
    }

    public interface IDeleteSightCommand : ICommand<int>
    {
    }

    public interface IUploadImageCommand : ICommand<UploadImageDto, string>
    {
    }
}

namespace FlexTravel.Application.UseCases.Queries
{
    public interface IGetCatalogOptionsQuery : IQuery<int, CatalogOptionsDto>
    {
    }

    public interface IGetDestinationForEditQuery : IQuery<int, DestinationForEditDto>
    {
    }
}

namespace FlexTravel.Application.Files
{
    public interface IFileStorage
    {
        string Save(string extension, byte[] content);

        IEnumerable<(string Src, DateTime SavedAt)> List();

        void Delete(string src);
    }
}
