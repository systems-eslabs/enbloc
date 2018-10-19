using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace enbloc.DbEntities
{
    public partial class empezarContext : DbContext
    {
        public empezarContext()
        {
        }

        public empezarContext(DbContextOptions<empezarContext> options)
            : base(options)
        {
        }

        public virtual DbSet<EmptyEnbloc> EmptyEnbloc { get; set; }
        public virtual DbSet<EmptyEnblocArchive> EmptyEnblocArchive { get; set; }
        public virtual DbSet<EmptyEnblocContainers> EmptyEnblocContainers { get; set; }
        public virtual DbSet<EmptyEnblocContainersHistory> EmptyEnblocContainersHistory { get; set; }
        public virtual DbSet<EmptyEnblocContainersHistoryArchive> EmptyEnblocContainersHistoryArchive { get; set; }
        public virtual DbSet<EmptyEnblocHistory> EmptyEnblocHistory { get; set; }
        public virtual DbSet<EmptyEnblocHistoryArchive> EmptyEnblocHistoryArchive { get; set; }
        public virtual DbSet<EmptyEnblocSnapshot> EmptyEnblocSnapshot { get; set; }
        public virtual DbSet<EmptyEnblocSnapshotArchive> EmptyEnblocSnapshotArchive { get; set; }
        public virtual DbSet<MasterStatus> MasterStatus { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql(@"server=35.200.194.132;port=3306;user=root;
password=pop@123456;database=empezar");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmptyEnbloc>(entity =>
            {
                entity.ToTable("empty_enbloc");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.AgentName)
                    .HasColumnName("agent_name")
                    .HasColumnType("varchar(300)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.DepotName)
                    .HasColumnName("depot_name")
                    .HasColumnType("varchar(150)");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnName("modified_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modified_date")
                    .HasColumnType("timestamp");

                entity.Property(e => e.PermissionDate)
                    .HasColumnName("permission_date")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.TransactionId)
                    .HasColumnName("transaction_id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Vessel)
                    .IsRequired()
                    .HasColumnName("vessel")
                    .HasColumnType("varchar(200)");

                entity.Property(e => e.VesselNo)
                    .IsRequired()
                    .HasColumnName("vessel_no")
                    .HasColumnType("varchar(250)");

                entity.Property(e => e.ViaNo)
                    .HasColumnName("via_no")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.Voyage)
                    .IsRequired()
                    .HasColumnName("voyage")
                    .HasColumnType("varchar(10)");
            });

            modelBuilder.Entity<EmptyEnblocArchive>(entity =>
            {
                entity.ToTable("empty_enbloc_archive");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.AgentName)
                    .HasColumnName("agent_name")
                    .HasColumnType("varchar(300)");

                entity.Property(e => e.ArchivedBy)
                    .HasColumnName("archived_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ArchivedDate)
                    .HasColumnName("archived_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp");

                entity.Property(e => e.DepotName)
                    .HasColumnName("depot_name")
                    .HasColumnType("varchar(150)");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnName("modified_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modified_date")
                    .HasColumnType("timestamp");

                entity.Property(e => e.PermissionDate)
                    .HasColumnName("permission_date")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.TransactionId)
                    .HasColumnName("transaction_id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Vessel)
                    .IsRequired()
                    .HasColumnName("vessel")
                    .HasColumnType("varchar(200)");

                entity.Property(e => e.VesselNo)
                    .IsRequired()
                    .HasColumnName("vessel_no")
                    .HasColumnType("varchar(250)");

                entity.Property(e => e.ViaNo)
                    .HasColumnName("via_no")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.Voyage)
                    .IsRequired()
                    .HasColumnName("voyage")
                    .HasColumnType("varchar(10)");
            });

            modelBuilder.Entity<EmptyEnblocContainers>(entity =>
            {
                entity.ToTable("empty_enbloc_containers");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.BlNumber)
                    .HasColumnName("bl_number")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Cargo)
                    .HasColumnName("cargo")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.CargoDescription)
                    .HasColumnName("cargo_description")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ContainerDimension)
                    .IsRequired()
                    .HasColumnName("container_dimension")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.ContainerGrossDetails)
                    .HasColumnName("container_gross_details")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ContainerNo)
                    .IsRequired()
                    .HasColumnName("container_no")
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ContainerSize)
                    .HasColumnName("container_size")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ContainerType)
                    .IsRequired()
                    .HasColumnName("container_type")
                    .HasColumnType("varchar(8)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.DisposalMode)
                    .HasColumnName("disposal_mode")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ImdgClass)
                    .HasColumnName("imdg_class")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.IsoCode)
                    .HasColumnName("iso_code")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ItemNo)
                    .HasColumnName("item_no")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnName("modified_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modified_date")
                    .HasColumnType("timestamp");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.OogDeatils)
                    .HasColumnName("oog_deatils")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ReferTemrature)
                    .HasColumnName("refer_temrature")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.SealNo1)
                    .HasColumnName("seal_no_1")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.SealNo2)
                    .HasColumnName("seal_no_2")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.SealNo3)
                    .HasColumnName("seal_no_3")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Srl)
                    .HasColumnName("srl")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.TransactionId)
                    .HasColumnName("transaction_id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Vessel)
                    .IsRequired()
                    .HasColumnName("vessel")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.VesselNo)
                    .IsRequired()
                    .HasColumnName("vessel_no")
                    .HasColumnType("varchar(150)");

                entity.Property(e => e.Voyage)
                    .IsRequired()
                    .HasColumnName("voyage")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Wt)
                    .HasColumnName("wt")
                    .HasColumnType("varchar(500)");
            });

            modelBuilder.Entity<EmptyEnblocContainersHistory>(entity =>
            {
                entity.ToTable("empty_enbloc_containers_history");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.BlNumber)
                    .HasColumnName("bl_number")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Cargo)
                    .HasColumnName("cargo")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.CargoDescription)
                    .HasColumnName("cargo_description")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ContainerDimension)
                    .IsRequired()
                    .HasColumnName("container_dimension")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.ContainerGrossDetails)
                    .HasColumnName("container_gross_details")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ContainerNo)
                    .IsRequired()
                    .HasColumnName("container_no")
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ContainerSize)
                    .HasColumnName("container_size")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ContainerType)
                    .IsRequired()
                    .HasColumnName("container_type")
                    .HasColumnType("varchar(8)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp");

                entity.Property(e => e.DisposalMode)
                    .HasColumnName("disposal_mode")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.HistoryBy)
                    .HasColumnName("history_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.HistoryDate)
                    .HasColumnName("history_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ImdgClass)
                    .HasColumnName("imdg_class")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.IsoCode)
                    .HasColumnName("iso_code")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ItemNo)
                    .HasColumnName("item_no")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnName("modified_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modified_date")
                    .HasColumnType("timestamp");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.OogDeatils)
                    .HasColumnName("oog_deatils")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ReferTemrature)
                    .HasColumnName("refer_temrature")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.SealNo1)
                    .HasColumnName("seal_no_1")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.SealNo2)
                    .HasColumnName("seal_no_2")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.SealNo3)
                    .HasColumnName("seal_no_3")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Srl)
                    .HasColumnName("srl")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.TransactionId)
                    .HasColumnName("transaction_id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Vessel)
                    .IsRequired()
                    .HasColumnName("vessel")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.VesselNo)
                    .IsRequired()
                    .HasColumnName("vessel_no")
                    .HasColumnType("varchar(150)");

                entity.Property(e => e.Voyage)
                    .IsRequired()
                    .HasColumnName("voyage")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Wt)
                    .HasColumnName("wt")
                    .HasColumnType("varchar(500)");
            });

            modelBuilder.Entity<EmptyEnblocContainersHistoryArchive>(entity =>
            {
                entity.ToTable("empty_enbloc_containers_history_archive");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.ArchivedBy)
                    .HasColumnName("archived_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ArchivedDate)
                    .HasColumnName("archived_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.BlNumber)
                    .HasColumnName("bl_number")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Cargo)
                    .HasColumnName("cargo")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.CargoDescription)
                    .HasColumnName("cargo_description")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ContainerDimension)
                    .IsRequired()
                    .HasColumnName("container_dimension")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.ContainerGrossDetails)
                    .HasColumnName("container_gross_details")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ContainerNo)
                    .IsRequired()
                    .HasColumnName("container_no")
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.ContainerSize)
                    .HasColumnName("container_size")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ContainerType)
                    .IsRequired()
                    .HasColumnName("container_type")
                    .HasColumnType("varchar(8)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp");

                entity.Property(e => e.DisposalMode)
                    .HasColumnName("disposal_mode")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.HistoryBy)
                    .HasColumnName("history_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.HistoryDate)
                    .HasColumnName("history_date")
                    .HasColumnType("timestamp");

                entity.Property(e => e.ImdgClass)
                    .HasColumnName("imdg_class")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.IsoCode)
                    .HasColumnName("iso_code")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ItemNo)
                    .HasColumnName("item_no")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnName("modified_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modified_date")
                    .HasColumnType("timestamp");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.OogDeatils)
                    .HasColumnName("oog_deatils")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ReferTemrature)
                    .HasColumnName("refer_temrature")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.SealNo1)
                    .HasColumnName("seal_no_1")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.SealNo2)
                    .HasColumnName("seal_no_2")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.SealNo3)
                    .HasColumnName("seal_no_3")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Srl)
                    .HasColumnName("srl")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.TransactionId)
                    .HasColumnName("transaction_id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Vessel)
                    .IsRequired()
                    .HasColumnName("vessel")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.VesselNo)
                    .IsRequired()
                    .HasColumnName("vessel_no")
                    .HasColumnType("varchar(150)");

                entity.Property(e => e.Voyage)
                    .IsRequired()
                    .HasColumnName("voyage")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Wt)
                    .HasColumnName("wt")
                    .HasColumnType("varchar(500)");
            });

            modelBuilder.Entity<EmptyEnblocHistory>(entity =>
            {
                entity.ToTable("empty_enbloc_history");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.AgentName)
                    .HasColumnName("agent_name")
                    .HasColumnType("varchar(200)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.HistoryBy)
                    .HasColumnName("history_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.HistoryDate)
                    .HasColumnName("history_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ModifiedBy)
                    .HasColumnName("modified_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modified_date")
                    .HasColumnType("timestamp");

                entity.Property(e => e.TransactionId)
                    .HasColumnName("transaction_id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Vessel)
                    .IsRequired()
                    .HasColumnName("vessel")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.VesselNo)
                    .IsRequired()
                    .HasColumnName("vessel_no")
                    .HasColumnType("varchar(150)");

                entity.Property(e => e.ViaNo)
                    .HasColumnName("via_no")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Voyage)
                    .IsRequired()
                    .HasColumnName("voyage")
                    .HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<EmptyEnblocHistoryArchive>(entity =>
            {
                entity.ToTable("empty_enbloc_history_archive");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.AgentName)
                    .HasColumnName("agent_name")
                    .HasColumnType("varchar(200)");

                entity.Property(e => e.ArchivedBy)
                    .HasColumnName("archived_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ArchivedDate)
                    .HasColumnName("archived_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.HistoryBy)
                    .HasColumnName("history_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.HistoryDate)
                    .HasColumnName("history_date")
                    .HasColumnType("timestamp");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnName("modified_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modified_date")
                    .HasColumnType("timestamp");

                entity.Property(e => e.TransactionId)
                    .HasColumnName("transaction_id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Vessel)
                    .IsRequired()
                    .HasColumnName("vessel")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.VesselNo)
                    .IsRequired()
                    .HasColumnName("vessel_no")
                    .HasColumnType("varchar(150)");

                entity.Property(e => e.ViaNo)
                    .HasColumnName("via_no")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Voyage)
                    .IsRequired()
                    .HasColumnName("voyage")
                    .HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<EmptyEnblocSnapshot>(entity =>
            {
                entity.ToTable("empty_enbloc_snapshot");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.AgentName)
                    .HasColumnName("agent_name")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.BlNumber)
                    .HasColumnName("bl_number")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Cargo)
                    .HasColumnName("cargo")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.CargoDescription)
                    .HasColumnName("cargo_description")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ContainerGrossDetails)
                    .HasColumnName("container_gross_details")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ContainerNo)
                    .HasColumnName("container_no")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ContainerType)
                    .HasColumnName("container_type")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.DisposalMode)
                    .HasColumnName("disposal_mode")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ImdgClass)
                    .HasColumnName("imdg_class")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.IsoCode)
                    .HasColumnName("iso_code")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ItemNo)
                    .HasColumnName("item_no")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.OogDeatils)
                    .HasColumnName("oog_deatils")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ReferTemrature)
                    .HasColumnName("refer_temrature")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.SealNo1)
                    .HasColumnName("seal_no_1")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.SealNo2)
                    .HasColumnName("seal_no_2")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.SealNo3)
                    .HasColumnName("seal_no_3")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Srl)
                    .HasColumnName("srl")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.TransactionId)
                    .HasColumnName("transaction_id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Vessel)
                    .HasColumnName("vessel")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ViaNo)
                    .HasColumnName("via_no")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Voyage)
                    .HasColumnName("voyage")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Wt)
                    .HasColumnName("wt")
                    .HasColumnType("varchar(500)");
            });

            modelBuilder.Entity<EmptyEnblocSnapshotArchive>(entity =>
            {
                entity.ToTable("empty_enbloc_snapshot_archive");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.AgentName)
                    .HasColumnName("agent_name")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ArchivedBy)
                    .HasColumnName("archived_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ArchivedDate)
                    .HasColumnName("archived_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.BlNumber)
                    .HasColumnName("bl_number")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Cargo)
                    .HasColumnName("cargo")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.CargoDescription)
                    .HasColumnName("cargo_description")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ContainerGrossDetails)
                    .HasColumnName("container_gross_details")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ContainerNo)
                    .HasColumnName("container_no")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ContainerType)
                    .HasColumnName("container_type")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.DisposalMode)
                    .HasColumnName("disposal_mode")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ImdgClass)
                    .HasColumnName("imdg_class")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.IsoCode)
                    .HasColumnName("iso_code")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ItemNo)
                    .HasColumnName("item_no")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.OogDeatils)
                    .HasColumnName("oog_deatils")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ReferTemrature)
                    .HasColumnName("refer_temrature")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.SealNo1)
                    .HasColumnName("seal_no_1")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.SealNo2)
                    .HasColumnName("seal_no_2")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.SealNo3)
                    .HasColumnName("seal_no_3")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Srl)
                    .HasColumnName("srl")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.TransactionId)
                    .HasColumnName("transaction_id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Vessel)
                    .HasColumnName("vessel")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ViaNo)
                    .HasColumnName("via_no")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Voyage)
                    .HasColumnName("voyage")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Wt)
                    .HasColumnName("wt")
                    .HasColumnType("varchar(500)");
            });

            modelBuilder.Entity<MasterStatus>(entity =>
            {
                entity.ToTable("master_status");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ArchivedDate)
                    .HasColumnName("archived_date")
                    .HasColumnType("timestamp");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("code")
                    .HasColumnType("varchar(5)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.IsActive)
                    .HasColumnName("is_active")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnName("modified_by")
                    .HasColumnType("timestamp");

                entity.Property(e => e.Scope)
                    .IsRequired()
                    .HasColumnName("scope")
                    .HasColumnType("varchar(30)");
            });
        }
    }
}
