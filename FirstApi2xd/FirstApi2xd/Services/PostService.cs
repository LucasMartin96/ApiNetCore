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
        private readonly DataContext _dataContext;

        public PostService(DataContext dbContext)
        {
            _dataContext = dbContext;
        }

        public async Task<List<Post>> GetPostsAsync()
        {
            // El Include agrega los tags del post
            return await _dataContext.Posts.Include(x=> x.Tags).ToListAsync();
        }

        public async Task<Post> GetPostByIdAsync(Guid postId)
        {
            // El Include agrega los tags del post
            return await _dataContext.Posts.Include(x=> x.Tags).SingleOrDefaultAsync(x => x.Id == postId);
        }

        public async Task<bool> CreatePostAsync(Post post)
        {
            post.Tags?.ForEach(x=>x.TagName = x.TagName.ToLower());

            await AddNewTags(post);

            await _dataContext.Posts.AddAsync(post);

            var created = await _dataContext.SaveChangesAsync();
            return created > 0;

        }

        public async Task<bool> UpdatePostAsync(Post postToUpdate)
        {
            postToUpdate.Tags?.ForEach(x=> x.TagName = x.TagName.ToLower());

            await AddNewTags(postToUpdate);
            _dataContext.Posts.Update(postToUpdate);


            var updated = await _dataContext.SaveChangesAsync();
            return updated > 0;
        }

        public async Task<bool> DeletePostAsync(Guid postToDelete)
        {
            var post = await GetPostByIdAsync(postToDelete);
            if (post == null)
            {
                return false;
            }

            _dataContext.Posts.Remove(post);
            var deleted = await _dataContext.SaveChangesAsync();
            return deleted > 0;

        }

        public async Task<bool> UserOwnsPostAsync(Guid postId, string userId)
        {
            var post = await _dataContext.Posts.AsNoTracking().SingleOrDefaultAsync(x => x.Id == postId);
            return post != null && post.UserId == userId;
        }

        public async Task<List<Tags>> GetAllTagsAsync()
        {
            return await _dataContext.Tags.AsNoTracking().ToListAsync();
        }

        public async Task<bool> CreateTagAsync(Tags tag)
        {
            tag.Name = tag.Name.ToLower();
            var existingTag = await _dataContext.Tags.AsNoTracking().SingleOrDefaultAsync(x => x.Name == tag.Name);
            if (existingTag != null)
            {
                return false;
            }

            await _dataContext.Tags.AddAsync(tag);
            var created = await _dataContext.SaveChangesAsync();
            return created > 0;
        }

        public async Task<Tags> GetTagByNameAsync(string tagName)
        {
            return await _dataContext.Tags.AsNoTracking().SingleOrDefaultAsync(x => x.Name == tagName.ToLower());
        }

        public async Task<bool> DeleteTagAsync(string tagName)
        {
            var tag = await _dataContext.Tags.AsNoTracking().SingleOrDefaultAsync(x => x.Name == tagName.ToLower());
            if (tag == null) return false;

            var postTags = await _dataContext.PostTag.Where(x => x.TagName == tagName.ToLower()).ToListAsync();
            
            // Remove range lo uso cuando necesito eliminar todos los elementos de una coleccion
            _dataContext.RemoveRange(postTags);
            _dataContext.Remove(tag);
            return await _dataContext.SaveChangesAsync() > postTags.Count;

        }

        private async Task AddNewTags(Post post)
        {
            foreach (var tag in post.Tags)
            {
                var existingTag =
                    await _dataContext.Tags.SingleOrDefaultAsync(x => x.Name == tag.TagName);
                if (existingTag != null)
                {
                    continue;
                }

                await _dataContext.Tags.AddAsync(new Tags
                {
                    Name = tag.TagName, CreatedOn = DateTime.UtcNow, CreatorId = post.UserId
                });
            }
        }
    }
}