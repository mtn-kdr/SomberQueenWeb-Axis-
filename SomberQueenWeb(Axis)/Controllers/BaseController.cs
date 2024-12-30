using Microsoft.AspNetCore.Mvc;
using SomberQueenWeb_Axis_.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace SomberQueenWeb_Axis_.Controllers
{
    public class BaseController : Controller
    {
        protected readonly DBHelper _dbHelper;

        public BaseController(DBHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        protected async Task CreateLog(string action, string details = null)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    string fullAction = details != null ? $"{action}: {details}" : action;
                    
                    string query = @"INSERT INTO logs (user_id, action, created_at) 
                                   VALUES (@user_id, @action, @created_at)";

                    await _dbHelper.ExecuteAsync(query, new
                    {
                        user_id = userId,
                        action = fullAction,
                        created_at = DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Log creation failed: {ex.Message}");
            }
        }
    }
} 