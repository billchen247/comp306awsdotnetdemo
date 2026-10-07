using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Example5WeekWeb.Data;
using Example5WeekWeb.Models;

namespace Example5WeekWeb.Pages.Students
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _db;

        public CreateModel(AppDbContext db) => _db = db;

        [BindProperty]
        public Student Student { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _db.Students.Add(Student);
            await _db.SaveChangesAsync();

            return RedirectToPage("/Students/Index");
        }
    }
}
