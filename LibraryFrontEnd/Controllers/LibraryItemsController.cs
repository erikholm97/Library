using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library;
using LibraryBackEnd;
using LibraryFrontEnd.Models;

namespace LibraryFrontEnd.Controllers
{
    public class LibraryItemsController : Controller
    {
        private readonly LibraryContext _context;

        public LibraryItemsController(LibraryContext context)
        {
            _context = context;
        }

        // GET: LibraryItems
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["TypeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "orderByType" : "";
            ViewData["CategorySortParam"] = String.IsNullOrEmpty(sortOrder) ? "orderByCategory" : "";

            try
            {
                var items = from i in _context.LibraryItem.ToList() join c in _context.Category.ToList() on i.CategoryId equals c.Id
                    select i;

                switch (sortOrder)
                {
                    case "orderByType":
                        return View(items.OrderBy(s => s.Type).ToList());
                        
                    case "orderByCategory":
                    default:
                        items = _context.LibraryItem.OrderBy(x => x.Category.CategoryName);
                        return View(items);

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not update");
            }
        }

        // GET: LibraryItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libraryItem = await _context.LibraryItem
                .Include(l => l.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (libraryItem == null)
            {
                return NotFound();
            }

            return View(libraryItem);
        }

        public IActionResult CheckOut(int? id)
        {
            var ids = id;
            return View();
        }
        //public async Task<IActionResult> CheckOut(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var libraryItem = await _context.LibraryItem.FindAsync(id);
        //    if (libraryItem == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Id", libraryItem.CategoryId);
        //    return View(libraryItem);
        //}
        // GET: LibraryItems/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Id");
            return View();
        }

        // POST: LibraryItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CategoryId,Title,Author,Pages,RunTimeMinutes,IsBorrowable,Borrower,BorrowDate,Type")] LibraryItem libraryItem)
        {
            if (ModelState.IsValid)
            {
                switch (libraryItem.Type)
                {
                    case "0":
                        libraryItem.Type = "Book";

                        break;
                    case "1":
                        libraryItem.Type = "ReferenceBook";
                        libraryItem.IsBorrowable = false;
                        break;
                    case "2":
                        libraryItem.Type = "DVD";
                        break;
                    case "3":
                        libraryItem.Type = "AudioBook";
                        break;
                }

                _context.Add(libraryItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Id", libraryItem.CategoryId);

            var items = from i in _context.LibraryItem.ToList()
                        join c in _context.Category.ToList() on i.CategoryId equals c.Id
                        select i;

            return View(items);
        }
        public void AddLibraryItem(int id)
        {
            var items = from i in _context.LibraryItem.ToList()
                        where i.Id == id
                        select i;



            //When editing a library item, if the item is borrowable(IsBorrowable = true), the
            //    application user can check out the item for a customer. When lending an item to a
            //    customer the user enters the customer’s name which will set the Borrower field in the
            //    database, along with the current date which will be set in the BorrowDate field.
        }

        // GET: LibraryItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libraryItem = await _context.LibraryItem.FindAsync(id);
            if (libraryItem == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Id", libraryItem.CategoryId);
            return View(libraryItem);
        }

        // POST: LibraryItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryId,Title,Author,Pages,RunTimeMinutes,IsBorrowable,Borrower,BorrowDate,Type")] LibraryItem libraryItem)
        {
            if (id != libraryItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                switch (libraryItem.Type)
                {
                    case "0":
                        libraryItem.Type = "Book";
                        break;
                    case "1":
                        libraryItem.Type = "ReferenceBook";
                        break;
                    case "2":
                        libraryItem.Type = "DVD";
                        break;
                    case "3":
                        libraryItem.Type = "AudioBook";
                        break;
                }
                try
                {
                    _context.Update(libraryItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibraryItemExists(libraryItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Id", libraryItem.CategoryId);
            return View(libraryItem);
        }

        // GET: LibraryItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libraryItem = await _context.LibraryItem
                .Include(l => l.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (libraryItem == null)
            {
                return NotFound();
            }

            return View(libraryItem);
        }
        
        // POST: LibraryItems/Delete/5
       [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var libraryItem = await _context.LibraryItem.FindAsync(id);
            _context.LibraryItem.Remove(libraryItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LibraryItemExists(int id)
        {
            return _context.LibraryItem.Any(e => e.Id == id);
        }
    }
}
