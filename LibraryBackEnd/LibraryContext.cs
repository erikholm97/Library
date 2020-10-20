using Library;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryBackEnd
{
    public class LibraryContext : DbContext
    {
        public DbSet<Category> Category { get; set; }

        public DbSet<Employees> Employees { get; set; }

        public DbSet<LibraryItem> LibraryItem { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=Blogging;Integrated Security=True");
        }
    }
}



