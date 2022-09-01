using Microsoft.EntityFrameworkCore;
using RoleBasedApplication.Entities;
using RoleBasedApplication.Models;
using System.Security.Claims;

namespace RoleBasedApplication.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private DataBaseContext _context;
        public UserService(IHttpContextAccessor httpContextAccessor, DataBaseContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }


        public async Task<List<PostDto>> getAllPosts()
        {
            List<PostModel> posts = await _context.Posts.ToListAsync();
            List<PostDto> postsDto = posts.ConvertAll(new Converter<PostModel, PostDto>(ConvertPostModelToPostDto));
            return postsDto;

        }
        public async Task<PostDto> getPostById(int postId)
        {
            var postModel = await _context.Posts.FindAsync(postId);
            if(postModel != null)
            {
               return ConvertPostModelToPostDto(postModel);
            }
            return new PostDto();
        }

        public async Task<List<PostDto>> getAllPostsByUserId(int userId)
        {
            List <PostModel> postsModels = await _context.Posts.Where(u => u.UserId == userId).ToListAsync();
            List<PostDto> postsDto = postsModels.ConvertAll(new Converter<PostModel, PostDto>(ConvertPostModelToPostDto));
            return postsDto;
        }
        public async Task<PostDto> addPost(PostDto post)
        {

            var loggedInUserName = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                loggedInUserName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            }

            var user = await _context.Users.Where(u => u.Username == loggedInUserName).FirstAsync();
            var postToReturn = new PostDto();
            if (user != null)
            {
                var newPost = new PostModel()
                {
                    Branch = post.Branch,
                    User = user,
                    UserId = user.Id,
                    Description = post.Description,
                    ImageUrl = post.ImageUrl,
                    Name = post.Name,
                    Phone = post.Phone,
                    Solution = post.Solution
                };

                postToReturn = ConvertPostModelToPostDto(newPost);
                await _context.Posts.AddAsync(newPost);
            }
            await _context.SaveChangesAsync();
            return postToReturn;
        }

        public async Task<List<UserDto>> getAllUsers()
        {
            List<UserDto> usersDto = new();
            List<UserModel> users = await _context.Users.Include(u => u.Posts).ToListAsync();

            usersDto = users.ConvertAll(new Converter<UserModel, UserDto>(ConvertUserModelToUserDto));
            return usersDto;
        }

        public string getRole()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            }
            return result;
        }

        private UserDto ConvertUserModelToUserDto(UserModel userModel)
        {
            UserDto userDto = new UserDto();
            userDto.Id = userModel.Id;
            userDto.Username = userModel.Username;
            userDto.Role = userModel.Role;
            userDto.Posts = userModel.Posts;
            return userDto;
        }

        private PostDto ConvertPostModelToPostDto(PostModel postModel)
        {
            PostDto postDto = new PostDto()
            {
                Branch = postModel.Branch,
                Description = postModel.Description,
                Id = postModel.Id,
                ImageUrl = postModel.ImageUrl,
                Name = postModel.Name,
                Phone = postModel.Phone,
                Solution = postModel.Solution,
                UserId = postModel.UserId
            };

            return postDto;
        }
    }
}
