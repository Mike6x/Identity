namespace BuildingBlocks.Storage.File.Features;
// public class FileUploadCommand : IRequest<FileUploadResponse>
public class FileUploadCommand
{
    public string Name { get; set; } = default!;
    public string Extension { get; set; } = default!;
    public string Data { get; set; } = default!;
}
