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
        public PostsController(AppDbContext context) => _context = context;

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

        [HttpPost]
        public async Task<IActionResult> Create(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return Ok(post);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Post post)
        {
            var existing = await _context.Posts.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Name = post.Name;
            existing.Description = post.Description;
            existing.ImageUrl = post.ImageUrl;
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