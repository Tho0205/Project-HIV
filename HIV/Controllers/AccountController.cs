
using HIV.DTOs;
using HIV.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;


namespace WebAPITest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Account
        [HttpGet]
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
                
            return Ok(new
            {
                //trả về 1 chuổi kiểu if else nếu đùng thì trả về fullname còn sai thì trả về Unknown
                fullName = user?.FullName ?? "Unknown",
                role = user?.Role ?? "Unknown",
                accountid = account.AccountId
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

            Random random = new Random();

            //sau khi có account_id mới tiến hành thêm Data của User vô
            var newUser = new User
            {
                UserId = random.Next(1000000, 10000000),
                AccountId = newAccount.AccountId,
                FullName = dto.full_name,
                Phone = dto.phone,
                Gender = dto.gender,
                Birthdate = dto.birthdate,
                Role = dto.role
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Register success" });
        }


        // GET: api/Account/
        [HttpGet("{id}")]
        public async Task<ActionResult<DTOGetbyID>> GetAccountById(int id)
        {
            var account = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AccountId == id);

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
            };

            return Ok(dto);
        }

        //[HttpPut("{id}")]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(204)]
        //[ProducesResponseType(404)]

        //public async Task<ActionResult<DTOGetbyID>> UpdateInfo(int account_id, [FromBody]DTOGetbyID updateinfo)
        //{
        //    var account = await _context.Account
        //        .Include(a => a.User)
        //        .FirstOrDefaultAsync(a => a.account_id == account_id);
        //}
    }
}
