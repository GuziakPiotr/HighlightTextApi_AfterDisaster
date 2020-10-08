using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;

namespace TextHighlightCore.DataAccessLayer
{
    public class HighlightContext : DbContext
    {
        public HighlightContext() : base("Hightlight Context")
        {
            Database.CreateIfNotExists();
        }

        public DbSet<ColorRule> ColorRules { get; set; }
    }
}
