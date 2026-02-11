using Microsoft.AspNetCore.Mvc;
using MindfulMomentsApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using MindfulMomentsApp.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        public IActionResult Create()
            {
                return View(new Entry());
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EntryId,JournalId,Mood,CreatedDate, UpdatedDate, Activity, Description")] Entry entry)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine(error.ErrorMessage);
                        
                    }
                return View(entry);
            }

            entry.CreatedDate = DateTime.UtcNow;
            entry.UpdatedDate = null;
            Console.WriteLine(JsonSerializer.Serialize(entry, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Converters = { new JsonStringEnumConverter() }

                }));
            // _context.Add(entry);
            // await _context.SaveChangesAsync();
            return Redirect("/Journal");
        }

        
        // GET: Journal/Edit/1
         //Update Entries
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Entry entry)
            {
                if (id != entry.EntryId)
                {
                    return BadRequest();
                }

                if (!ModelState.IsValid)
                {
                    return View(entry);
                }

                try
                {
                    entry.UpdatedDate = DateTime.UtcNow;

                    _context.Update(entry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Entries.Any(e => e.EntryId == id))
                        return NotFound();
                    else
                        throw;
                }

                return Redirect("/Journal");
        }

       
       //GET: Journal/Delete/1
       //Delete Entries
       public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entry = await _context.Entries
                .FirstOrDefaultAsync(m => m.EntryId == id);
            if (entry == null)
            {
                return NotFound();
            }

             return View();
        }
        
    }
}