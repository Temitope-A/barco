using BarcoRota.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BarcoRota.Client.Models
{
    public class BarcoContext : DbContext
    {
        public DbSet<BarcoJob> BarcoJobs { get; set; }
        public DbSet<BarcoMember> BarcoMembers { get; set; }
        public DbSet<BarcoShift> BarcoShifts { get; set; }
        public DbSet<WorkPackage> WorkPackages { get; set; }

        //static BarcoContext(){
        //    using (var context = new BarcoContext())
        //    {
        //        context.Database.Migrate();
        //    }
        //}

        public BarcoContext(DbContextOptions<BarcoContext> options)
            : base(options)
        {
            Database.Migrate();
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    //optionsBuilder.UseSqlite();
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Define the Table(s) and References to be created automatically
            modelBuilder.Entity<BarcoMember>(b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e => e.Id).ValueGeneratedOnAdd();
                b.Property(e => e.Name).IsRequired().HasMaxLength(255);
                b.Property(e => e.NickName).IsRequired().HasMaxLength(15);
                b.Property(e => e.RotaStatus).IsRequired().HasDefaultValue(RotaStatus.Inactive);
                b.ToTable("BarcoMembers");
            });

            modelBuilder.Entity<WorkPackage>(b =>
            {
                b.HasKey(w => w.Id);
                b.Property(w => w.Id).ValueGeneratedOnAdd();
                b.Property(w => w.Name).IsRequired();
                b.HasOne(w => w.Manager).WithMany().OnDelete(DeleteBehavior.SetNull);
                b.ToTable("WorkPackages");
            });

            modelBuilder.Entity<BarcoJob>(b =>
            {
                b.HasKey(j => j.Id);
                b.Property(j => j.Id).ValueGeneratedOnAdd();
                b.Property(j => j.Created).IsRequired();
                b.Property(j => j.StartDateTime).IsRequired();
                b.Property(j => j.EndDateTime).IsRequired();
                b.Property(j => j.JobCapacity).IsRequired();
                b.Property(j => j.JobType).IsRequired();
                b.Property(j => j.JobCapacity).IsRequired();
                b.HasOne(j => j.WorkPackage).WithMany(w=>w.Jobs).HasForeignKey(j=>j.WorkPackageId).OnDelete(DeleteBehavior.Cascade);
                b.ToTable("BarcoJobs");
            });

            modelBuilder.Entity<BarcoShift>(b =>
            {
                b.HasKey(s => s.Id);
                b.Property(s => s.Id).ValueGeneratedOnAdd();
                b.Property(s => s.ShiftStatus).IsRequired();
                b.HasOne(s => s.BarcoMember).WithMany().IsRequired().OnDelete(DeleteBehavior.Cascade);
                b.HasOne(s => s.BarcoJob).WithMany(j=>j.Shifts).IsRequired().OnDelete(DeleteBehavior.Cascade);
                b.ToTable("BarcoShifts");
            });


        }

        public async Task<BarcoMember> GetMemberAsync(string userName)
        {
            var member = await BarcoMembers.SingleOrDefaultAsync(m => m.UserName == userName);
            return member;
        }

    }
}
