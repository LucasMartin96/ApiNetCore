using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirstApi2xd.Data;
using FirstApi2xd.Domain;
using Microsoft.EntityFrameworkCore;

namespace FirstApi2xd.Services
{
    public class PostService : IPostService
    {
        private readonly DataContext _dataContex;

        public PostService(DataContext dbContext)
        {
            _dataContex = dbContext;
        }

        public async Task<List<Post>> GetPostsAsync()
        {
            return await _dataContex.Posts.ToListAsync();
        }

        public async Task<Post> GetPostByIdAsync(Guid postId)
        {
            return await _dataContex.Posts.SingleOrDefaultAsync(x => x.Id == postId);
        }

        public async Task<bool> CreatePostAsync(Post post)
        {
            await _dataContex.Posts.AddAsync(post);
            var created = await _dataContex.SaveChangesAsync();
            return created > 0;

        }

        public async Task<bool> UpdatePostAsync(Post postToUpdate)
        {
            _dataContex.Posts.Update(postToUpdate);

            var updated = await _dataContex.SaveChangesAsync();
            return updated > 0;
        }

        public async Task<bool> DeletePostAsync(Guid postToDelete)
        {
            var post = await GetPostByIdAsync(postToDelete);
            var deleted = await _dataContex.SaveChangesAsync();
            return deleted > 0;

        }

        public async Task<bool> UserOwnsPostAsync(Guid postId, string userId)
        {
            var post = await _dataContex.Posts.AsNoTracking().SingleOrDefaultAsync(x => x.Id == postId);
            return post != null && post.UserId == userId;
        }
    }
}