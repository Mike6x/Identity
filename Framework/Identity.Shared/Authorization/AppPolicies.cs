namespace Identity.Shared.Authorization
{
    public static class AppPolicies
    {
        
        public const string CanManageApplications = nameof(CanManageApplications);
        public const string CanManageScopes = nameof(CanManageScopes);
        public const string CanManageUsers = nameof(CanManageUsers);
       public const string CanManageRoles = nameof(CanManageRoles);
        
        public const string AuthPolicy = nameof(AuthPolicy);
       
        public const string CanManageStudents = nameof(CanManageStudents);
        public const string CanManageCities = nameof(CanManageCities);
        
        public const string PaidForecast = nameof(PaidForecast);  
        public const string SecureForecast = nameof(SecureForecast);  
        
        public const string WeatherRead = "weather:read";
    }
}
