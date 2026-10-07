using FlexTravel.Application.Files;

namespace FlexTravel.Api.Core
{
    public class LocalFileStorage : IFileStorage
    {
        public const string Folder = "uploads";
        private readonly string _directory;

        public LocalFileStorage(IWebHostEnvironment environment)
        {
            _directory = DirectoryFor(environment);
            Directory.CreateDirectory(_directory);
        }

        public static string DirectoryFor(IWebHostEnvironment environment) =>
            Path.Combine(environment.ContentRootPath, "wwwroot", Folder);

        public string Save(string extension, byte[] content)
        {
            var name = Guid.NewGuid().ToString("N") + extension;
            File.WriteAllBytes(Path.Combine(_directory, name), content);
            return $"{Folder}/{name}";
        }

        public IEnumerable<(string Src, DateTime SavedAt)> List() =>
            Directory.EnumerateFiles(_directory)
                .Select(path => ($"{Folder}/{Path.GetFileName(path)}", File.GetLastWriteTimeUtc(path)))
                .ToList();

        public void Delete(string src)
        {
            var name = Path.GetFileName(src ?? "");
            if (src != $"{Folder}/{name}" || name.Length == 0)
            {
                return;
            }

            var path = Path.Combine(_directory, name);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
