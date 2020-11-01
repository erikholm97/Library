using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library;

namespace LibraryFrontEnd.Helper
{
    /// <summary>
    /// Helper class for Employees
    /// </summary>
    public class EmployeeHelper
    {
        
        #region Salary Functions

        public decimal salaryCeo = 2.725M;
        public decimal salaryManager = 1.725M;
        public decimal salaryEmployee = 1.125M;

        /// <summary>
        /// Functions that calculate salary before creating the employee.
        /// This rank is used in conjunction with the salary coefficient (The variables above) to calculate the employee’s
        /// </summary>
        /// <param name="rank"></param>
        /// <returns></returns>
        public decimal CalculateCeoSalary(decimal rank)
        {
            //Salary coefficient: 2.725
            rank = rank * salaryCeo;

            return rank;
        }
        public decimal CalculateManagerSalary(decimal rank)
        {
            //Salary coefficient: 1.725
            rank = rank * salaryManager;

            return rank;
        }
        public decimal CalculateEmployeeSalary(decimal number)
        {
            //Salary coefficient: 1.125
            number = number * salaryEmployee;

            return number;
        }

        #endregion

        /// <summary>
        /// Removes manager fields if user is a CEO before adding Employee to db.
        /// </summary>
        /// <param name="employees"></param>
        public Employees RemoveManagerFields(Employees employees)
        {
            employees.ManagerId = null;
            employees.IsManager = false;

            return employees;
        }
    }
}
