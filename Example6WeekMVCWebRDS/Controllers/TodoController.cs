using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Example6WeekMVCWebRDS.Data;
using Example6WeekMVCWebRDS.Models;

namespace Example6WeekMVCWebRDS.Controllers
{
    public class TodoController : Controller
    {
        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var todos = await _context.Todos.ToListAsync();
            return View(todos);
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
                _context.Add(todo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(todo);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var todo = await _context.Todos.FindAsync(id.Value);
            if (todo == null) return NotFound();
            return View(todo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TodoItem todo)
        {
            if (id != todo.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(todo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(todo);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var todo = await _context.Todos.FindAsync(id.Value);
            if (todo == null) return NotFound();
            return View(todo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo != null)
            {
                _context.Todos.Remove(todo);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
