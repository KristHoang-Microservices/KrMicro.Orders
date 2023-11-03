﻿// <auto-generated />
using System;
using KrMicro.Orders.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KrMicro.Orders.Migrations
{
    [DbContext(typeof(OrderDbContext))]
    [Migration("20231103042411_Update order model")]
    partial class Updateordermodel
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.22")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("KrMicro.Orders.Models.DeliveryInformation", b =>
                {
                    b.Property<short?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasColumnName("Id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<short?>("Id"));

                    b.Property<DateTimeOffset?>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("CreatedAt");

                    b.Property<short?>("CustomerId")
                        .HasColumnType("smallint");

                    b.Property<string>("FullAddress")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("FullAddress");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Name");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Phone");

                    b.Property<int?>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("Status");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("UpdatedAt");

                    b.HasKey("Id");

                    b.ToTable("DeliveryInformation");
                });

            modelBuilder.Entity("KrMicro.Orders.Models.Order", b =>
                {
                    b.Property<short?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasColumnName("Id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<short?>("Id"));

                    b.Property<DateTimeOffset?>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("CreatedAt");

                    b.Property<short>("DeliveryInformationId")
                        .HasColumnType("smallint");

                    b.Property<DateTimeOffset?>("OrderDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("OrderDate");

                    b.Property<int>("OrderStatus")
                        .HasColumnType("integer")
                        .HasColumnName("OrderStatus");

                    b.Property<int?>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("Status");

                    b.Property<int>("TotalAmount")
                        .HasColumnType("integer")
                        .HasColumnName("TotalAmount");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("UpdatedAt");

                    b.HasKey("Id");

                    b.HasIndex("DeliveryInformationId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("KrMicro.Orders.Models.OrderDetail", b =>
                {
                    b.Property<short>("OrderId")
                        .HasColumnType("smallint")
                        .HasColumnName("OrderId");

                    b.Property<short>("ProductId")
                        .HasColumnType("smallint")
                        .HasColumnName("ProductId");

                    b.Property<DateTimeOffset?>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("CreatedAt");

                    b.Property<short?>("Id")
                        .HasColumnType("smallint")
                        .HasColumnName("Id");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric")
                        .HasColumnName("Price");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer")
                        .HasColumnName("Quantity");

                    b.Property<string>("SizeCode")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("SizeCode");

                    b.Property<int?>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("Status");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("UpdatedAt");

                    b.HasKey("OrderId", "ProductId");

                    b.ToTable("OrderDetail");
                });

            modelBuilder.Entity("KrMicro.Orders.Models.Payment", b =>
                {
                    b.Property<short?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasColumnName("Id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<short?>("Id"));

                    b.Property<DateTimeOffset?>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("CreatedAt");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Name");

                    b.Property<int?>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("Status");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("UpdatedAt");

                    b.HasKey("Id");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("KrMicro.Orders.Models.Transaction", b =>
                {
                    b.Property<short?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasColumnName("Id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<short?>("Id"));

                    b.Property<DateTimeOffset?>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("CreatedAt");

                    b.Property<short>("CustomerId")
                        .HasColumnType("smallint")
                        .HasColumnName("CustomerId");

                    b.Property<short>("OrderId")
                        .HasColumnType("smallint")
                        .HasColumnName("OrderId");

                    b.Property<short>("OrderId_Transaction")
                        .HasColumnType("smallint");

                    b.Property<short>("PaymentId")
                        .HasColumnType("smallint");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("PhoneNumber");

                    b.Property<int?>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("Status");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("UpdatedAt");

                    b.HasKey("Id");

                    b.HasIndex("OrderId_Transaction");

                    b.HasIndex("PaymentId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("KrMicro.Orders.Models.Order", b =>
                {
                    b.HasOne("KrMicro.Orders.Models.DeliveryInformation", "DeliveryInformation")
                        .WithMany("Orders")
                        .HasForeignKey("DeliveryInformationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DeliveryInformation");
                });

            modelBuilder.Entity("KrMicro.Orders.Models.OrderDetail", b =>
                {
                    b.HasOne("KrMicro.Orders.Models.Order", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("KrMicro.Orders.Models.Transaction", b =>
                {
                    b.HasOne("KrMicro.Orders.Models.Order", "Order")
                        .WithMany("Transactions")
                        .HasForeignKey("OrderId_Transaction")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KrMicro.Orders.Models.Payment", "Payment")
                        .WithMany("Transactions")
                        .HasForeignKey("PaymentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Payment");
                });

            modelBuilder.Entity("KrMicro.Orders.Models.DeliveryInformation", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("KrMicro.Orders.Models.Order", b =>
                {
                    b.Navigation("OrderDetails");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("KrMicro.Orders.Models.Payment", b =>
                {
                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
