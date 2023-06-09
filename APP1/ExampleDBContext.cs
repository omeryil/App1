﻿using APP1.Models;
using Microsoft.EntityFrameworkCore;

namespace APP1
{
    public class ExampleDBContext:DbContext
    {
        public ExampleDBContext(DbContextOptions<ExampleDBContext> options)
               : base(options)
        { }
        public DbSet<user> user { get; set; }
        public DbSet<role> role { get; set; }
        public DbSet<tests> test { get; set; }
        public DbSet<Uretim> Uretim { get; set; }
        public DbSet<permission> permission { get; set; }
        public DbSet<urls> urls { get; set; }
    }
}
