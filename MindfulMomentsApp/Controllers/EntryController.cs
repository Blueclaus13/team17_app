using Microsoft.AspNetCore.Mvc;
using MindfulMomentsApp.Models;
using Microsoft.EntityFrameworkCore;
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
            // (You can implement later)
            return View();
        }

        // GET: /Entry/Create
        public IActionResult Create()
        {
            return View(new Entry());
        }

        // POST: /Entry/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EntryId,JournalId,Mood,Activity,Description")] Entry entry)
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

            _context.Entries.Add(entry);
            await _context.SaveChangesAsync();

            Console.WriteLine(JsonSerializer.Serialize(entry, new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            }));

            return Redirect("/Journal");
        }

        //GET: /Entry/Edit/1  (loads the form)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var entry = await _context.Entries.FirstOrDefaultAsync(e => e.EntryId == id);
            if (entry == null)
                return NotFound();

            return View(entry);
        }

        //POST: /Entry/Edit 
        [HttpPost("Entry/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,[Bind("EntryId,JournalId,Mood,Activity,Description")] Entry entry)
        {
            if (id != entry.EntryId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(entry);

            var existing = await _context.Entries.FirstOrDefaultAsync(e => e.EntryId == id);
            if (existing == null)
                return NotFound();

            existing.Mood = entry.Mood;
            existing.Activity = entry.Activity;
            existing.Description = entry.Description;
            existing.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Redirect("/Journal");
        }

        // GET: /Entry/Delete/1
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var entry = await _context.Entries.FirstOrDefaultAsync(m => m.EntryId == id);
            if (entry == null)
                return NotFound();

            return View(entry);
        }

        // POST: /Entry/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int entryId)
        {
            var entry = await _context.Entries.FindAsync(entryId);
            if (entry == null)
                return NotFound();

            _context.Entries.Remove(entry);
            await _context.SaveChangesAsync();

            return Redirect("/Journal");
        }
    }
}
