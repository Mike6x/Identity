using Microsoft.JSInterop;

namespace BlazorWeb.Wasm.Client.Services;

public class HostingEnvironmentService
{
    private bool IsWebAssembly { get; set; }

    public HostingEnvironmentService(IJSRuntime jsRuntime)
    {
        IsWebAssembly = jsRuntime is IJSInProcessRuntime;
    }

    public string EnvironmentName => IsWebAssembly ? "WebAssembly" : "Server";
}