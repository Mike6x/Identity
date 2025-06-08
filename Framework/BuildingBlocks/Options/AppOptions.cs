using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.Options
{
    public class AppOptions : IOptionsRoot
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; } = "OpenIdDict.API";
    }
}
// Add from fsh