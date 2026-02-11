using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MindfulMomentsApp.Data;
using MindfulMomentsApp.Models;
using System.Security.Claims;

namespace MindfulMomentsApp.Controllers
{
    public class EntryController : Controller
    {
        private readonly AppDbContext _context;

        public EntryController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var journalId = await GetCurrentUserJournalIdAsync();
            if (journalId == null)
            {
                return View(new List<Entry>());
            }

            var entries = await _context.Entries
                .Where(e => e.JournalId == journalId.Value)
                .OrderByDescending(e => e.CreatedDate)
                .ToListAsync();

            return View(entries);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journalId = await GetCurrentUserJournalIdAsync();

            var entry = await _context.Entries
                .FirstOrDefaultAsync(m => m.EntryId == id && m.JournalId == journalId);

            if (entry == null)
            {
                return NotFound();
            }

            return View(entry);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Entry entry)
        {
            var journalId = await GetCurrentUserJournalIdAsync();
            if (journalId == null)
            {
                ModelState.AddModelError(string.Empty, "No journal is associated with the current user.");
                return View(entry);
            }

            if (!ModelState.IsValid)
            {
                return View(entry);
            }

            entry.JournalId = journalId.Value;
            entry.CreatedDate = DateTime.UtcNow;
            entry.UpdatedDate = null;

            _context.Add(entry);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journalId = await GetCurrentUserJournalIdAsync();

            var entry = await _context.Entries
                .FirstOrDefaultAsync(e => e.EntryId == id && e.JournalId == journalId);
            if (entry == null)
            {
                return NotFound();
            }

            return View(entry);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Entry entry)
        {
            if (id != entry.EntryId)
            {
                return NotFound();
            }

            var journalId = await GetCurrentUserJournalIdAsync();
            if (journalId == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(entry);
            }

            try
            {
                entry.JournalId = journalId.Value;
                entry.UpdatedDate = DateTime.UtcNow;
                _context.Update(entry);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntryExists(entry.EntryId))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journalId = await GetCurrentUserJournalIdAsync();

            var entry = await _context.Entries
                .FirstOrDefaultAsync(m => m.EntryId == id && m.JournalId == journalId);

            if (entry == null)
            {
                return NotFound();
            }

            return View(entry);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var journalId = await GetCurrentUserJournalIdAsync();

            var entry = await _context.Entries
                .FirstOrDefaultAsync(e => e.EntryId == id && e.JournalId == journalId);
            if (entry != null)
            {
                _context.Entries.Remove(entry);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EntryExists(int id)
        {
            return _context.Entries.Any(e => e.EntryId == id);
        }

        private async Task<int?> GetCurrentUserJournalIdAsync()
        {
            var externalId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                             ?? User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(externalId))
            {
                return null;
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.GoogleId == externalId);

            if (user == null)
            {
                return null;
            }

            var journal = await _context.Journals
                .FirstOrDefaultAsync(j => j.UserId == user.UserId);

            return journal?.JournalId;
        }
    }
}