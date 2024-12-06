using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PruebaFleet.Models;

namespace PruebaFleet;

public partial class DdddbContext : DbContext
{
    public DdddbContext()
    {
    }

    public DdddbContext(DbContextOptions<DdddbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Compra> Compras { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=rox.database.windows.net;Database=DDDDB;User Id=adminX;Password=Hola123456;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(e => e.CompId).HasName("pk_compra");

            entity.ToTable("compras");

            entity.Property(e => e.CompId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("compId");
            entity.Property(e => e.CompCategoria)
                .HasMaxLength(250)
                .HasDefaultValue("")
                .HasColumnName("comp_categoria");
            entity.Property(e => e.CompDCreate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("comp_dCreate");
            entity.Property(e => e.CompProducto)
                .HasMaxLength(250)
                .HasDefaultValue("")
                .HasColumnName("comp_producto");
            entity.Property(e => e.CompStatus)
                .HasDefaultValue((byte)1)
                .HasColumnName("comp_status");
            entity.Property(e => e.CompTipoGarantia)
                .HasMaxLength(250)
                .HasDefaultValue("")
                .HasColumnName("comp_tipoGarantia");
            entity.Property(e => e.CompUsuarioGuid)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("comp_usuarioGuid");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("pk_usuarios");

            entity.ToTable("usuarios");

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.UserAge).HasColumnName("user_age");
            entity.Property(e => e.UserDcreate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("user_dcreate");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(300)
                .HasDefaultValue("")
                .HasColumnName("user_email");
            entity.Property(e => e.UserGenderId).HasColumnName("user_genderId");
            entity.Property(e => e.UserGuid)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("userGuid");
            entity.Property(e => e.UserNombre)
                .HasMaxLength(250)
                .HasDefaultValue("")
                .HasColumnName("user_nombre");
            entity.Property(e => e.UserPApellido)
                .HasMaxLength(250)
                .HasDefaultValue("")
                .HasColumnName("user_pApellido");
            entity.Property(e => e.UserPass)
                .HasDefaultValue("")
                .HasColumnName("user_pass");
            entity.Property(e => e.UserSApellido)
                .HasMaxLength(250)
                .HasDefaultValue("")
                .HasColumnName("user_sApellido");
            entity.Property(e => e.UserStatus).HasColumnName("user_status");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
