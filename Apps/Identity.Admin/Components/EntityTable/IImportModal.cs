namespace Identity.Admin.Components.EntityTable;

public interface IImportModal<out TRequest>
{
    TRequest RequestModel { get; }
    void ForceRender();
}
