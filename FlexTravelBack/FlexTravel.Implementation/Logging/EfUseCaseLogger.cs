using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases;
using FlexTravel.DataAccess;
using FlexTravel.Domain;

namespace FlexTravel.Implementation.Logging
{
    public class EfUseCaseLogger : IUseCaseLogger
    {
        private readonly FlexTravelContext _context;

        public EfUseCaseLogger(FlexTravelContext context)
        {
            _context = context;
        }

        public void Log(UseCaseLogDto log)
        {
            _context.AuditLog.Add(new AuditLog
            {
                UserId = log.UserId,
                Name = log.User,
                UseCaseName = log.UseCaseName,
                ExecutionDate = log.ExecutionDateTime,
                Data = log.Data,
                IsAuthorized = log.IsAuthorized
            });

            _context.SaveChanges();
        }

        public PagedResponse<UseCaseLogDto> GetLogs(UseCaseLogSearchDto search)
        {
            var query = _context.AuditLog.AsQueryable();

            if (search.DateFrom.HasValue)
            {
                query = query.Where(x => x.ExecutionDate >= search.DateFrom.Value);
            }

            if (search.DateTo.HasValue)
            {
                query = query.Where(x => x.ExecutionDate <= search.DateTo.Value);
            }

            if (!string.IsNullOrWhiteSpace(search.UseCaseName))
            {
                query = query.Where(x => x.UseCaseName.Contains(search.UseCaseName));
            }

            if (!string.IsNullOrWhiteSpace(search.Username))
            {
                query = query.Where(x => x.Name.Contains(search.Username));
            }

            var perPage = Math.Clamp(search.PerPage, 1, 100);
            var page = Math.Max(search.Page, 1);

            return new PagedResponse<UseCaseLogDto>
            {
                TotalCount = query.Count(),
                CurrentPage = page,
                ItemsPerPage = perPage,
                Items = query
                    .OrderByDescending(x => x.AuditLogId)
                    .Skip((page - 1) * perPage)
                    .Take(perPage)
                    .Select(x => new UseCaseLogDto
                    {
                        Id = x.AuditLogId,
                        UserId = x.UserId,
                        User = x.Name,
                        UseCaseName = x.UseCaseName,
                        ExecutionDateTime = x.ExecutionDate,
                        Data = x.Data,
                        IsAuthorized = x.IsAuthorized
                    })
                    .ToList()
            };
        }
    }
}
