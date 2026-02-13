using Microsoft.AspNetCore.Mvc;
using MindfulMomentsApp.Models;
using Microsoft.EntityFrameworkCore;
using MindfulMomentsApp.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace MindfulMomentsApp.Controllers
{
    // Reqires user to be authenticated to access any actions in this controller
    [Authorize]
    public class EntryController : Controller
    {
        private readonly AppDbContext _context;

        public EntryController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            return View();
        }

        // POST: /Entry/Create
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
            return Redirect("/Journal");
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

        // POST: /Entry/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Entry entry)
        {
            if (id != entry.EntryId)
                return NotFound();

            var journalId = await GetCurrentUserJournalIdAsync();
            if (journalId == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(entry);

            var existing = await _context.Entries
                .FirstOrDefaultAsync(e => e.EntryId == id && e.JournalId == journalId.Value);

            if (existing == null)
                return NotFound();

            existing.Mood = entry.Mood;
            existing.Activity = entry.Activity;
            existing.Description = entry.Description;   // DB will update this
            existing.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Redirect("/Journal");
        }

        //GET: /Entry/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var journalId = await GetCurrentUserJournalIdAsync();

            var entry = await _context.Entries
                .FirstOrDefaultAsync(m => m.EntryId == id && m.JournalId == journalId);
            if (entry == null)
                return NotFound();

            return View(entry);
        }
        // POST: /Entry/Delete/DeleteConfirmed
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

            return Redirect("/Journal");
        }

        //Post: /Entry/Details
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
                .FirstOrDefaultAsync(u => u.GoogleId == externalId || u.Email == externalId);

            if (user == null)
            {
                return null;
            }

            var journal = await _context.Journals
                .FirstOrDefaultAsync(j => j.UserId == user.UserId);

            if (journal == null)
            {
                var journalName = string.IsNullOrWhiteSpace(user.FirstName)
                    ? "My Journal"
                    : $"{user.FirstName}'s Journal";

                journal = new Journal
                {
                    UserId = user.UserId,
                    JournalName = journalName
                };

                _context.Journals.Add(journal);
                await _context.SaveChangesAsync();
            }

            return journal.JournalId;
        }
    }
}
