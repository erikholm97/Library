using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library;
using LibraryBackEnd;

namespace LibraryFrontEnd.Controllers
{
    public class EmployeesController : Controller
    {
        private decimal salaryCeo = 2.725M;
        private decimal salaryManager = 1.725M;
        private decimal employeeSalary = 1.125M;

        private readonly LibraryContext _context;

        public EmployeesController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var getCeo = from ceo in _context.Employees where ceo.IsCEO == true select ceo;

            var getEmployee = from employee in _context.Employees where employee.IsCEO == false && employee.IsManager == false select employee;

            var getManager = from manager in _context.Employees where manager.IsManager == true && manager.IsCEO == false select manager;

            IEnumerable<Employees> groupedList = getCeo.Concat(getCeo).Concat(getManager).Concat(getEmployee);
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

                if (employees.Salary < 1 || employees.Salary > 10)
                {
                    return RedirectToAction("InformationInput");
                }

                var ceo = _context.Employees.Count(x => x.IsCEO == true);

                //Todo Kolla om Man

                /*
                 Create, update and delete employees
    
                • You should not be able to delete a manger or CEO that is managing another
                employee
                • CEO can manage managers but not employees
                • Managers can manage other managers and employees
                • No one can manage the CEO
            
                 */

                if (ceo == 0)
                {
                    employees.Salary = employees.Salary * salaryCeo; // calculate ceo salary

                } else if(ceo > 0 && employees.IsCEO == true)
                {
                    ViewBag.Error = "There is already an CEO in the library system.";
                    
                    return RedirectToAction("InformationEmployees");
                }
                else if (employees.IsManager == true && employees.IsCEO == false)
                {
                    employees.Salary = employees.Salary * salaryManager;
                }
                else
                {
                    employees.Salary = employees.Salary * employeeSalary;
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
