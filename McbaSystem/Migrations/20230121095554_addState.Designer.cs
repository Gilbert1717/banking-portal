﻿// <auto-generated />
using System;
using McbaSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace McbaSystem.Migrations
{
    [DbContext(typeof(McbaContext))]
    [Migration("20230121095554_addState")]
    partial class addState
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("McbaSystem.Models.Account", b =>
                {
                    b.Property<int>("AccountNumber")
                        .HasColumnType("int");

                    b.Property<int>("AccountType")
                        .HasColumnType("int");

                    b.Property<decimal>("Balance")
                        .HasColumnType("money");

                    b.Property<int>("CustomerID")
                        .HasColumnType("int");

                    b.HasKey("AccountNumber");

                    b.HasIndex("CustomerID");

                    b.ToTable("Accounts", t =>
                        {
                            t.HasCheckConstraint("CH_Account_Balance", "Balance >= 0");
                        });
                });

            modelBuilder.Entity("McbaSystem.Models.Customer", b =>
                {
                    b.Property<int>("CustomerID")
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("City")
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<string>("Mobile")
                        .HasMaxLength(12)
                        .HasColumnType("nvarchar(12)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PostCode")
                        .HasMaxLength(4)
                        .HasColumnType("nvarchar(4)");

                    b.Property<string>("State")
                        .HasMaxLength(3)
                        .HasColumnType("nvarchar(3)");

                    b.Property<string>("TFN")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("CustomerID");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("McbaSystem.Models.Login", b =>
                {
                    b.Property<string>("LoginID")
                        .HasMaxLength(8)
                        .HasColumnType("char");

                    b.Property<int>("CustomerID")
                        .HasColumnType("int");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(94)
                        .HasColumnType("char");

                    b.HasKey("LoginID");

                    b.HasIndex("CustomerID");

                    b.ToTable("Logins", t =>
                        {
                            t.HasCheckConstraint("CH_Login_LoginID", "len(LoginID) = 8");

                            t.HasCheckConstraint("CH_Login_PasswordHash", "len(PasswordHash) = 94");
                        });
                });

            modelBuilder.Entity("McbaSystem.Models.Transaction", b =>
                {
                    b.Property<int>("TransactionID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TransactionID"));

                    b.Property<int>("AccountNumber")
                        .HasColumnType("int");

                    b.Property<decimal>("Amount")
                        .HasColumnType("money");

                    b.Property<string>("Comment")
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<int?>("DestinationAccountNumber")
                        .HasColumnType("int");

                    b.Property<DateTime>("TransactionTimeUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("TransactionType")
                        .HasColumnType("int");

                    b.HasKey("TransactionID");

                    b.HasIndex("AccountNumber");

                    b.HasIndex("DestinationAccountNumber");

                    b.ToTable("Transactions", t =>
                        {
                            t.HasCheckConstraint("CH_Transaction_Amount", "Amount != 0");
                        });
                });

            modelBuilder.Entity("McbaSystem.Models.Account", b =>
                {
                    b.HasOne("McbaSystem.Models.Customer", "Customer")
                        .WithMany("Accounts")
                        .HasForeignKey("CustomerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("McbaSystem.Models.Login", b =>
                {
                    b.HasOne("McbaSystem.Models.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("McbaSystem.Models.Transaction", b =>
                {
                    b.HasOne("McbaSystem.Models.Account", "Account")
                        .WithMany("Transactions")
                        .HasForeignKey("AccountNumber")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("McbaSystem.Models.Account", "DestinationAccount")
                        .WithMany()
                        .HasForeignKey("DestinationAccountNumber");

                    b.Navigation("Account");

                    b.Navigation("DestinationAccount");
                });

            modelBuilder.Entity("McbaSystem.Models.Account", b =>
                {
                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("McbaSystem.Models.Customer", b =>
                {
                    b.Navigation("Accounts");
                });
#pragma warning restore 612, 618
        }
    }
}
