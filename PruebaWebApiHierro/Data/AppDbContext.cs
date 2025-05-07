using Microsoft.EntityFrameworkCore;
using PruebaWebApiHierro.Models;

namespace PruebaWebApiHierro.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Tambo> Tambos { get; set; }
        public DbSet<Asignacion> Asignaciones { get; set; }
        public DbSet<Contacto> Contactos { get; set; }
        public DbSet<Paciente> Pacientes { get; set; } // 👈 nuevo DbSet


        // Vistas
        public DbSet<GestorDisponible> GestoresDisponibles { get; set; }
        public DbSet<TamboDisponible> TambosDisponibles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tabla real
            modelBuilder.Entity<Asignacion>().ToTable("Asignaciones");
            modelBuilder.Entity<Paciente>().ToTable("Pacientes");

            modelBuilder.Entity<Asignacion>()
    .HasOne(a => a.Gestor)
    .WithMany()
    .HasForeignKey(a => a.GestorId);

            modelBuilder.Entity<Asignacion>()
                .HasOne(a => a.Tambo)
                .WithMany()
                .HasForeignKey(a => a.TamboId);


            modelBuilder.Entity<Paciente>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Contacto>()
            .HasOne(c => c.Paciente)
            .WithMany(p => p.Contactos)
            .HasForeignKey(c => c.PacienteId)
            .OnDelete(DeleteBehavior.Cascade);


            // Vistas (sin clave primaria)
            modelBuilder.Entity<GestorDisponible>()
                .ToView("GestoresDisponibles")
                .HasNoKey();

            modelBuilder.Entity<TamboDisponible>()
                .ToView("TambosDisponibles")
                .HasNoKey();
        }
    }
}
