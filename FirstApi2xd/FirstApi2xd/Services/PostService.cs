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
            // El Include agrega los tags del post
            return await _dataContex.Posts.Include(x=> x.Tags).ToListAsync();
        }

        public async Task<Post> GetPostByIdAsync(Guid postId)
        {
            // El Include agrega los tags del post
            return await _dataContex.Posts.Include(x=> x.Tags).SingleOrDefaultAsync(x => x.Id == postId);
        }

        public async Task<bool> CreatePostAsync(Post post)
        {
            post.Tags?.ForEach(x=>x.TagName = x.TagName.ToLower());

            await AddNewTags(post);

            await _dataContex.Posts.AddAsync(post);

            var created = await _dataContex.SaveChangesAsync();
            return created > 0;

        }

        public async Task<bool> UpdatePostAsync(Post postToUpdate)
        {
            postToUpdate.Tags?.ForEach(x=> x.TagName = x.TagName.ToLower());

            await AddNewTags(postToUpdate);
            _dataContex.Posts.Update(postToUpdate);


            var updated = await _dataContex.SaveChangesAsync();
            return updated > 0;
        }

        public async Task<bool> DeletePostAsync(Guid postToDelete)
        {
            var post = await GetPostByIdAsync(postToDelete);
            if (post == null)
            {
                return false;
            }

            _dataContex.Posts.Remove(post);
            var deleted = await _dataContex.SaveChangesAsync();
            return deleted > 0;

        }

        public async Task<bool> UserOwnsPostAsync(Guid postId, string userId)
        {
            var post = await _dataContex.Posts.AsNoTracking().SingleOrDefaultAsync(x => x.Id == postId);
            return post != null && post.UserId == userId;
        }

        public async Task<List<Tags>> GetAllTagsAsync()
        {
            return await _dataContex.Tags.AsNoTracking().ToListAsync();
        }

        public async Task<bool> CreateTagAsync(Tags tag)
        {
            tag.Name = tag.Name.ToLower();
            var existingTag = await _dataContex.Tags.AsNoTracking().SingleOrDefaultAsync(x => x.Name == tag.Name);
            if (existingTag != null)
            {
                return false;
            }

            await _dataContex.Tags.AddAsync(tag);
            var created = await _dataContex.SaveChangesAsync();
            return created > 0;
        }

        public async Task<Tags> GetTagByNameAsync(string tagName)
        {
            return await _dataContex.Tags.AsNoTracking().SingleOrDefaultAsync(x => x.Name == tagName.ToLower());
        }

        public async Task<bool> DeleteTagAsync(string tagName)
        {
            var tag = await _dataContex.Tags.AsNoTracking().SingleOrDefaultAsync(x => x.Name == tagName.ToLower());
            if (tag == null) return false;

            var postTags = await _dataContex.PostTag.Where(x => x.TagName == tagName.ToLower()).ToListAsync();
            
            // Remove range lo uso cuando necesito eliminar todos los elementos de una coleccion
            _dataContex.RemoveRange(postTags);
            _dataContex.Remove(tag);
            return await _dataContex.SaveChangesAsync() > postTags.Count;

        }

        private async Task AddNewTags(Post post)
        {
            foreach (var tag in post.Tags)
            {
                var existingTag =
                    await _dataContex.Tags.SingleOrDefaultAsync(x => x.Name == tag.TagName);
                if (existingTag != null)
                {
                    continue;
                }

                await _dataContex.Tags.AddAsync(new Tags
                {
                    Name = tag.TagName, CreatedOn = DateTime.UtcNow, CreatorId = post.UserId
                });
            }
        }
    }
}