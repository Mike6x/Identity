using System.Collections.ObjectModel;

namespace Identity.Shared.Authorization;

public static class AppRoles
{
    public const string Superuser = nameof(Superuser);
    public const string Root = nameof(Root);
    public const string Admin = nameof(Admin);
    public const string Manager = nameof(Manager);
    public const string Editor = nameof(Editor);
    public const string Viewer = nameof(Viewer);
    public const string Customer = nameof(Customer);
    public const string Basic = nameof(Basic);

    public static IReadOnlyList<string> DefaultRoles { get; } = new ReadOnlyCollection<string>([
        Superuser,
        Root,
        Admin,
        Manager,
        Editor,
        Viewer,
        Customer,
        Basic
    ]);

    public static bool IsDefault(string roleName)
    {
        return DefaultRoles.Any(r => r == roleName);
    }
}