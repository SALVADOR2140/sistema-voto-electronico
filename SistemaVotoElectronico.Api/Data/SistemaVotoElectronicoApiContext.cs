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
    }
