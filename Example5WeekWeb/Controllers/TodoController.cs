using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Example5WeekWeb.Data;
using Example5WeekWeb.Models;
using System.Threading.Tasks;
using System.Linq;

namespace Example5WeekWeb.Controllers
{
    // Classic MVC controller for Todo using in-memory EF Core
    public class TodoController : Controller
    {
        private readonly AppDbContext _db;

        public TodoController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _db.Todos.OrderBy(t => t.IsDone).ThenBy(t => t.DueDate).ToListAsync();
            return View(items);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TodoItem todo)
        {
            if (ModelState.IsValid)
            {
                _db.Todos.Add(todo);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(todo);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var todo = await _db.Todos.FindAsync(id);
            if (todo == null) return NotFound();
            return View(todo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TodoItem todo)
        {
            if (id != todo.Id) return BadRequest();
            if (ModelState.IsValid)
            {
                _db.Update(todo);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(todo);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var todo = await _db.Todos.FindAsync(id);
            if (todo == null) return NotFound();
            return View(todo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var todo = await _db.Todos.FindAsync(id);
            if (todo != null)
            {
                _db.Todos.Remove(todo);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
