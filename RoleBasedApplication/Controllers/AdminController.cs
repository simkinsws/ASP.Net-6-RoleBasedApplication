using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoleBasedApplication.Entities;
using RoleBasedApplication.Models;

namespace RoleBasedApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet("AllUsers"), Authorize(Roles = "Admin")]
        public Task<List<UserDto>> getAllUsers()
        {
            return _userService.getAllUsers();
        }

        [HttpGet("AllPosts"), Authorize(Roles = "Admin")]
        public Task<List<PostDto>> getAllPosts()
        {
            return _userService.getAllPosts();
        }
    }
}
