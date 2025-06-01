using Microsoft.EntityFrameworkCore;
using Tutorial10Task.Models;

namespace Tutorial10Task.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PrescriptionMedicament>()
            .HasKey(pm => new { pm.IdMedicament, pm.IdPrescription });

        modelBuilder.Entity<PrescriptionMedicament>()
            .HasOne(pm => pm.Medicament)
            .WithMany(m => m.PrescriptionMedicaments)
            .HasForeignKey(pm => pm.IdMedicament);

        modelBuilder.Entity<PrescriptionMedicament>()
            .HasOne(pm => pm.Prescription)
            .WithMany(p => p.PrescriptionMedicaments)
            .HasForeignKey(pm => pm.IdPrescription);
        
        
        modelBuilder.Entity<Patient>()
            .Property(p => p.IdPatient)
            .ValueGeneratedNever(); 

        
        modelBuilder.Entity<Doctor>().HasData(
            new Doctor
            {
                IdDoctor = 1,
                FirstName = "Gregory",
                LastName = "House",
                Email = "house@example.com"
            },
            new Doctor
            {
                IdDoctor = 2,
                FirstName = "Meredith",
                LastName = "Grey",
                Email = "grey@example.com"
            }
        );

 
        modelBuilder.Entity<Patient>().HasData(
            new Patient
            {
                IdPatient = 1,
                FirstName = "Jacob",
                LastName = "Kociak",
                Birthdate = new DateTime(1980, 1, 1)
            },
            new Patient
            {
                IdPatient = 2,
                FirstName = "Ania",
                LastName = "Nowak",
                Birthdate = new DateTime(1995, 5, 20)
            }
        );

        modelBuilder.Entity<Medicament>().HasData(
            new Medicament
            {
                IdMedicament = 1,
                Name = "Ibuprofen",
                Description = "Painkiller",
                Type = "Tablet"
            },
            new Medicament
            {
                IdMedicament = 2,
                Name = "Paracetamol",
                Description = "Fever reducer",
                Type = "Tablet"
            },
            new Medicament
            {
                IdMedicament = 3,
                Name = "Amoxicillin",
                Description = "Antibiotic",
                Type = "Capsule"
            }
        );
    }
}