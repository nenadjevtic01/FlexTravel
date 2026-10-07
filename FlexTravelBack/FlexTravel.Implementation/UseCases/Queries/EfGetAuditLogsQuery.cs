using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Queries;

namespace FlexTravel.Implementation.UseCases.Queries
{
    public class EfGetAuditLogsQuery : IGetAuditLogsQuery, ISkipAuditLog
    {
        private readonly IUseCaseLogger _logger;

        public EfGetAuditLogsQuery(IUseCaseLogger logger)
        {
            _logger = logger;
        }

        public int UseCaseId => UseCaseIds.GetAuditLogs;
        public string UseCaseName => "Search audit logs";
        public string UseCaseDescription => "Search the log of executed use cases";

        public PagedResponse<UseCaseLogDto> Execute(UseCaseLogSearchDto search)
        {
            return _logger.GetLogs(search);
        }
    }
}
