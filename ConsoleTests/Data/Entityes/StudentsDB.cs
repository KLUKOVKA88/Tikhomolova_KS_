using Microsoft.EntityFrameworkCore;
using ConsoleTests.Data.Entityes;

namespace ConsoleTests.Data.Entityes
{
    class StudentsDB : DbContext
    {
        public DbSet<Student> Students { get; set; }

        public DbSet<Group> Groups { get; set; }

       public StudentsDB(DbContextOptions<StudentsDB> options) : base(options)  {  }
    }
}
