using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostManager.API.Data;
using PostManager.API.Models;

namespace PostManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly Cloudinary _cloudinary;

        public PostsController(AppDbContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            string? search, string? sort,
            int page = 1, int pageSize = 4)
        {
            var query = _context.Posts.AsQueryable();

            // Filter
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.ToLower().Contains(search.ToLower()));

            // Sort
            if (sort == "asc") query = query.OrderBy(p => p.Name);
            else if (sort == "desc") query = query.OrderByDescending(p => p.Name);

            // Total count (for pagination)
            var total = await query.CountAsync();

            // Apply paging
            var posts = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { total, posts });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
                return NotFound();

            return Ok(post);
        }

        //[HttpPost]
        //public async Task<IActionResult> Create(Post post)
        //{
        //    _context.Posts.Add(post);
        //    await _context.SaveChangesAsync();
        //    return Ok(post);
        //}

        [HttpPost]
        [RequestSizeLimit(10_000_000)] // giới hạn 10MB
        public async Task<IActionResult> Create([FromForm] string name, [FromForm] string description, IFormFile? imageFile, [FromForm] string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
                return BadRequest("Name and description are required.");

            string finalImageUrl;

            // Nếu user upload file thật → upload lên Cloudinary
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream()),
                    Folder = "postmanager_uploads"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                finalImageUrl = uploadResult.SecureUrl?.AbsoluteUri ?? "";
            }
            // Nếu user nhập link hợp lệ
            else if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                finalImageUrl = imageUrl;
            }
            // Nếu không chọn ảnh → dùng default
            else
            {
                finalImageUrl = "https://placehold.co/600x400?text=No+Image+Available";
            }

            var post = new Post
            {
                Name = name,
                Description = description,
                ImageUrl = finalImageUrl,
                CreatedAt = DateTime.UtcNow
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return Ok(post);
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(int id, Post post)
        //{
        //    var existing = await _context.Posts.FindAsync(id);
        //    if (existing == null) return NotFound();
        //    existing.Name = post.Name;
        //    existing.Description = post.Description;
        //    existing.ImageUrl = post.ImageUrl;
        //    await _context.SaveChangesAsync();
        //    return Ok(existing);
        //}

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] string name, [FromForm] string description, IFormFile? imageFile, [FromForm] string? imageUrl)
        {
            var existing = await _context.Posts.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Name = name;
            existing.Description = description;

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream()),
                    Folder = "postmanager_uploads"
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                existing.ImageUrl = uploadResult.SecureUrl?.AbsoluteUri ?? existing.ImageUrl;
            }
            else if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                existing.ImageUrl = imageUrl;
            }

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _context.Posts.FindAsync(id);
            if (existing == null) return NotFound();
            _context.Posts.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}