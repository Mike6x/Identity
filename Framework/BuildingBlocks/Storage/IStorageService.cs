using BuildingBlocks.Storage.File;
using BuildingBlocks.Storage.File.Features;

namespace BuildingBlocks.Storage;

public interface IStorageService
{
    public Task<Uri> UploadAsync<T>(FileUploadCommand? request, FileType supportedFileType, CancellationToken cancellationToken = default)
        where T : class;

    public void Remove(Uri? path);

    public Uri? UnZip(Uri zipPath);

    public void RemoveFolder(string fullPath);

    public string GetLocalPathFromUri(Uri? path, bool isFullPath);
}

// Add from fsh