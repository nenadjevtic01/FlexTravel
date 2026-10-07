using FlexTravel.Application.Dto;
using FlexTravel.Application.Notifications;
using FlexTravel.Application.UseCases;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FlexTravel.Tests.Infrastructure
{
    public sealed class TestDatabase : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<FlexTravelContext> _options;

        public TestDatabase()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _connection.CreateFunction("GETDATE", () => DateTime.Now);

            _options = new DbContextOptionsBuilder<FlexTravelContext>().UseSqlite(_connection).Options;
            Context = NewContext();
            Context.Database.EnsureCreated();
            World = World.Create(Context);
        }

        public FlexTravelContext Context { get; }
        public World World { get; }

        public FlexTravelContext NewContext() => new FlexTravelContext(_options);

        public void Dispose()
        {
            Context.Dispose();
            _connection.Dispose();
        }
    }

    public class TestUser : IUser
    {
        public TestUser(User user, params int[] useCaseIds)
        {
            Id = user.UserId;
            Identity = user.FirstName + " " + user.LastName;
            Email = user.Email;
            UseCaseIds = useCaseIds;
        }

        public string Identity { get; }
        public int Id { get; }
        public IEnumerable<int> UseCaseIds { get; }
        public string Email { get; }
    }

    public class RecordingPublisher : INotificationPublisher
    {
        public List<(int UserId, NotificationDto Notification)> Published { get; } = new List<(int, NotificationDto)>();

        public void Publish(int userId, NotificationDto notification) => Published.Add((userId, notification));
    }
}
