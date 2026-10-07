using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Example5WeekWeb.Data;
using Example5WeekWeb.Models;

namespace Example5WeekWeb.Pages.Students
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _db;

        public EditModel(AppDbContext db) => _db = db;

        [BindProperty]
        public Student Student { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Student = await _db.Students.FindAsync(id);
            if (Student == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            _db.Attach(Student).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(Student.Id)) return NotFound();
                throw;
            }

            return RedirectToPage("/Students/Index");
        }

        private bool StudentExists(int id) => _db.Students.Any(e => e.Id == id);
    }
}
