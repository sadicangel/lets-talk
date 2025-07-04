﻿// <auto-generated />
using System;
using LetsTalk.ChatService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LetsTalk.ChatService.DbMigration.Migrations
{
    [DbContext(typeof(ChatDbContext))]
    [Migration("20250625223457_InitialChatSchema")]
    partial class InitialChatSchema
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("LetsTalk.ChatService.Domain.Entities.Channel", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id");

                    b.ToTable("Channels");
                });

            modelBuilder.Entity("LetsTalk.ChatService.Domain.Entities.ChannelMember", b =>
                {
                    b.Property<string>("ChannelId")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("InvitedByUserId")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("LastSeenAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("MemberSince")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("ChannelId", "UserId");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("LetsTalk.ChatService.Domain.Entities.ChannelMessage", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<string>("AuthorId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ChannelId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<byte[]>("Content")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("LetsTalk.ChatService.Domain.Entities.ChannelMember", b =>
                {
                    b.HasOne("LetsTalk.ChatService.Domain.Entities.Channel", "Channel")
                        .WithMany("Members")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Channel");
                });

            modelBuilder.Entity("LetsTalk.ChatService.Domain.Entities.ChannelMessage", b =>
                {
                    b.HasOne("LetsTalk.ChatService.Domain.Entities.Channel", "Channel")
                        .WithMany("Messages")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Channel");
                });

            modelBuilder.Entity("LetsTalk.ChatService.Domain.Entities.Channel", b =>
                {
                    b.Navigation("Members");

                    b.Navigation("Messages");
                });
#pragma warning restore 612, 618
        }
    }
}
