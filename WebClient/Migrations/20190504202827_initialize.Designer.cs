﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebClient.Models;

namespace WebClient.Migrations
{
    [DbContext(typeof(LampContext))]
    [Migration("20190504202827_initialize")]
    partial class initialize
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity("WebClient.Models.LampSourcing", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Status")
                        .IsRequired();

                    b.Property<DateTime>("Time");

                    b.HasKey("Id");

                    b.ToTable("Lamp");
                });
#pragma warning restore 612, 618
        }
    }
}
