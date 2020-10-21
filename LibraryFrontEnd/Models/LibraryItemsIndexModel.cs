using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryFrontEnd.Models
{
    public class LibraryItemsIndexModel : LibraryItem
    {
        public bool IsChecked { get; set; }
    }
}
