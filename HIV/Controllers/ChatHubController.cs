﻿using HIV.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace HIV.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatHubController :ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly AppDbContext _appDbContext;

        public ChatHubController(IHubContext<ChatHub> hubContext, AppDbContext appDbContext)
        {
            _hubContext = hubContext;
            _appDbContext = appDbContext;
        }

        [HttpGet("available-staff")]
        public IActionResult GetAvailableStaff()
        {
            var staffList = _appDbContext.Users
                .Where(x => x.Role == "Staff")
                .Select(x => new {
                    userId = x.UserId.ToString(),
                    name = x.FullName,
                    role = x.Role
                })
                .ToList();

            return Ok(staffList);
        }

        [HttpGet("available-patient")]
        public IActionResult GetAvailablePatient()
        {
            var patientList = _appDbContext.Users
                .Where(x => x.Role == "Patient")
                .Select(x => new {
                    userId = x.UserId.ToString(),
                    name = x.FullName,
                    role = x.Role
                })
                .ToList();

            return Ok(patientList);
        }
    }
}
