using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
