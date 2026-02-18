using Microsoft.EntityFrameworkCore;
using SistemaVoto.Modelos;
using SistemaVotoElectronico.Modelos.Votacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

public DbSet<Votante> Votantes { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Cedula)
            .IsUnique();

        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.TokenVotacion)
            .IsUnique();

        modelBuilder.Entity<Voto>()
            .HasOne(v => v.ListaPolitica)
            .WithMany()
            .HasForeignKey(v => v.ListaPoliticaId)
            .IsRequired(false);

        // Esto evita que subas dos veces la misma cédula por error
        modelBuilder.Entity<Votante>()
            .HasIndex(v => v.Cedula)
            .IsUnique();

        // Esto asegura que nunca se repita un Token generado
        modelBuilder.Entity<Votante>()
            .HasIndex(v => v.Token)
            .IsUnique();
    }
}
