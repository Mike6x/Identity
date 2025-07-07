namespace BuildingBlocks.Identity.Users.Dtos;
public class DashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int OnlineUsers { get; set; }
    
    public int LockedUsers { get; set; }
    public int NewUsersToday { get; set; }
    public IEnumerable<UserOnlineDto> RecentUsers { get; set; } = new List<UserOnlineDto>();
}
