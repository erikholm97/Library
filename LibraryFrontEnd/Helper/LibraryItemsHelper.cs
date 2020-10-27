using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library;

namespace LibraryFrontEnd.Helper
{
    public class LibraryItemsHelper
    {
        public string GetType(string type)
        {
            switch (type)
            {
                case "0":
                    type = "Book";

                    break;
                case "1":
                    type = "ReferenceBook";
                    break;
                case "2":
                    type = "DVD";
                    break;
                case "3":
                    type = "AudioBook";
                    break;
            }

            return type;
        }
        /// <summary>
        /// Unsets the borrower values.
        /// </summary>
        /// <param name="libraryItem">Library items with no borrower.</param>
        /// <returns></returns>
        public LibraryItem UnsetBorrower(LibraryItem libraryItem)
        {
            libraryItem.IsBorrowable = false;
            libraryItem.BorrowDate = null;
            libraryItem.Borrower = null;

            return libraryItem;
        }
    }
}
