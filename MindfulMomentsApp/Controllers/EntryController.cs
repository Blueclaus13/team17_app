using Microsoft.AspNetCore.Mvc;
using MindfulMomentsApp.Models;
//using MindfulMomentsApp.Data;

namespace MindfulMomentsApp.Controllers

{   

    public class EntryController : Controller
    {
        // private readonly EntryContext _context;

        // public EntryController(EntryContext context)
        // {
        //     _context = context;
        // }

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