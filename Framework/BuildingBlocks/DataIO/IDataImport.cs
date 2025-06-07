using BuildingBlocks.Storage.File;
using BuildingBlocks.Storage.File.Features;

namespace BuildingBlocks.DataIO;

public interface IDataImport
{
    Task<IList<T>> ToListAsync<T>(FileUploadCommand request, FileType supportedFileType, string sheetName = "Sheet1");
}
