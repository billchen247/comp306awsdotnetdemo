using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Example4Week4RDS.Data;

namespace Example4Week4RDS.Controllers
{
    // Simple REST controller demonstrating CRUD operations with EF Core.
    // Routes:
    // GET    /api/todo         -> list all todos
    // GET    /api/todo/{id}    -> get one todo
    // POST   /api/todo         -> create todo
    // PUT    /api/todo/{id}    -> update todo
    // DELETE /api/todo/{id}    -> delete todo
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;
        }

        // Return all todos. AsNoTracking improves read performance since we don't update entities here.
        [HttpGet]
        public async Task<IEnumerable<TodoItem>> GetAll()
        {
            return await _context.TodoItems.AsNoTracking().ToListAsync();
        }

        // Return a single todo by id.
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> Get(int id)
        {
            var item = await _context.TodoItems.FindAsync(id);
            if (item == null) return NotFound();
            return item;
        }

        // Create a new todo. Returns 201 Created with a Location header.
        [HttpPost]
        public async Task<ActionResult<TodoItem>> Create(TodoItem item)
        {
            _context.TodoItems.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
        }

        // Update an existing todo. Simple pattern: fetch, modify, save.
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TodoItem input)
        {
            if (id != input.Id) return BadRequest();

            var item = await _context.TodoItems.FindAsync(id);
            if (item == null) return NotFound();

            item.Title = input.Title;
            item.IsCompleted = input.IsCompleted;
            item.DueAt = input.DueAt;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Delete by id.
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.TodoItems.FindAsync(id);
            if (item == null) return NotFound();

            _context.TodoItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
