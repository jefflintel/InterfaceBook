using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InterfaceBook.Data;
using InterfaceBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InterfaceBook.Controllers
{
    [Route("user-posts")]
    public class UserPostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public UserPostController(ApplicationDbContext context, ILogger<UserPostController> logger, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        [HttpPost("new-post")]
        public JsonResult NewPost(string text)
        {
            IdentityUser user = Task.Run(async () => { return await _userManager.GetUserAsync(HttpContext.User); }).Result;
            _context.Add(new UserPost
            {
                Text = text,
                User = user,
                Created = DateTime.Now,
                Edited = DateTime.Now
            });
            _context.SaveChanges();
            return new JsonResult(new
            {
                Status = true
            });
        }

        [HttpPost("update-post")]
        public JsonResult Post(string text, int id)
        {
            IdentityUser user = Task.Run(async () => { return await _userManager.GetUserAsync(HttpContext.User); }).Result;

            UserPost post = _context.UserPosts
                .Include(p => p.Comments)
                .Where(p => p.UserPostId == id)
                .First();

            if (user.Id == post.User.Id)
            {
                post.Text = text;
                post.Edited = DateTime.Now;

                _context.Update(post);

                _context.SaveChanges();

            } else
            {
                _logger.LogWarning("User {} attempted to edit a post from user {}", user.Id, post.User.Id);
            }

           return new JsonResult(new
            {
                Status = true
            });
        }

        [HttpGet("delete-comment")]
        public JsonResult DeleteComment(int id)
        {
            IdentityUser user = Task.Run(async () => { return await _userManager.GetUserAsync(HttpContext.User); }).Result;

            var comment = _context.Comments
                .Where(c => c.CommentId == id)
                .First();

            if (user.Id == comment.User.Id)
            {
                _context.Remove(comment);

                _context.SaveChanges();

            }
            else
            {
                _logger.LogWarning("User {} attempted to delete a comment from user {}", user.Id, comment.User.Id);
            }

            return new JsonResult(new
            {
                Status = true
            });
        }

        [HttpGet("delete-post")]
        public JsonResult DeletePost(int id)
        {
            IdentityUser user = Task.Run(async () => { return await _userManager.GetUserAsync(HttpContext.User); }).Result;

            var post = _context.UserPosts
                .Where(p => p.UserPostId == id)
                .First();

            if (user.Id == post.User.Id)
            {

                List<Comment> comments = _context.Comments
                    .Include(comment => comment.User)
                    .Where(comment => comment.UserPost.UserPostId == id)
                    .ToList();

                _context.RemoveRange(comments);

                _context.Remove(post);

                _context.SaveChanges();

            }
            else
            {
                _logger.LogWarning("User {} attempted to delete a post from user {}", user.Id, post.User.Id);
            }

            return new JsonResult(new
            {
                Status = true
            });
        }

        [HttpPost("update-comment")]
        public JsonResult UpdateComment(string text, int id)
        {
            IdentityUser user = Task.Run(async () => { return await _userManager.GetUserAsync(HttpContext.User); }).Result;

            var comment = _context.Comments
                .Where(c => c.CommentId == id)
                .First();

            if (user.Id == comment.User.Id)
            {
                comment.Text = text;
                comment.Edited = DateTime.Now;

                _context.Update(comment);

                _context.SaveChanges();

            }
            else
            {
                _logger.LogWarning("User {} attempted to edit a post from user {}", user.Id, comment.User.Id);
            }

            return new JsonResult(new
            {
                Status = true
            });
        }

        [HttpPost("new-comment")]
        public JsonResult NewComment(int postId, string text)
        {
            IdentityUser user = Task.Run(async () => { return await _userManager.GetUserAsync(HttpContext.User); }).Result;

            UserPost post = _context.UserPosts
                .Include(p => p.Comments)
                .Where(p => p.UserPostId == postId)
                .First();
            post.Comments.Add(new Comment
            {
                Text = text,
                User = user,
                Created = DateTime.Now,
                Edited = DateTime.Now
            });
            _context.SaveChanges();

            /* _context.Add(new Comment
            {
                Text = text,
                User = user,
                UserPost = new UserPost
                {
                    UserPostId = postId
                },
                Created = DateTime.Now,
                Edited = DateTime.Now
            });
            _context.SaveChanges();*/
            return new JsonResult(new
            {
                Status = true
            });
        }

        [HttpGet("get-user-posts")]
        public JsonResult GetUserPosts(int limit, int offset)
        {
            IdentityUser user = Task.Run(async () => { return await _userManager.GetUserAsync(HttpContext.User); }).Result;
            List <UserPost> posts = _context.UserPosts
                .Include(post => post.User)
                .Where(post => post.User.Id == user.Id)
                .OrderByDescending(post => post.Edited)
                .Skip(offset)
                .Take(limit)
                .Select(post => new UserPost
                {
                    UserPostId = post.UserPostId,
                    Text = post.Text,
                    CommentCount = post.Comments.Count(),
                    Created = post.Created,
                    User = post.User,
                    Edited = post.Edited,
                    Editable = user.Id == post.User.Id
                })
                .ToList();

            return new JsonResult(posts);
        }

        [HttpGet("get-comments")]
        public JsonResult GetComments(int userPostID)
        {
            List<Comment> comments = _context.Comments
                .Include(comment => comment.User)
                .Where(comment => comment.UserPost.UserPostId == userPostID)
                .ToList();
            return new JsonResult(comments);
        }
    }
}