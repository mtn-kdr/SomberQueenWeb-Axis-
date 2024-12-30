using Microsoft.AspNetCore.Mvc;
using SomberQueenWeb_Axis_.Utilities;
using SomberQueenWeb_Axis_.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace SomberQueenWeb_Axis_.Controllers
{
    public class EncryptedFilesController : Controller
    {
        private readonly DBHelper _dbHelper;

        public EncryptedFilesController(DBHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            string query = "SELECT * FROM encrypted_files";
            var encryptedFiles = _dbHelper.Query<EncryptedFile>(query);
            return View(encryptedFiles);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var file = await _dbHelper.QueryFirstOrDefaultAsync<EncryptedFile>(
                "SELECT * FROM encrypted_files WHERE id = @id",
                new { id });

            if (file == null)
            {
                return NotFound();
            }

            return View(file);
        }

        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string query = "SELECT * FROM encrypted_files WHERE file_id = @file_id";
            var file = _dbHelper.QueryFirstOrDefault<EncryptedFile>(query, new { file_id = id });

            if (file == null)
            {
                return NotFound();
            }

            return View(file);
        }

    }
}
