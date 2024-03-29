﻿using RoleBasedApplication.Entities;
using RoleBasedApplication.Models;

namespace RoleBasedApplication.Services
{
    public interface IUserService
    {
       public string getRole();
        public Task<string> getUserName();
        public Task<List<UserDto>> getAllUsers();
        public Task<List<PostDto>> getAllPosts();

        public Task<PostDto> addPost(PostDto post);
        public Task<PostDto> getPostById(int postId);
        Task<List<PostDto>> getAllPostsByUserId(int userId);
        Task<List<PostDto>> getAllPostsByUserName(string userName);
        Task<List<PostDto>> getLatestPostsByUserName(string userName);
    }
}
