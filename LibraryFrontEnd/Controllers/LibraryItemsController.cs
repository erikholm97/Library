using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library;
using LibraryBackEnd;
using LibraryFrontEnd.Helper;
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
            ViewData["CategorySortParm"] = String.IsNullOrEmpty(sortOrder) ? "orderByCategory" : "";

            try
            {
                var items = from i in _context.LibraryItem.ToList()
                            join c in _context.Category.ToList() on i.CategoryId equals c.Id
                            select i;

             
                if (sortOrder is null)
                {
                    //AppSettings to persist the sortOrder in the current session.
                    sortOrder = AppSettings.ReadSetting();
                }

                switch (sortOrder)
                {
                    //Can be changed to type by the user.
                    case "orderByType":
                        //Stores option to persist through the application in appsettings.
                        AppSettings.UpdateAppSettings("orderByType");
                        return View(items.OrderBy(s => s.Type).ToList());

                    //Listing library sorted by Category Name by default (Hence Listing library items should be sorted by Category Name).
                    case "orderByCategory":
                    default:
                        //Stores option to persist through the application in appsettings.
                        AppSettings.UpdateAppSettings("orderByCategory");
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

        public async Task<IActionResult> CheckOut(int? id)
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
        ////If user checks in item (Pressed check in in index view) the CheckIn action is again displayed since the item is now borrowable and whithout a borrower.
        public async Task<IActionResult> CheckIn(int? id)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn(int? id, [Bind("Id,CategoryId,Title,Author,Pages,RunTimeMinutes,IsBorrowable,Borrower,BorrowDate,Type")] LibraryItem libraryItem)
        {
            ModelState.Remove("Category");

            if (id != libraryItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    LibraryItemsHelper helper = new LibraryItemsHelper();

                    //Function that unsets the values in the fields Borrower and BorrowDate. (The operation that unsets the values in the fields Borrower and BorrowDate.
                    libraryItem = helper.UnsetBorrower(libraryItem);

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
        /// <summary>
        /// CheckOut for libraryitem in which borrowable is set to true.
        /// </summary>
        /// <param name="id">Id of item that is to be checked out for lending</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut(int? id, [Bind("Id,CategoryId,Title,Author,Pages,RunTimeMinutes,IsBorrowable,Borrower,BorrowDate,Type")] LibraryItem libraryItem)
        {
            //When lending an item to customer the user enters the customer’s name (in this case the user can also choose what date is to be set in borrower field in the view).
            ModelState.Remove("Category");
            if (id != libraryItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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
            ModelState.Remove("Category");
            if (ModelState.IsValid)
            {
                LibraryItemsHelper helper = new LibraryItemsHelper();

                libraryItem.Type = helper.GetType(libraryItem.Type);

                
                if (libraryItem.CategoryId == null)
                {
                    ViewBag.ErrorMessage = "Please create an category id.";

                    return View(libraryItem);
                }
                //Since ReferenceBook is a book you read at the library but can’t be borrowed home
                if (libraryItem.Type == "ReferenceBook")
                {

                    libraryItem.IsBorrowable = false;
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
            ModelState.Remove("Category");
            if (id != libraryItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                LibraryItemsHelper helper = new LibraryItemsHelper();

                libraryItem.Type = helper.GetType(libraryItem.Type);

                if (libraryItem.Type == "ReferenceBook")
                {
                    libraryItem.IsBorrowable = false;
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
