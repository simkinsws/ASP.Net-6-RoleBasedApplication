using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoleBasedApplication.Entities;
using RoleBasedApplication.Models;
using System.Security.Claims;

namespace RoleBasedApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {


        private DataBaseContext _context;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostController(IHttpContextAccessor httpContextAccessor ,IConfiguration configuration, IUserService userService,
            DataBaseContext dataBaseContext)
        {
            _configuration = configuration;
            _userService = userService;
            _context = dataBaseContext;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("AddPost"), Authorize]
        public async Task<ActionResult<PostDto>> AddPost(PostDto post)
        {
            return Ok(await _userService.addPost(post));
        }

        [HttpGet("GetPostById/{postId}")]
        public async Task<ActionResult<PostDto>> getPostById(int postId)
        {
            //var postById = await _context.Posts.FindAsync(postId);
            //if(postById == null)
            //{
            //    return NotFound($"Post with id {postId} dosnt exists.");
            //}
            return Ok(await _userService.getPostById(postId));
        }

        [HttpGet("GetAllPostsByUserId/{userId}")]
        public async Task<ActionResult<List<PostDto>>> getAllPostsByUserId(int userId)
        {
            return Ok(await _userService.getAllPostsByUserId(userId));
        }
    }
}
