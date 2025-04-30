namespace IdentityServer.Core.Constants;

public static class Permissions
{
    // Admin Permissions
    public const string AdminManageUsers = "Permissions.Admin.ManageUsers";
    public const string AdminManageRoles = "Permissions.Admin.ManageRoles";
    public const string AdminManageSystem = "Permissions.Admin.ManageSystem";

    // Doctor Permissions
    public const string DoctorRead = "Permissions.Doctor.Read";
    public const string DoctorCreate = "Permissions.Doctor.Create";
    public const string DoctorUpdate = "Permissions.Doctor.Update";
    public const string DoctorDelete = "Permissions.Doctor.Delete";

    // Patient Permissions
    public const string PatientRead = "Permissions.Patient.Read";
    public const string PatientCreate = "Permissions.Patient.Create";
    public const string PatientUpdate = "Permissions.Patient.Update";
    public const string PatientDelete = "Permissions.Patient.Delete";

    public static IReadOnlyList<string> GetAllPermissions()
    {
        return typeof(Permissions)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
            .Select(x => (string)x.GetValue(null))
            .ToList()
            .AsReadOnly();
    }
}