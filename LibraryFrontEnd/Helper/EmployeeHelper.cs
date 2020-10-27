using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryFrontEnd.Helper
{
    /// <summary>
    /// Helper class for Employees
    /// </summary>
    public class EmployeeHelper
    {
        public decimal salaryCeo = 2.725M;
        public decimal salaryManager = 1.725M;
        public decimal salaryEmployee = 1.125M;

        /// <summary>
        /// Functions that calculate salary before creating the employee.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public decimal CalculateCeoSalary(decimal number)
        {
            number = number * salaryCeo;

            return number;
        }
        public decimal CalculateManagerSalary(decimal number)
        {
            number = number * salaryManager;

            return number;
        }
        public decimal CalculateEmployeeSalary(decimal number)
        {
            number = number * salaryEmployee;

            return number;
        }
    }
}
