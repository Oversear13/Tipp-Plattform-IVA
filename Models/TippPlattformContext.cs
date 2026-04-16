
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TippPlattform.Models;

public partial class TippPlattformContext : DbContext
{
    public TippPlattformContext()
    {
    }

    public TippPlattformContext(DbContextOptions<TippPlattformContext> options)
        : base(options)
    {
    }


    public virtual DbSet<Beitritt> Beitritte { get; set; }

    public virtual DbSet<Liga> Ligen { get; set; }

    public virtual DbSet<Mannschaft> Mannschaften { get; set; }

    public virtual DbSet<PunkteRegel> PunkteRegeln { get; set; }

    public virtual DbSet<Spiele> Spiele { get; set; }

    public virtual DbSet<SpieleInTippgruppe> SpieleInTippgruppen { get; set; }

    public virtual DbSet<Sporttype> Sporttypen { get; set; }

    public virtual DbSet<TippgruppeAdmin> TippgruppeAdmins { get; set; }

    public virtual DbSet<Tippgruppe> Tippgruppen { get; set; }

    public virtual DbSet<Tippschein> Tippscheine { get; set; }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Nachricht> Nachrichten { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=ubi30.informatik.uni-siegen.de;Database=Group2;User Id=Group2;Password=HSjTFdRSyQCvTOE2;TrustServerCertificate=True;Encrypt=False;");

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<Beitritt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("beitritte_pkey");

            entity.ToTable("beitritte");

            entity.Property(e => e.Id)
            .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.JoinedAt)
                .HasColumnType("datetime")
                .HasColumnName("joined_at");
            entity.Property(e => e.TippgruppeId).HasColumnName("tippgruppe_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Points).HasColumnName("points");

            entity.HasOne(d => d.Tippgruppe).WithMany(p => p.Beitrittes)
                .HasForeignKey(d => d.TippgruppeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("beitritte_tippgruppe_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Beitrittes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("beitritte_user_id_fkey");
        });

        modelBuilder.Entity<Liga>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("liga_pkey");

            entity.ToTable("liga");

            entity.Property(e => e.Id)
            .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.ApiLigaId)
                .HasColumnName("api_liga_id");
            entity.Property(e => e.LigaName)
                .HasColumnType("character varying(255)")
                .HasColumnName("liga_name");
        });

        modelBuilder.Entity<Mannschaft>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mannschaft_pkey");

            entity.ToTable("mannschaft");

            entity.Property(e => e.Id)
            .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.APIMannschaftID)
                .HasColumnName("api_mannschaft_id");
            entity.Property(e => e.LigaId).HasColumnName("liga_id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying(255)")
                .HasColumnName("name");
            entity.Property(e => e.Rang).HasColumnName("rang");

            entity.HasOne(d => d.Liga).WithMany(p => p.Mannschafts)
                .HasForeignKey(d => d.LigaId)
                .HasConstraintName("mannschaft_liga_id_fkey")
                .HasPrincipalKey(p => p.ApiLigaId);
        });

        modelBuilder.Entity<Spiele>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("spiele_pkey");

            entity.ToTable("spiele");

            entity.Property(e => e.Id)
            .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.APISpielID)
                .HasColumnName("api_spiel_id");
            entity.Property(e => e.LigaId).HasColumnName("liga_id");
            entity.Property(e => e.SpielBeginn)
                .HasColumnType("datetime")
                .HasColumnName("spiel_beginn");
            entity.Property(e => e.SpielEnde)
                .HasColumnType("datetime")
                .HasColumnName("spiel_ende");
            // Rename and change column type to integer
            entity.Property(e => e.TeamAId)
                .HasColumnName("teamAId")
                .HasColumnType("integer");
            entity.Property(e => e.TeamAScore).HasColumnName("teama_score");
            // Rename and change column type to integer
            entity.Property(e => e.TeamBId)
                .HasColumnName("teamBId")
                .HasColumnType("integer");
            entity.Property(e => e.TeamBScore).HasColumnName("teamb_score");
            // Define the relationship with Liga
            entity.HasOne(d => d.Liga) // Spiel has one Liga
                .WithMany(p => p.Spieles) // Liga has many Spiele
                .HasForeignKey(d => d.LigaId)
                .HasConstraintName("spiele_liga_id_fkey")
                .HasPrincipalKey(p => p.ApiLigaId);
            // Define the relationship with Mannschaft (TeamA)
            entity.HasOne(d => d.TeamA)
                .WithMany(p => p.HomeMatches)
                .HasForeignKey(d => d.TeamAId)
                 .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("spiele_teamAId_fkey");
            // Define the relationship with Mannschaft (TeamB)
            entity.HasOne(d => d.TeamB)
                .WithMany(p => p.AwayMatches)
                .HasForeignKey(d => d.TeamBId)
                 .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("spiele_teamBId_fkey");
        });

        modelBuilder.Entity<SpieleInTippgruppe>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("spiele_in_tippgruppe_pkey");

            entity.ToTable("spiele_in_tippgruppe");

            entity.Property(e => e.Id)
            .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.SpielId).HasColumnName("spiel_id");
            entity.Property(e => e.TippgruppeId).HasColumnName("tippgruppe_id");
            entity.Property(e => e.PunkteRegelId).HasColumnName("punkteregel_id");

            entity.HasOne(d => d.Spiel).WithMany(p => p.SpieleInTippgruppes)
                .HasForeignKey(d => d.SpielId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("spiele_in_tippgruppe_spiel_id_fkey");

            entity.HasOne(d => d.Tippgruppe).WithMany(p => p.SpieleInTippgruppes)
                .HasForeignKey(d => d.TippgruppeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("spiele_in_tippgruppe_tippgruppe_id_fkey");

            entity.HasOne(d => d.PunkteRegel)
                .WithMany(p => p.SpieleInTippgruppe)
                .HasForeignKey(d => d.PunkteRegelId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("spiele_in_tippgruppe_punkteregel_id_fkey");
        });

        modelBuilder.Entity<Sporttype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sporttypes_pkey");

            entity.ToTable("sporttypes");

            entity.Property(e => e.Id)
            .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying(255)")
                .HasColumnName("name");
        });

        modelBuilder.Entity<TippgruppeAdmin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tippgruppe_admin_pkey");

            entity.ToTable("tippgruppe_admin");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.TippgruppeId).HasColumnName("tippgruppe_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Tippgruppe).WithMany(p => p.TippgruppeAdmins)
                .HasForeignKey(d => d.TippgruppeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("tippgruppe_admin_tippgruppe_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.TippgruppeAdmins)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("tippgruppe_admin_user_id_fkey");
        });

        modelBuilder.Entity<Tippgruppe>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tippgruppen_pkey");

            entity.ToTable("tippgruppen");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasColumnType("character varying(255)")
                .HasColumnName("name");
            entity.Property(e => e.Beschreibung)
                .HasColumnType("character varying(255)")
                .HasColumnName("beschreibung");
            entity.Property(e => e.Passwort)
                .HasColumnType("character varying(255)")
                .HasColumnName("passwort");
            entity.Property(e => e.SporttypeId).HasColumnName("sporttype_id");

            entity.HasOne(d => d.Sporttype).WithMany(p => p.Tippgruppens)
                .HasForeignKey(d => d.SporttypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tippgruppen_sporttype_id_fkey");

        });

        modelBuilder.Entity<Tippschein>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tippscheine_pkey");

            entity.ToTable("tippscheine");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.SpielId).HasColumnName("spiel_id");
            entity.Property(e => e.TippgruppeId).HasColumnName("tippgruppe_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Quote1).HasColumnName("quote1");
            entity.Property(e => e.Quote2).HasColumnName("quote2");
            entity.Property(e => e.Quote3).HasColumnName("quote3");
            entity.Property(e => e.Quote4).HasColumnName("quote4");
            entity.Property(e => e.TippA).HasColumnName("tipp_a");
            entity.Property(e => e.TippB).HasColumnName("tipp_b");

            entity.HasOne(d => d.Spiel).WithMany(p => p.Tippscheines)
                .HasForeignKey(d => d.SpielId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("tippscheine_spiel_id_fkey");

            entity.HasOne(d => d.Tippgruppe).WithMany(p => p.Tippscheines)
                .HasForeignKey(d => d.TippgruppeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("tippscheine_tippgruppe_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Tippscheines)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("tippscheine_user_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasColumnType("character varying(255)")
                .HasColumnName("email");
            entity.Property(e => e.Geburtstag)
                .HasColumnType("datetime")
                .HasColumnName("geburtstag");
            entity.Property(e => e.Password)
                .HasColumnType("character varying(255)")
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasColumnType("character varying(255)")
                .HasColumnName("role");
            entity.Property(e => e.Username)
                .HasColumnType("character varying(255)")
                .HasColumnName("username");

        });

        modelBuilder.Entity<PunkteRegel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_PunkteRegel");

            entity.ToTable("PunkteRegel");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            entity.Property(e => e.Name)
                .HasColumnType("character varying(255)")
                .HasColumnName("Name");

            entity.Property(e => e.Beschreibung)
                .HasColumnType("character varying(1000)")
                .HasColumnName("Beschreibung");

            entity.Property(e => e.Quote1)
                .HasColumnName("Quote1");

            entity.Property(e => e.Quote2)
                .HasColumnName("Quote2");

            entity.Property(e => e.Quote3)
                .HasColumnName("Quote3");

            entity.Property(e => e.Quote4)
                .HasColumnName("Quote4");

            entity.Property(e => e.Tippgruppe_Id)
                .HasColumnName("Tippgruppe_Id");

            entity.HasOne(e => e.Tippgruppe)
                .WithMany(t => t.PunkteRegeln)
                .HasForeignKey(e => e.Tippgruppe_Id)
                .HasConstraintName("tippgruppe_punkteregeln_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
