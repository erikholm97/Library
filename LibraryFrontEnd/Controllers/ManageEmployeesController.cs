using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library;
using LibraryBackEnd;
using Microsoft.AspNetCore.Http;

namespace LibraryFrontEnd.Controllers
{
    public class ManageEmployeesController : Controller
    {
        private readonly LibraryContext _context;

        public ManageEmployeesController(LibraryContext context)
        {
            _context = context;
        }

        // GET: ManageEmployees
        public async Task<IActionResult> Index()
        {
            //ViewData["CategoryId"] = new SelectList(_context.Employees, "Id", "Id");
            //var item = ViewBag.Id = new SelectList(_context.Employees);
            //Console.WriteLine(item);

           

            return View(await _context.Employees.ToListAsync());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IFormCollection form)
        {
            string getSelector = Request.Form["Id"].ToString();
            int id = int.Parse(getSelector);

            var employee = await _context.Employees.SingleOrDefaultAsync(e => e.Id == id);


            if(employee.IsCEO == true)
            {
                CEOView();
            } 
            else if(employee.IsManager == true)
            {

            }
            else
            {

            }
            

            return View(await _context.Employees.ToListAsync());
        }

        // GET: ManageEmployees
        public async Task<IActionResult> CEOView()
        {
            //ViewData["CategoryId"] = new SelectList(_context.Employees, "Id", "Id");
            //var item = ViewBag.Id = new SelectList(_context.Employees);
            //Console.WriteLine(item);

            var selectManagers = from m in _context.Employees where m.IsManager == true select m;

            return View(selectManagers.ToListAsync());
        }

        // GET: ManageEmployees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employees = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employees == null)
            {
                return NotFound();
            }

            return View(employees);
        }

        // GET: ManageEmployees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ManageEmployees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Salary,IsCEO,IsManager,ManagerId")] Employees employees)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employees);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employees);
        }

        // GET: ManageEmployees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employees = await _context.Employees.FindAsync(id);
            if (employees == null)
            {
                return NotFound();
            }
            return View(employees);
        }

        // POST: ManageEmployees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Salary,IsCEO,IsManager,ManagerId")] Employees employees)
        {
            if (id != employees.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employees);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeesExists(employees.Id))
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
            return View(employees);
        }

        // GET: ManageEmployees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employees = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employees == null)
            {
                return NotFound();
            }

            return View(employees);
        }

        // POST: ManageEmployees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employees = await _context.Employees.FindAsync(id);
            _context.Employees.Remove(employees);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeesExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
