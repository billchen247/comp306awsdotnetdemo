using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Example5WeekWeb.Data;
using Example5WeekWeb.Models;

namespace Example5WeekWeb.Pages.Students
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;

        public IndexModel(AppDbContext db)
        {
            _db = db;
        }

        public IList<Student> Students { get; set; } = new List<Student>();
        public async Task OnGetAsync()
        {
            Students = await _db.Students.OrderBy(s => s.Name).ToListAsync();
        }
    }
}
