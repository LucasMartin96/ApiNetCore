using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FirstApi2xd.Domain;

namespace FirstApi2xd.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetPostsAsync();
        Task<Post> GetPostByIdAsync(Guid postId);
        Task<bool> UpdatePostAsync(Post postToUpdate);
        Task<bool> CreatePostAsync(Post post);
        Task<bool> DeletePostAsync(Guid postToDelete);
        Task<bool> UserOwnsPostAsync(Guid postId, string getUserId);
        Task<List<Tags>> GetAllTagsAsync();
        Task<bool> CreateTagAsync(Tags tag);
        Task<Tags> GetTagByNameAsync(string tagName);
        Task<bool> DeleteTagAsync(string tagName);
    }
}