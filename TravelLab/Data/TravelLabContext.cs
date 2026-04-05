using Microsoft.EntityFrameworkCore;
using TravelLab.Models;

namespace TravelLab.Data
{
    public class TravelLabContext : DbContext
    {
        public TravelLabContext(DbContextOptions<TravelLabContext> options) : base(options) { }

        public DbSet<Cliente> Clienti { get; set; }
        public DbSet<Viaggio> Viaggi { get; set; }
        public DbSet<Prenotazione> Prenotazioni { get; set; }
        public DbSet<Fattura> Fatture { get; set; }
        public DbSet<Servizio> Servizi { get; set; }
        public DbSet<Volo> Voli { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Biglietto> Biglietti { get; set; }
        public DbSet<Agenzia> Agenzie { get; set; }
        public DbSet<Mezzo> Mezzi { get; set; }
        public DbSet<Treno> Treni { get; set; }
        public DbSet<Nave> Navi { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurazioni esistenti
            modelBuilder.Entity<Prenotazione>()
                .HasOne(p => p.Cliente)
                .WithMany(c => c.Prenotazioni)
                .HasForeignKey(p => p.ClienteId);

            modelBuilder.Entity<Prenotazione>()
                .HasOne(p => p.Viaggio)
                .WithMany()
                .HasForeignKey(p => p.ViaggioId);

            modelBuilder.Entity<Fattura>()
                .HasOne(f => f.Prenotazione)
                .WithOne(p => p.Fattura)
                .HasForeignKey<Fattura>(f => f.PrenotazioneId);

            modelBuilder.Entity<Biglietto>()
                .HasOne(b => b.Prenotazione)
                .WithMany(p => p.Biglietti)
                .HasForeignKey(b => b.PrenotazioneId);

            modelBuilder.Entity<Biglietto>()
                .HasOne(b => b.Servizio)
                .WithMany()
                .HasForeignKey(b => b.ServizioId);

            modelBuilder.Entity<Volo>()
                .HasOne(v => v.Servizio)
                .WithOne(s => s.Volo)
                .HasForeignKey<Volo>(v => v.ServizioId);

            modelBuilder.Entity<Hotel>()
                .HasOne(h => h.Servizio)
                .WithOne(s => s.Hotel)
                .HasForeignKey<Hotel>(h => h.ServizioId);

            // Nuove configurazioni per Treno e Nave
            modelBuilder.Entity<Treno>()
                .HasOne(t => t.Servizio)
                .WithOne(s => s.Treno)
                .HasForeignKey<Treno>(t => t.IdServizio);

            modelBuilder.Entity<Nave>()
                .HasOne(n => n.Servizio)
                .WithOne(s => s.Nave)
                .HasForeignKey<Nave>(n => n.IdServizio);
            modelBuilder.Entity<Treno>().HasKey(t => t.IdServizio);
            modelBuilder.Entity<Nave>().HasKey(n => n.IdServizio);
        }
    }
}