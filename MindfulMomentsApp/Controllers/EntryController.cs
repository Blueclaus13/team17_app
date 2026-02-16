using Microsoft.AspNetCore.Mvc;
using MindfulMomentsApp.Models;
using Microsoft.EntityFrameworkCore;
using MindfulMomentsApp.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace MindfulMomentsApp.Controllers
{
    /// <summary>
    /// Manages CRUD operations for journal entries. All actions require user authentication.
    /// </summary>
    [Authorize]
    public class EntryController : Controller
    {
        private readonly AppDbContext _context;

        public EntryController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays the form for creating a new journal entry.
        /// </summary>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Processes the new entry submission, associates it with the user's journal, and saves to database.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Entry entry)
        {
            var journalId = await GetCurrentUserJournalIdAsync();
            if (journalId == null)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
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

        /// <summary>
        /// Displays the edit form for an existing entry that belongs to the current user.
        /// </summary>
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

        /// <summary>
        /// Processes the edited entry submission and updates it in the database.
        /// </summary>
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

            return Redirect("/Journal");
        }

        /// <summary>
        /// Displays the confirmation page before deleting an entry.
        /// </summary>
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

        /// <summary>
        /// Permanently deletes the entry from the database after user confirmation.
        /// </summary>
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

        /// <summary>
        /// Checks if an entry with the specified ID exists in the database.
        /// </summary>
        private bool EntryExists(int id)
        {
            return _context.Entries.Any(e => e.EntryId == id);
        }

        /// <summary>
        /// Retrieves the journal ID for the current user, creating a new journal if one doesn't exist.
        /// </summary>
        private async Task<int?> GetCurrentUserJournalIdAsync()
        {
            var identifier = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(identifier))
                return null;

            User? user = null;

            if (Guid.TryParse(identifier, out var userGuid))
            {
                user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == userGuid);
            }
            else
            {
                user = await _context.Users
                    .FirstOrDefaultAsync(u =>
                        u.GoogleId == identifier ||
                        u.Email == identifier);
            }

            if (user == null)
                return null;

            var journal = await _context.Journals
                .FirstOrDefaultAsync(j => j.UserId == user.UserId);

            if (journal == null)
            {
                journal = new Journal
                {
                    UserId = user.UserId,
                    JournalName = string.IsNullOrWhiteSpace(user.FirstName)
                        ? "My Journal"
                        : $"{user.FirstName}'s Journal"
                };

                _context.Journals.Add(journal);
                await _context.SaveChangesAsync();
            }

            return journal.JournalId;
        }

    }
}
