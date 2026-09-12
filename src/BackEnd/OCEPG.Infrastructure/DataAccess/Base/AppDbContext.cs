using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OCPEG.Domain.DataAccess.Entities;
using System.Collections.Generic;

namespace OCEPG.Infrastructure.DataAccess.Base
{
    public class AppDbContext : DbContext
    {
        protected string? connectionString { get; }

        //public OcPegContext() : base()
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            var builder = new ConfigurationBuilder();

            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            builder.AddJsonFile(path, false);
            var configuration = builder.Build();

            connectionString = configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
        }



        public DbSet<Usuario> Usuario { get; set; }
        public virtual DbSet<Assunto> Assunto { get; set; } = null!;
        public virtual DbSet<Atendimento> Atendimento { get; set; } = null!;
        public virtual DbSet<Cliente> Cliente { get; set; } = null!;
        public virtual DbSet<Resolucao> Resolucao { get; set; } = null!;
        public virtual DbSet<TipoCliente> TipoCliente { get; set; } = null!;
        public virtual DbSet<ClienteImagem> ClienteImagem { get; set; } = null!;
        public virtual DbSet<Role> Role { get; set; } = null!;
        public virtual DbSet<UsuarioRole> UsuarioRole { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Diz para o Entity Framework => Você irá utilizar as configurações que está em um projeto.
            //E o meu projeto é o proprio infraestrutura que está aqui.
            //Está falando para ele onde que irá ficar as configurações.
            //Quais as configurações ?
            //As configurações que está no DbContext, no dbset que está dentro da propria classe.
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);


            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Assunto>(entity =>
            {
                entity.ToTable("Assunto");

                entity.Property(e => e.Descricao)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Atendimento>(entity =>
            {
                entity.HasKey(e => e.Protocolo);

                entity.ToTable("Atendimento");

                entity.HasIndex(e => e.IdAssunto, "IX_Atendimento_IdAssunto");

                entity.HasIndex(e => e.IdCliente, "IX_Atendimento_IdCliente");

                entity.HasIndex(e => e.UsuarioId, "IX_Atendimento_UsuarioId");

                entity.Property(e => e.Comentario).IsUnicode(false);

                entity.HasOne(d => d.IdAssuntoNavigation).WithMany(p => p.Atendimentos)
                    .HasForeignKey(d => d.IdAssunto)
                    .HasConstraintName("FK_Atendimento_Assunto");

                entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Atendimentos)
                    .HasForeignKey(d => d.IdCliente)
                    .HasConstraintName("FK_Atendimento_Cliente");

            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("Cliente");

                entity.HasIndex(e => e.TipoId, "IX_Cliente_TipoId");

                entity.Property(e => e.DataInscricao).HasColumnType("datetime");
                //entity.Property(e => e.DataInscricao).HasColumnType("TIMESTAMP(3)"); // Para Oracle
                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Mensalidade).HasColumnType("smallmoney");
                //entity.Property(e => e.Mensalidade).HasColumnType("number(10,4)");  // Para Oracle
                entity.Property(e => e.Nome)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Telefone)
                    .HasMaxLength(10)
                    .IsUnicode(false);
                entity.Property(e => e.Texto).IsUnicode(false);

                entity.HasOne(d => d.Tipo).WithMany(p => p.Clientes)
                    .HasForeignKey(d => d.TipoId)
                    .HasConstraintName("FK_Cliente_Tipo");
            });

            modelBuilder.Entity<ClienteImagem>(entity =>
            {
                entity.ToTable("ClienteImagem");

                entity.Property(e => e.Descricao)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ContentType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Dados).IsUnicode(false);
                entity.Property(e => e.DataUpload).HasColumnType("datetime2");
            });

            modelBuilder.Entity<Resolucao>(entity =>
            {
                entity.HasKey(e => e.Protocolo);

                entity.ToTable("Resolucao");

                entity.Property(e => e.Protocolo).ValueGeneratedNever();
                entity.Property(e => e.Comentario).IsUnicode(false);

                entity.HasOne(d => d.ProtocoloNavigation).WithOne(p => p.Resolucao)
                    .HasForeignKey<Resolucao>(d => d.Protocolo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Resolucao_Resolucao");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Nome).HasMaxLength(50);
                entity.Property(e => e.RoleId).HasMaxLength(50);
            });

            modelBuilder.Entity<TipoCliente>(entity =>
            {
                entity.ToTable("TipoCliente");

                entity.Property(e => e.Nome)
                    .HasMaxLength(40)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuario");

                entity.Property(e => e.Email)
                    .HasMaxLength(150)
                    .IsUnicode(false);
                entity.Property(e => e.Nome)
                    .HasMaxLength(150)
                    .IsUnicode(false);
                entity.Property(e => e.Telefone)
                    .HasMaxLength(20)
                    .IsUnicode(false);
                entity.Property(e => e.RefreshToken).IsUnicode(false);
                entity.Property(e => e.RefreshTokenExpiryTime).HasColumnType("datetime");
                //entity.Property(e => e.RefreshTokenExpiryTime).HasColumnType("TIMESTAMP(3)"); // Para Oracle
                entity.Property(e => e.PasswordHash).IsUnicode(false);
                entity.Property(e => e.Active).HasColumnType("bit");

                entity.Property(e => e.Administrador).HasColumnType("bit");

                entity.Property(e => e.Funcao).HasColumnType("smallint");

                entity.Property(e => e.UserIdentifier)
                    .HasColumnType("UNIQUEIDENTIFIER ROWGUIDCOL")
                    .IsRequired();

                entity.HasAlternateKey(e => e.UserIdentifier);
            });

            modelBuilder.Entity<UsuarioRole>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_UsuarioRole_1");

                entity.ToTable("UsuarioRole");
            });

        }

    }
}
