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

namespace LibraryFrontEnd.Controllers
{
    public class EmployeesController : Controller
    {
        
        private readonly LibraryContext _context;

        public EmployeesController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var getCeo = from ceo in _context.Employees where ceo.IsCEO == true select ceo;

            var getManager = from manager in _context.Employees where manager.IsManager == true && manager.IsCEO == false select manager;

            var getEmployee = from employee in _context.Employees where employee.IsCEO == false && employee.IsManager == false select employee;

            IEnumerable<Employees> groupedList = getCeo.Concat(getManager).Concat(getEmployee);

            var list = groupedList.ToList();
            return View(groupedList.ToList());
        }

        // GET: Employees/Details/5
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

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Salary,IsCEO,IsManager,ManagerId")] Employees employees)
        {
            if (ModelState.IsValid)
            {
                EmployeeHelper helper = new EmployeeHelper();

                if (employees.Salary < 1 || employees.Salary > 10)
                {
                    return RedirectToAction("InformationInput");
                }

                var ceo = _context.Employees.Count(x => x.IsCEO == true);

                
                if (ceo == 0)
                {
                    employees.Salary = helper.CalculateCeoSalary(employees.Salary);

                } 
                else if(ceo > 0 && employees.IsCEO == true)
                {
                    ViewBag.Error = "There is already an CEO in the library system.";
                    
                    return RedirectToAction("InformationEmployees");
                }
                else if (employees.IsManager == true && employees.IsCEO == false)
                {
                    employees.Salary = helper.CalculateManagerSalary(employees.Salary);
                }
                else
                {
                    employees.Salary = helper.CalculateEmployeeSalary(employees.Salary);
                }
                
                _context.Add(employees);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employees);
        }

        public IActionResult InformationEmployees()
        {
            ViewData["Message"] = "There is already an CEO.";

            return View();
        }
        public IActionResult InformationInput()
        {
            ViewData["Message"] = "Input an value between 1-10 for salary.";

            return View();
        }
        // GET: Employees/Edit/5
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

        // POST: Employees/Edit/5
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

        // GET: Employees/Delete/5
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

        // POST: Employees/Delete/5
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
