using FlexTravel.Application.Dto;
using FlexTravel.Application.Files;
using FlexTravel.Implementation.UseCases.Commands;
using FlexTravel.Implementation.UseCases.Commands.Account;
using FlexTravel.Implementation.Validators;
using FlexTravel.Tests.Infrastructure;
using FluentValidation;

namespace FlexTravel.Tests
{
    public class AccountAdminTests : IDisposable
    {
        private static readonly byte[] Png = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0, 0, 0, 0 };

        private readonly TestDatabase _db = new TestDatabase();
        private readonly MemoryStorage _storage = new MemoryStorage();
        private World World => _db.World;

        public void Dispose() => _db.Dispose();

        private EfChangeMyAvatarCommand Avatar() =>
            new EfChangeMyAvatarCommand(_db.Context, new TestUser(World.Customer), _storage, new UploadImageValidator());

        [Fact]
        public void A_user_uploads_and_removes_their_own_photo()
        {
            var src = Avatar().Execute(new UploadImageDto { FileName = "me.png", ContentType = "image/png", Content = Png });

            Assert.Equal("uploads/1.png", src);
            Assert.Equal(src, _db.NewContext().User.Single(u => u.UserId == World.Customer.UserId).Avatar);

            Assert.Null(Avatar().Execute(new UploadImageDto()));
            Assert.Null(_db.NewContext().User.Single(u => u.UserId == World.Customer.UserId).Avatar);
        }

        [Fact]
        public void Only_real_images_are_accepted_as_photos()
        {
            var svg = new byte[] { (byte)'<', (byte)'s', (byte)'v', (byte)'g', 0, 0, 0, 0, 0, 0, 0, 0 };

            Assert.Throws<ValidationException>(() =>
                Avatar().Execute(new UploadImageDto { FileName = "me.png", ContentType = "image/png", Content = svg }));
            Assert.Empty(_storage.Saved);
        }

        [Fact]
        public void The_admin_removes_a_photo_and_cleaning_up_keeps_photos_in_use()
        {
            var src = Avatar().Execute(new UploadImageDto { FileName = "me.png", ContentType = "image/png", Content = Png });
            _storage.Save(".png", Png);
            _storage.MakeOld();

            Assert.Equal(1, new EfCleanUpImagesCommand(_db.Context, _storage).Execute(0));
            Assert.Contains(src, _storage.Saved);

            new EfRemoveUserAvatarCommand(_db.Context).Execute(World.Customer.UserId);
            Assert.Null(_db.NewContext().User.Single(u => u.UserId == World.Customer.UserId).Avatar);
        }

        [Fact]
        public void The_admin_corrects_account_data_but_e_mails_stay_unique()
        {
            var command = new EfUpdateUserCommand(_db.Context, new UpdateUserValidator(_db.Context));
            var request = new UpdateUserDto
            {
                UserId = World.Customer.UserId,
                FirstName = " Cleopatra ",
                LastName = "Test",
                Email = "Cleo.New@Test.Local",
                Phone = "555 1",
                Address = "New street",
                City = "Novi Sad"
            };

            command.Execute(request);

            var saved = _db.NewContext().User.Single(u => u.UserId == World.Customer.UserId);
            Assert.Equal(("Cleopatra", "cleo.new@test.local", "Novi Sad"), (saved.FirstName, saved.Email, saved.City));

            request.Email = World.OtherCustomer.Email;
            Assert.Throws<ValidationException>(() => command.Execute(request));
        }

        private class MemoryStorage : IFileStorage
        {
            private readonly Dictionary<string, DateTime> _files = new Dictionary<string, DateTime>();

            public IEnumerable<string> Saved => _files.Keys;

            public string Save(string extension, byte[] content)
            {
                var src = $"uploads/{_files.Count + 1}{extension}";
                _files[src] = DateTime.UtcNow;
                return src;
            }

            public IEnumerable<(string Src, DateTime SavedAt)> List() => _files.Select(f => (f.Key, f.Value)).ToList();

            public void Delete(string src) => _files.Remove(src);

            public void MakeOld()
            {
                foreach (var key in _files.Keys.ToList())
                {
                    _files[key] = DateTime.UtcNow.AddHours(-2);
                }
            }
        }
    }
}
