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

        // 1. FORZAR NOMBRES DE TABLAS (Para que coincidan con pgAdmin)
        modelBuilder.Entity<Usuario>().ToTable("Usuarios"); // Asegura el uso de "Usuarios"
        modelBuilder.Entity<RolUsuario>().ToTable("RolUsuarios"); // Asegura el uso de "RolUsuarios" con 's'

        // 2. CONFIGURAR RELACIÓN USUARIO -> ROL
        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.RolUsuario)
            .WithMany()
            .HasForeignKey(u => u.RolUsuarioId); // Vincula la FK con la propiedad de navegación

        // 3. ÍNDICES DE USUARIO
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Cedula)
            .IsUnique();

        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.TokenVotacion)
            .IsUnique();

        // 4. CONFIGURACIÓN DE VOTOS
        modelBuilder.Entity<Voto>()
            .HasOne(v => v.ListaPolitica)
            .WithMany()
            .HasForeignKey(v => v.ListaPoliticaId)
            .IsRequired(false);

        // 5. CONFIGURACIÓN DE VOTANTES
        modelBuilder.Entity<Votante>()
            .HasIndex(v => v.Cedula)
            .IsUnique();

        modelBuilder.Entity<Votante>()
            .HasIndex(v => v.Token)
            .IsUnique();
    }
}
