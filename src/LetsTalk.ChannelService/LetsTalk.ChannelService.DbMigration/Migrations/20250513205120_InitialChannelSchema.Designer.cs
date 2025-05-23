﻿// <auto-generated />
using System;
using LetsTalk.ChannelService.WebApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LetsTalk.ChannelService.DbMigration.Migrations
{
    [DbContext(typeof(ChannelDbContext))]
    [Migration("20250513205120_InitialChannelSchema")]
    partial class InitialChannelSchema
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("LetsTalk.ChannelService.WebApi.Channel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Channels");
                });

            modelBuilder.Entity("LetsTalk.ChannelService.WebApi.ChannelMember", b =>
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

                    b.HasKey("ChannelId", "UserId");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("LetsTalk.ChannelService.WebApi.ChannelMember", b =>
                {
                    b.HasOne("LetsTalk.ChannelService.WebApi.Channel", "Channel")
                        .WithMany("Members")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Channel");
                });

            modelBuilder.Entity("LetsTalk.ChannelService.WebApi.Channel", b =>
                {
                    b.Navigation("Members");
                });
#pragma warning restore 612, 618
        }
    }
}
