using Microsoft.AspNetCore.Mvc;
using MindfulMomentsApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using MindfulMomentsApp.Data;
using MindfulMomentsApp.Models;

namespace MindfulMomentsApp.Controllers
{   

    public class EntryController : Controller
    {
         private readonly AppDbContext _context;

        public EntryController(AppDbContext context)
        {
            _context = context;
        }

         // GET: /Dashboard
        public IActionResult Dashboard()
            {
                return View();
            }


        // GET: /Journal/Details/1
        public IActionResult Details(int? id)
            {
                // if (id == null)
                // {
                //     return NotFound();
                // }

                // var entry = _context.Entries.Find(id);
                // if (entry == null)
                // {
                //     return NotFound();
                // }
                 return View();
            }       

        // GET: /AddEntry
        //Post Entries
        public IActionResult Create()
            {
                    return View();
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EntryId,JournalId,Mood,CreatedDate, UpdatedDate, Activity, Description")] Entry entry)
        {
            if (!ModelState.IsValid)
            {
                return View(entry);
            }

            entry.CreatedDate = DateTime.UtcNow;
            entry.UpdatedDate = null;

            _context.Add(entry);
            await _context.SaveChangesAsync();
            return Redirect("/Journal");
        }

        
        // GET: Journal/Edit/1
         //Update Entries
         public async Task<IActionResult> Edit(int? id)
        {
            // if (id == null)
            // {
            //     return NotFound();
            // }

            // var entry = await _context.Entries.FindAsync(id);
            // if (entry == null)
            // {
            //     return NotFound();
            // }
             return View();
        }
       
       //GET: Journal/Delete/1
       //Delete Entries
       public async Task<IActionResult> Delete(int? id)
        {
            // if (id == null)
            // {
            //     return NotFound();
            // }

            // var entry = await _context.Entries
            //     .FirstOrDefaultAsync(m => m.Id == id);
            // if (entry == null)
            // {
            //     return NotFound();
            // }

             return View();
        }
        
    }
}