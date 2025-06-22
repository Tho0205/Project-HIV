using AutoMapper;
using DemoSWP391.Services;
using HIV.DTOs;
using HIV.Interfaces;
using HIV.Models;
using Microsoft.AspNetCore.Mvc;

namespace HIV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminManagementAccount _accountService;
        private readonly IMapper _mapper;

        public AdminController(IAdminManagementAccount accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountDto>>> GetAllAccounts()
        {
            try
            {
                var accounts = await _accountService.GetAllAccountsAsync();
                var accountDtos = _mapper.Map<IEnumerable<AccountDto>>(accounts);
                return Ok(accountDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AccountDto>> GetAccount(int id)
        {
            try
            {
                var account = await _accountService.GetAccountByIdAsync(id);
                if (account == null)
                {
                    return NotFound($"Không tìm thấy tài khoản với ID {id}");
                }

                var accountDto = _mapper.Map<AccountDto>(account);
                return Ok(accountDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ: {ex.Message}");
            }
        }

        [HttpGet("{id}/info")]
        public async Task<ActionResult<AccountInfoDto>> GetAccountInfo(int id)
        {
            try
            {
                var account = await _accountService.GetAccountInfoAsync(id);
                if (account == null)
                {
                    return NotFound($"Không tìm thấy tài khoản với ID {id}");
                }

                var accountInfoDto = _mapper.Map<AccountInfoDto>(account);
                return Ok(accountInfoDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<AccountDto>> CreateAccount(CreateAccountDto createAccountDto)
        {
            try
            {
                // Kiểm tra tài khoản đã tồn tại
                var exists = await _accountService.AccountExistsAsync(createAccountDto.Username, createAccountDto.Email);
                if (exists)
                {
                    return BadRequest("Tài khoản với tên đăng nhập hoặc email này đã tồn tại");
                }

                var account = _mapper.Map<Account>(createAccountDto);
                var createdAccount = await _accountService.CreateAccountAsync(account);
                var accountDto = _mapper.Map<AccountDto>(createdAccount);

                return CreatedAtAction(nameof(GetAccount), new { id = accountDto.AccountId }, accountDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(int id, UpdateAccountDto updateAccountDto)
        {
            try
            {
                if (id != updateAccountDto.AccountId)
                {
                    return BadRequest("ID tài khoản không khớp");
                }

                var account = _mapper.Map<Account>(updateAccountDto);
                var updatedAccount = await _accountService.UpdateAccountAsync(account);

                if (updatedAccount == null)
                {
                    return NotFound($"Không tìm thấy tài khoản với ID {id}");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ: {ex.Message}");
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateAccountStatus(int id, UpdateAccountStatusDto statusDto)
        {
            try
            {
                if (id != statusDto.AccountId)
                {
                    return BadRequest("ID tài khoản không khớp");
                }

                var result = await _accountService.UpdateAccountStatusAsync(id, statusDto.Status);
                if (!result)
                {
                    return NotFound($"Không tìm thấy tài khoản với ID {id}");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            try
            {
                var result = await _accountService.DeleteAccountAsync(id);
                if (!result)
                {
                    return NotFound($"Không tìm thấy tài khoản với ID {id}");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ: {ex.Message}");
            }
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<AccountDto>>> GetAccountsByStatus(string status)
        {
            try
            {
                var accounts = await _accountService.GetAccountsByStatusAsync(status);
                var accountDtos = _mapper.Map<IEnumerable<AccountDto>>(accounts);
                return Ok(accountDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ: {ex.Message}");
            }
        }

        [HttpGet("username/{username}")]
        public async Task<ActionResult<AccountDto>> GetAccountByUsername(string username)
        {
            try
            {
                var account = await _accountService.GetAccountByUsernameAsync(username);
                if (account == null)
                {
                    return NotFound($"Không tìm thấy tài khoản với tên đăng nhập {username}");
                }

                var accountDto = _mapper.Map<AccountDto>(account);
                return Ok(accountDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ: {ex.Message}");
            }
        }
    }
}