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
using Microsoft.AspNetCore.Http;

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
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["TypeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "orderByEmployee" : "orderByManager";

            var getCeo = from ceo in _context.Employees where ceo.IsCEO == true select ceo;

            var getManager = from manager in _context.Employees where manager.IsManager == true && manager.IsCEO == false select manager;

            var getEmployee = from employee in _context.Employees where employee.IsCEO == false && employee.IsManager == false select employee;


            var employees = new List<Employees>();

            switch (sortOrder)
            {
                case "orderByEmployee":
                    employees = getEmployee.Concat(getCeo).Concat(getManager).ToList();
                    break;
                case "orderByManager":
                    employees = getEmployee.Concat(getCeo).Concat(getManager).ToList();
                    break;
                default:
                    employees = getCeo.Concat(getManager).Concat(getEmployee).ToList();
                    break;
            }

            return View(employees);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IFormCollection form)
        {
            string getSelector = Request.Form["Id"].ToString();
            int id = int.Parse(getSelector);

            var employee = await _context.Employees.SingleOrDefaultAsync(e => e.Id == id);


            if (employee.IsCEO == true)
            {
                return RedirectToAction("CEOView");
            }
            else if (employee.IsManager == true)
            {
                return RedirectToAction("ManagerView");
            }

            return View(await _context.Employees.ToListAsync());

        }

        public async Task<IActionResult> CEOView()
        {
            var selectManagers = from m in _context.Employees where m.IsManager == true select m;

            return View(await selectManagers.ToListAsync());
        }
        public async Task<IActionResult> ManagerView()
        {
            var selectManagers = from m in _context.Employees where m.IsManager == false && m.IsCEO == false select m;

            return View(await selectManagers.ToListAsync());
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

                //If user user has set IsManager to true and IsCEO to true.
                if (employees.IsManager == true && employees.IsCEO == true)
                {
                    ViewBag.ErrorMessage = "You can't create a CEO and Manager at the same time.";

                    return View(employees);
                }

                //If user has made an input less than 1 or higher than 10. 
                if (employees.Salary < 1 || employees.Salary > 10)
                {
                    ViewBag.ErrorMessage = "Input an value between 1-10 for salary.";

                    return View(employees);
                }

                bool ceoExist = await CheckIfCeoExist() ? true : false;

                //Ceo exists in system and User wants to create a new CEO. 
                if (ceoExist && employees.IsCEO == true)
                {
                    ViewBag.ErrorMessage = "There is already an CEO in the library system.";

                    return View(employees);
                }

                //Ceo does not exists in system and User wants to create a new CEO. 
                else if (!ceoExist && employees.IsCEO == true)
                {
                    //Method that removes fields from employees that is not necessary for CEO.
                    employees = helper.RemoveManagerFields(employees);
                    
                    employees.Salary = helper.CalculateCeoSalary(employees.Salary);
                }

                if (employees.IsManager == true && employees.IsCEO == false)
                {
                    employees.Salary = helper.CalculateManagerSalary(employees.Salary);

                    //Check if manager id is null when creating an Id. 
                    if (employees.ManagerId is null)
                    {
                        ViewBag.ErrorMessage = "Input manager Id when creating a manager.";

                        return View(employees);
                    }
                }
                else if(employees.IsManager == false && employees.IsCEO == false)
                {
                    employees.Salary = helper.CalculateEmployeeSalary(employees.Salary);
                }
                
                _context.Add(employees);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(employees);
        }

        

        private async Task<bool> CheckIfCeoExist()
        {
            var ceo = _context.Employees.Count(x => x.IsCEO == true);

            if (ceo > 0)
            {
                return true;
            }

            return false;

        }
        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employees = await _context.Employees.FindAsync(id);
            employees.Salary = 0;

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
                else if (ceo > 0 && employees.IsCEO == true)
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
        #region Actions for Input validation.
        /// <summary>
        /// Actions to redirect the user if an wrongful value is done.
        /// </summary>
        /// <returns></returns>
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

        #endregion
    }
}
