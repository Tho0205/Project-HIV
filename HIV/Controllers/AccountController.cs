
using HIV.DTOs;
using HIV.Interfaces;
using HIV.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;


namespace WebAPITest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;

        public AccountController(AppDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        private int GetCurrentUserId()
        {
            var accountIdClaim = User.FindFirst("AccountId")?.Value;
            return int.TryParse(accountIdClaim, out var accountId) ? accountId : 0;
        }

        private string GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? "";
        }

        // GET: api/Account
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            return await _context.Accounts.ToListAsync();
        }

        // POST: api/Account/login
        [HttpPost("login")]
        public async Task<ActionResult<object>> Login([FromBody] DTOLogin login)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == login.identifier || a.Username == login.identifier);
            if (account == null)
            {
                return BadRequest(new { title = "Account incorrect." });
            }

            if (account.PasswordHash != login.password_hash)
            {
                return BadRequest(new { title = "Password incorrect." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == account.AccountId);

            var token = _jwtService.GenerateToken(
                user?.FullName ?? "Unknown",
                user?.Role ?? "Unknown",
                user?.UserId ?? 0,
                account.AccountId,
                user?.UserAvatar ?? "Unknown"
              );

            return Ok(new
            {
                //trả về 1 chuổi kiểu if else nếu đùng thì trả về fullname còn sai thì trả về Unknown
                //fullName = user?.FullName ?? "Unknown",
                //role = user?.Role ?? "Unknown",
                //accountid = account.AccountId,
                //user_avatar = user?.UserAvatar ?? "Unknown",
                //userid = account.User.UserId,
                // trả về 1 token
                token = token, 
                expires = DateTime.UtcNow.AddMinutes(30),
                success = true,
                message = "Login successful"
            });
        }




        [HttpPost("register")]
        public async Task<ActionResult<Account>> Register([FromBody] DTOGetbyID dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existing = await _context.Accounts.AnyAsync(a => a.Username == dto.username || a.Email == dto.email);
            if (existing)
            {
                return BadRequest("Username or Email already exists.");
            }

            //tạo trước để lấy account_id
            var newAccount = new Account
            {
                Username = dto.username,
                PasswordHash = dto.password_hash,
                Email = dto.email,
                CreatedAt = DateTime.Now
            };
            _context.Accounts.Add(newAccount);
            await _context.SaveChangesAsync();


            //sau khi có account_id mới tiến hành thêm Data của User vô
            var newUser = new User
            {
                UserId = dto.user_id,
                AccountId = newAccount.AccountId,
                FullName = dto.full_name,
                Phone = dto.phone,
                Gender = dto.gender,
                Birthdate = dto.birthdate,
                Role = dto.role,
                Address = dto.address,
                UserAvatar = string.IsNullOrWhiteSpace(dto.user_avatar) ? "patient.png" : dto.user_avatar
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Register success" });
        }


        // GET: api/Account/
        [HttpGet("{id}")]
        [Authorize(Roles = "Patient, Doctor")]
        public async Task<ActionResult<DTOGetbyID>> GetAccountById(int id)
        {

            var account = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AccountId == id);

            if (account == null)
            {
                return NotFound();
            }

            var dto = new DTOGetbyID
            {
                account_id = account.AccountId,
                username = account.Username,
                email = account.Email,
                created_at = (DateTime)account.CreatedAt,
                full_name = account.User.FullName,
                phone = account.User.Phone,
                gender = account.User.Gender,
                birthdate = account.User.Birthdate,
                role = account.User.Role,
                address = account.User.Address,
                user_avatar = account.User.UserAvatar
            };

            return Ok(dto);
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "Patient")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<DTOUpdate>> UpdateInfo(int id, [FromBody] DTOUpdate updateinfo)
        {


            var account = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }
            account.User.FullName = updateinfo.full_name;
            account.User.Gender = updateinfo.gender;
            account.User.Phone = updateinfo.phone;
            account.User.Birthdate = updateinfo.birthdate;
            account.User.Role = updateinfo.role;
            account.User.Address = updateinfo.address;
            account.User.UserAvatar = updateinfo.user_avatar;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPut("ChangePass/{id}")]
        [Authorize(Roles = "Patient")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePassword changepass)
        {

            var account = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AccountId == id);

            if (account == null)
            {
                return NotFound();
            }

            account.PasswordHash = changepass.password_hash;
            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpPost("UploadAvatar/{accountId}")]
        public async Task<IActionResult> UploadAvatar(int accountId, IFormFile avatar)

        {

            if (avatar == null || avatar.Length == 0)
                return BadRequest("No file uploaded.");

            var account = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AccountId == accountId);

            if (account == null)
                return NotFound("Account not found.");

            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Avatars");
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            // Xóa file avatar cũ nếu có
            var oldFileName = account.User.UserAvatar;
            if (!string.IsNullOrEmpty(oldFileName) && oldFileName.ToLower() != "patient.png")
            {
                var oldFilePath = Path.Combine(uploadFolder, oldFileName);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(avatar.FileName);
            var filePath = Path.Combine(uploadFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await avatar.CopyToAsync(stream);
            }

            // Lưu tên file mới vào DB (chỉ lưu tên file, không lưu full đường dẫn)
            account.User.UserAvatar = fileName;
            await _context.SaveChangesAsync();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var imageUrl = $"{baseUrl}/api/account/avatar/{fileName}";

            return Ok(new { path = imageUrl });
        }


        [HttpGet("avatar/{fileName}")]
        public IActionResult GetAvatar(string fileName)
        {
            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Avatars");
            var filePath = Path.Combine(uploadFolder, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var ext = Path.GetExtension(fileName).ToLower();
            var contentType = ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
            return PhysicalFile(filePath, contentType);
        }

        [HttpPost("refresh-token")]
        [Authorize]
        public async Task<ActionResult<object>> RefreshToken()
        {
            var currentUserId = GetCurrentUserId();
            var account = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AccountId == currentUserId);

            if (account == null)
                return NotFound();

            var newtoken = _jwtService.GenerateToken(
              account.User?.FullName ?? "Unknown",
              account.User?.Role ?? "Unknown",
              account.User?.UserId ?? 0,
              account.AccountId,
              account.User?.UserAvatar ?? "Unknown"
            );

            return Ok(new
            {
                token = newtoken,
                expires = DateTime.UtcNow.AddMinutes(60)
            });
        }
    }
}
