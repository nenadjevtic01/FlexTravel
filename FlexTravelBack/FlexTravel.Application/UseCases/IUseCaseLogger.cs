using FlexTravel.Application.Dto;

namespace FlexTravel.Application.UseCases
{
    public interface IUseCaseLogger
    {
        public void Log(UseCaseLogDto log);
        PagedResponse<UseCaseLogDto> GetLogs(UseCaseLogSearchDto search);
    }
}
