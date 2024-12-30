using Microsoft.AspNetCore.Mvc;
using SomberQueenWeb_Axis_.Utilities;
using SomberQueenWeb_Axis_.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace SomberQueenWeb_Axis_.Controllers
{
    public class UsersController : BaseController
    {
        public UsersController(DBHelper dbHelper) : base(dbHelper) { }

        public async Task<IActionResult> Index()
        {
            string query = "SELECT * FROM users ORDER BY created_at DESC";
            var users = await _dbHelper.QueryAsync<User>(query);
            return View(users);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string query = "SELECT * FROM users WHERE user_id = @user_id";
            var user = await _dbHelper.QueryFirstOrDefaultAsync<User>(query, new { user_id = id });

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        private async Task CreateLog(Guid userId, string action)
        {
            string query = @"INSERT INTO logs (user_id, action, created_at) 
                            VALUES (@user_id, @action, @created_at)";

            var parameters = new
            {
                user_id = userId,
                action = action,
                created_at = DateTime.UtcNow
            };

            await _dbHelper.ExecuteAsync(query, parameters);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _dbHelper.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM users WHERE user_id = @user_id",
                new { user_id = id });

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, User user)
        {
            if (id != user.user_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _dbHelper.ExecuteAsync(
                        "UPDATE users SET username = @username, role = @role WHERE user_id = @user_id",
                        new { username = user.username, role = user.role, user_id = user.user_id });

                    await _dbHelper.ExecuteAsync(
                        "UPDATE auth_users SET username = @username, role = @role WHERE username = @oldUsername",
                        new { username = user.username, role = user.role, oldUsername = user.username });

                    await CreateLog("UPDATE_USER", $"UserId: {user.user_id}, Username: {user.username}, Role: {user.role}");

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Update error: {ex.Message}");
                    ModelState.AddModelError("", "Güncelleme işlemi başarısız oldu.");
                }
            }
            return View(user);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid userId))
            {
                return NotFound();
            }

            string query = "SELECT * FROM users WHERE user_id = @user_id";
            var user = await _dbHelper.QueryFirstOrDefaultAsync<User>(query, new { user_id = userId });

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid userId))
                {
                    TempData["Error"] = "Geçersiz kullanıcı ID'si";
                    return RedirectToAction(nameof(Index));
                }

                var user = await _dbHelper.QueryFirstOrDefaultAsync<User>(
                    "SELECT * FROM users WHERE user_id = @user_id",
                    new { user_id = userId });

                if (user != null)
                {
                    await _dbHelper.ExecuteAsync(
                        "DELETE FROM encrypted_files WHERE user_id = @user_id",
                        new { user_id = userId });

                    await _dbHelper.ExecuteAsync(
                        "DELETE FROM logs WHERE user_id = @user_id",
                        new { user_id = userId });

                    await _dbHelper.ExecuteAsync(
                        "DELETE FROM users WHERE user_id = @user_id",
                        new { user_id = userId });

                    TempData["Success"] = $"Kullanıcı başarıyla silindi: {user.username}";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Silme işlemi başarısız oldu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
