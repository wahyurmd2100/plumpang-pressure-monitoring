﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PressMon.Web.Data;

#nullable disable

namespace TMS.Web.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.11");

            modelBuilder.Entity("TMS.Web.Models.Alarm", b =>
                {
                    b.Property<int>("AlarmID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AlarmStatus")
                        .HasColumnType("TEXT");

                    b.Property<string>("LocationName")
                        .HasColumnType("TEXT");

                    b.Property<double>("Pressure")
                        .HasColumnType("REAL");

                    b.Property<int>("TimeStamp")
                        .HasColumnType("INTEGER");

                    b.HasKey("AlarmID");

                    b.ToTable("Alarms");
                });

            modelBuilder.Entity("TMS.Web.Models.AlarmSettings", b =>
                {
                    b.Property<int>("AlarmSettingID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Info")
                        .HasColumnType("TEXT");

                    b.Property<int>("UpdateTimestamp")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Value")
                        .HasColumnType("REAL");

                    b.HasKey("AlarmSettingID");

                    b.ToTable("AlarmSettings");
                });

            modelBuilder.Entity("TMS.Web.Models.Historical", b =>
                {
                    b.Property<int>("HistoricalID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("LocationName")
                        .HasColumnType("TEXT");

                    b.Property<double>("Pressure")
                        .HasColumnType("REAL");

                    b.Property<int>("TimeStamp")
                        .HasColumnType("INTEGER");

                    b.HasKey("HistoricalID");

                    b.ToTable("Historicals");
                });

            modelBuilder.Entity("TMS.Web.Models.LiveData", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("LocationName")
                        .HasColumnType("TEXT");

                    b.Property<double>("Pressure")
                        .HasColumnType("REAL");

                    b.Property<int>("TimeStamp")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("LiveDatas");
                });
#pragma warning restore 612, 618
        }
    }
}
