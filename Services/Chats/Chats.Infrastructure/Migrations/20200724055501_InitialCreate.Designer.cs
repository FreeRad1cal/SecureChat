﻿// <auto-generated />
using System;
using Chats.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Chats.Infrastructure.Migrations
{
    [DbContext(typeof(ChatsContext))]
    [Migration("20200724055501_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Chats.Domain.AggregateModel.Chat", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Chats");
                });

            modelBuilder.Entity("Chats.Domain.AggregateModel.ChatMembership", b =>
                {
                    b.Property<string>("ChatId")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTimeOffset>("ModifiedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("ChatId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("ChatMemberships");
                });

            modelBuilder.Entity("Chats.Domain.AggregateModel.ChatModerator", b =>
                {
                    b.Property<string>("ChatId")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.HasKey("ChatId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("ChatModerators");
                });

            modelBuilder.Entity("Chats.Domain.AggregateModel.Message", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("ChatId")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTimeOffset>("ModifiedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.HasIndex("UserId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("Chats.Domain.AggregateModel.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Chats.Domain.AggregateModel.Chat", b =>
                {
                    b.HasOne("Chats.Domain.AggregateModel.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Chats.Domain.AggregateModel.ChatMembership", b =>
                {
                    b.HasOne("Chats.Domain.AggregateModel.Chat", "Chat")
                        .WithMany("ChatMemberships")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Chats.Domain.AggregateModel.User", "User")
                        .WithMany("ChatMemberships")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Chats.Domain.AggregateModel.ChatModerator", b =>
                {
                    b.HasOne("Chats.Domain.AggregateModel.Chat", "Chat")
                        .WithMany("ChatModerators")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Chats.Domain.AggregateModel.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Chats.Domain.AggregateModel.Message", b =>
                {
                    b.HasOne("Chats.Domain.AggregateModel.Chat", "Chat")
                        .WithMany("Messages")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Chats.Domain.AggregateModel.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
