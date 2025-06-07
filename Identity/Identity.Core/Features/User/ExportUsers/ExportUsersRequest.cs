using BuildingBlocks.Paging;

namespace Identity.Core.Features.User.ExportUsers;

public class ExportUsersRequest : BaseFilter
{
    public bool? IsActive { get; set; }
}
