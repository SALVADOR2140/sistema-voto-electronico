using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaVoto.Modelos;

    public class SistemaVotoElectronicoApiContext : DbContext
    {
        public SistemaVotoElectronicoApiContext (DbContextOptions<SistemaVotoElectronicoApiContext> options)
            : base(options)
        {
        }

        public DbSet<SistemaVoto.Modelos.Candidato> Candidatos { get; set; } = default!;

public DbSet<SistemaVoto.Modelos.EventoElectoral> EventosElectorales { get; set; } = default!;

public DbSet<SistemaVoto.Modelos.ListaPolitica> ListasPoliticas { get; set; } = default!;

public DbSet<SistemaVoto.Modelos.RolUsuario> RolUsuarios { get; set; } = default!;

public DbSet<SistemaVoto.Modelos.Usuario> Usuarios { get; set; } = default!;

public DbSet<SistemaVoto.Modelos.Certificado> Certificados { get; set; } = default!;

public DbSet<SistemaVoto.Modelos.ResultadoEleccion> ResultadosElecciones { get; set; } = default!;

public DbSet<SistemaVoto.Modelos.Voto> Votos { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- REGLA 1: La Cédula NO puede repetirse (Tu pedido) ---
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Cedula)
            .IsUnique();

        // --- REGLA 2: El Token de Votación también debería ser único ---
        // Para evitar que por un error cósmico se generen dos tokens iguales
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.TokenVotacion)
            .IsUnique();

        // --- REGLA 3: Configuración para Voto en Blanco (Opcional) ---
        modelBuilder.Entity<Voto>()
            .HasOne(v => v.ListaPolitica)
            .WithMany()
            .HasForeignKey(v => v.ListaPoliticaId)
            .IsRequired(false);
    }
}
