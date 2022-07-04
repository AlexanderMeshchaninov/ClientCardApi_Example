﻿// <auto-generated />
using System;
using Lessons.ClientCardApi.Data.Context.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Migrations.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20220225150818_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Lessons.ClientCardApi.Abstraction.Data.Models.CreditCardInfoModel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("CreditCardNumber")
                        .HasColumnType("bigint");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<long>("PassportNumber")
                        .HasColumnType("bigint");

                    b.Property<string>("Patronymic")
                        .HasColumnType("text");

                    b.Property<long>("PhoneNumber")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("CreditCardNumber")
                        .IsUnique();

                    b.ToTable("CreditCardInfo");
                });
#pragma warning restore 612, 618
        }
    }
}