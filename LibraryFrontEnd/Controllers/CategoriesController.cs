using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library;
using LibraryBackEnd;
using Microsoft.EntityFrameworkCore.Internal;

namespace LibraryFrontEnd.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly LibraryContext _context;

        public CategoriesController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return View(await _context.Category.ToListAsync());
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                bool exist = await CheckIfCategoryExist(category.CategoryName) ? true : false;

                if (exist)
                {
                    ViewBag.ErrorMessage = "This category already exist. Create another one.";
                }
                else
                {
                    _context.Add(category);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

            }
            return View(category);
        }
        /// <summary>
        /// Checks if categorie already exist.
        /// </summary>
        private async Task<bool> CheckIfCategoryExist(string name)
        {
            var categories = from b in await _context.Category.ToListAsync()
                             where b.CategoryName == name
                             select b;

            if (categories.Count() > 0)
            {
                return true;
            }

            return false;

        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryName")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bool exist = await CheckIfCategoryExist(category.CategoryName) ? true : false;

                    if (exist)
                    {
                        ViewBag.ErrorMessageEdit = "This category already exist. Create another one.";
                    }
                    else
                    {
                        _context.Update(category);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Todo A user should also be able to edit and delete categories. But if a
            //category is referenced in any library item it cannot be deleted until the reference is removed
            //first.
            
            bool exist = await CheckIfReferenceExistInLibraryItem(id) ? true : false;

            if (exist)
            {
                //Item exist cannot remove
                ViewBag.ErrorMessage = "This category already exist. Create another one.";
                return View();
            }
            else
            {
                var category = await _context.Category.FindAsync(id);
                _context.Category.Remove(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
        }

        private async Task<bool> CheckIfReferenceExistInLibraryItem(int id)
        {
            var exist = await _context.LibraryItem.AnyAsync(item => item.Category.Id == id);

            if(exist)
            {
                return true;
            }

            return false;

        }

        private bool CategoryExists(int id)
        {
            return _context.Category.Any(e => e.Id == id);
        }
    }
}
