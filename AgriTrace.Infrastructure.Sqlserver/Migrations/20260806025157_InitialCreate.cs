using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AgriTrace.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Symbol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Category = table.Column<int>(type: "int", nullable: false),
                    ConversionToBase = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organizations_OrganizationTypes_OrganizationTypeId",
                        column: x => x.OrganizationTypeId,
                        principalTable: "OrganizationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    MustChangePassword = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResetPasswordToken = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ResetPasswordTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Batches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentOrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    QRCode = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProductionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RemainingQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SourceQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ParentBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RootBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SplitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrganizationDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Batches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Batches_Batches_ParentBatchId",
                        column: x => x.ParentBatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Batches_Organizations_CurrentOrganizationId",
                        column: x => x.CurrentOrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Batches_Organizations_OrganizationDataModelId",
                        column: x => x.OrganizationDataModelId,
                        principalTable: "Organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Batches_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Batches_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BatchMerges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NewBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchMerges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatchMerges_Batches_NewBatchId",
                        column: x => x.NewBatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BatchSplits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchSplits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatchSplits_Batches_SourceBatchId",
                        column: x => x.SourceBatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EventTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    RejectionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventRequests_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventRequests_EventTypes_EventTypeId",
                        column: x => x.EventTypeId,
                        principalTable: "EventTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventRequests_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventRequests_Users_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventRequests_Users_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QualityInspections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InspectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InspectionType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    OverallResult = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    InspectionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualityInspections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QualityInspections_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QualityInspections_Users_InspectorId",
                        column: x => x.InspectorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Recalls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RecallDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recalls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recalls_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Recalls_Recalls_RecallDataModelId",
                        column: x => x.RecallDataModelId,
                        principalTable: "Recalls",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Recalls_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Recalls_Users_UserDataModelId",
                        column: x => x.UserDataModelId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SupplyChainEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerformedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InspectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PreviousHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CurrentHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EventTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UserDataModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplyChainEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplyChainEvents_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyChainEvents_EventTypes_EventTypeId",
                        column: x => x.EventTypeId,
                        principalTable: "EventTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyChainEvents_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyChainEvents_Users_PerformedByUserId",
                        column: x => x.PerformedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplyChainEvents_Users_UserDataModelId",
                        column: x => x.UserDataModelId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BatchMergeSources",
                columns: table => new
                {
                    BatchMergeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchMergeSources", x => new { x.BatchMergeId, x.SourceBatchId });
                    table.ForeignKey(
                        name: "FK_BatchMergeSources_BatchMerges_BatchMergeId",
                        column: x => x.BatchMergeId,
                        principalTable: "BatchMerges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BatchMergeSources_Batches_SourceBatchId",
                        column: x => x.SourceBatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BatchSplitDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SplitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchSplitDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatchSplitDetails_BatchSplits_SplitId",
                        column: x => x.SplitId,
                        principalTable: "BatchSplits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BatchSplitDetails_Batches_TargetBatchId",
                        column: x => x.TargetBatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InspectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CertificateType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FileUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IssuedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certificates_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Certificates_QualityInspections_InspectionId",
                        column: x => x.InspectionId,
                        principalTable: "QualityInspections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "InspectionLabTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InspectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MeasuredValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MinStandardValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MaxStandardValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsPassed = table.Column<bool>(type: "bit", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionLabTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionLabTests_QualityInspections_InspectionId",
                        column: x => x.InspectionId,
                        principalTable: "QualityInspections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Fresh vegetables and tubers", true, "Vegetables", null },
                    { new Guid("30000000-0000-0000-0000-000000000002"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Various types of coffee", true, "Coffee", null },
                    { new Guid("30000000-0000-0000-0000-000000000003"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Various types of rice", true, "Rice", null },
                    { new Guid("30000000-0000-0000-0000-000000000004"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Fresh fruits", true, "Fruits", null },
                    { new Guid("30000000-0000-0000-0000-000000000005"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Herbs and spices", true, "Herbs", null }
                });

            migrationBuilder.InsertData(
                table: "EventTypes",
                columns: new[] { "Id", "Code", "CreatedAt", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000000"), "CREATED", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Created", null },
                    { new Guid("20000000-0000-0000-0000-000000000001"), "HARVEST", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Harvest", null },
                    { new Guid("20000000-0000-0000-0000-000000000002"), "RECEIVE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Receive", null },
                    { new Guid("20000000-0000-0000-0000-000000000003"), "PROCESSING", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Processing", null },
                    { new Guid("20000000-0000-0000-0000-000000000004"), "PACKAGING", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Packaging", null },
                    { new Guid("20000000-0000-0000-0000-000000000005"), "TRANSPORT", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Transport", null },
                    { new Guid("20000000-0000-0000-0000-000000000006"), "DISTRIBUTION", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Distribution", null },
                    { new Guid("20000000-0000-0000-0000-000000000007"), "RETAIL", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Retail", null },
                    { new Guid("20000000-0000-0000-0000-000000000008"), "INSPECTION", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Inspection", null },
                    { new Guid("20000000-0000-0000-0000-000000000009"), "RECALL", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Recall", null },
                    { new Guid("20000000-0000-0000-0000-00000000000a"), "SPLIT", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Split", null },
                    { new Guid("20000000-0000-0000-0000-00000000000b"), "MERGE", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Merge", null }
                });

            migrationBuilder.InsertData(
                table: "OrganizationTypes",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "FARM", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Agricultural Farm", "Farm", null },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "PROCESSOR", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Processor", null },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "DISTRIBUTOR", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Distributor", null },
                    { new Guid("10000000-0000-0000-0000-000000000004"), "RETAILER", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Retailer", null },
                    { new Guid("10000000-0000-0000-0000-000000000005"), "INSPECTION", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Inspection", null },
                    { new Guid("10000000-0000-0000-0000-000000000006"), "SYSTEM", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "System", null }
                });

            migrationBuilder.InsertData(
                table: "Units",
                columns: new[] { "Id", "Category", "Code", "ConversionToBase", "CreatedAt", "Description", "Name", "Symbol", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000001"), 1, "KG", 1m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Metric unit of mass equal to 1000 grams", "Kilogram", "kg", null },
                    { new Guid("40000000-0000-0000-0000-000000000002"), 1, "GRAM", 0.001m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Metric unit of mass equal to 1/1000 kilogram", "Gram", "g", null },
                    { new Guid("40000000-0000-0000-0000-000000000003"), 2, "LITER", 1m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Metric unit of volume equal to 1000 cubic centimeters", "Liter", "L", null },
                    { new Guid("40000000-0000-0000-0000-000000000004"), 2, "MILLILITER", 0.001m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Metric unit of volume equal to 1/1000 liter", "Milliliter", "mL", null },
                    { new Guid("40000000-0000-0000-0000-000000000005"), 3, "BOX", 1m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Packaging unit containing a fixed number of items", "Box", "box", null },
                    { new Guid("40000000-0000-0000-0000-000000000006"), 3, "BALE", 1m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Compressed bundle of agricultural produce", "Bale", "bale", null },
                    { new Guid("40000000-0000-0000-0000-000000000007"), 3, "PIECE", 1m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Individual countable item", "Piece", "pc", null },
                    { new Guid("40000000-0000-0000-0000-000000000008"), 1, "TON", 1000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Metric unit of mass equal to 1000 kilograms", "Metric Ton", "t", null },
                    { new Guid("40000000-0000-0000-0000-000000000009"), 1, "SACK", 50m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Standard sack for bulk produce, typically 50 kg", "Sack", "sack", null }
                });

            migrationBuilder.InsertData(
                table: "Organizations",
                columns: new[] { "Id", "Address", "CreatedAt", "Name", "OrganizationTypeId", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("50000000-0000-0000-0000-000000000001"), "Tan Lac, Hoa Binh Province", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Green Farm Co.", new Guid("10000000-0000-0000-0000-000000000001"), 1, null },
                    { new Guid("50000000-0000-0000-0000-000000000002"), "Buon Ma Thuot, Dak Lak Province", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Golden Bean Processor", new Guid("10000000-0000-0000-0000-000000000002"), 1, null },
                    { new Guid("50000000-0000-0000-0000-000000000003"), "Binh Tan, Ho Chi Minh City", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fresh Link Distributor", new Guid("10000000-0000-0000-0000-000000000003"), 1, null },
                    { new Guid("50000000-0000-0000-0000-000000000004"), "Cau Giay, Hanoi City", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Agri Quality Inspection", new Guid("10000000-0000-0000-0000-000000000005"), 1, null },
                    { new Guid("50000000-0000-0000-0000-000000000005"), "District 1, Ho Chi Minh City", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fresh Market Retailer", new Guid("10000000-0000-0000-0000-000000000004"), 1, null },
                    { new Guid("50000000-0000-0000-0000-000000000006"), "Hanoi City", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System Operator", new Guid("10000000-0000-0000-0000-000000000006"), 1, null }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Name", "OrganizationId", "UnitId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("60000000-0000-0000-0000-000000000001"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Organic Tomato", new Guid("50000000-0000-0000-0000-000000000001"), new Guid("40000000-0000-0000-0000-000000000001"), null },
                    { new Guid("60000000-0000-0000-0000-000000000002"), new Guid("30000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dragon Fruit", new Guid("50000000-0000-0000-0000-000000000001"), new Guid("40000000-0000-0000-0000-000000000001"), null },
                    { new Guid("60000000-0000-0000-0000-000000000003"), new Guid("30000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Arabica Coffee", new Guid("50000000-0000-0000-0000-000000000002"), new Guid("40000000-0000-0000-0000-000000000005"), null },
                    { new Guid("60000000-0000-0000-0000-000000000004"), new Guid("30000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Jasmine Rice", new Guid("50000000-0000-0000-0000-000000000002"), new Guid("40000000-0000-0000-0000-000000000009"), null },
                    { new Guid("60000000-0000-0000-0000-000000000005"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Distributed Organic Cabbage", new Guid("50000000-0000-0000-0000-000000000003"), new Guid("40000000-0000-0000-0000-000000000001"), null },
                    { new Guid("60000000-0000-0000-0000-000000000006"), new Guid("30000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Retail Premium Mango", new Guid("50000000-0000-0000-0000-000000000005"), new Guid("40000000-0000-0000-0000-000000000001"), null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "OrganizationId", "PasswordHash", "Phone", "RefreshToken", "RefreshTokenExpiry", "ResetPasswordToken", "ResetPasswordTokenExpiry", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("70000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@agritrace.com", "System Administrator", true, new Guid("50000000-0000-0000-0000-000000000006"), "100000.WO50AmM77hFBSqiT1aSFiw==.e1i6MrL9ZZlQF4h2CiK5+qvkR7zilfDmRnLCHfUsNx8=", null, null, null, null, null, "Admin", null },
                    { new Guid("70000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff.a@greenfarm.com", "Nguyen Van A", true, new Guid("50000000-0000-0000-0000-000000000001"), "100000.a67yvmVEWhq7dIjEmejzIg==.8Q3q/IVS35pPn+kp951yFx+MHdVMm6EDdzXB4fqqEL0=", null, null, null, null, null, "Staff", null },
                    { new Guid("70000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "manager.b@goldenbean.com", "Tran Thi B", true, new Guid("50000000-0000-0000-0000-000000000002"), "100000.szsbqUNhABlx1s1a8koCTw==.bCSGZ6J7LaqRKz2Jqh55P0VHIdpQHe7+amEZl8Dk62I=", null, null, null, null, null, "Manager", null },
                    { new Guid("70000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff.c@agriquality.com", "Le Van C", true, new Guid("50000000-0000-0000-0000-000000000004"), "100000.8wke5U2qoW8dhTwYKYXlzQ==.iEDJyugFAUFuNzc5U+3bwcVXt1iNNU/FTZQAzrMwN8I=", null, null, null, null, null, "Staff", null },
                    { new Guid("70000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff.d@freshlink.com", "Pham Van D", true, new Guid("50000000-0000-0000-0000-000000000003"), "100000.kvsoLjDUX9yZUnO8qQ25bA==.WfdZNGvGSz5VLRa6KbjtFdMlu+Ac0wFDRY4OLYjZsxw=", null, null, null, null, null, "Staff", null },
                    { new Guid("70000000-0000-0000-0000-000000000006"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "manager.e@freshmarket.com", "Nguyen Thi E", true, new Guid("50000000-0000-0000-0000-000000000005"), "100000.GU+TnccSHk98rWkw0cQXEw==.ebiU7auk5qcdnKJlpraFuOV/h+ev7/Q9rUF9SjkQolk=", null, null, null, null, null, "Manager", null },
                    { new Guid("70000000-0000-0000-0000-000000000009"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "manager.a@greenfarm.com", "Tran Van A", true, new Guid("50000000-0000-0000-0000-000000000001"), "100000.a1msUgi5QIhbZpEXRt1RLw==.pBgsOLFf8fuOYD0kIrZE1QjF3Zw5mNqwItZaWY0Nu+A=", null, null, null, null, null, "Manager", null },
                    { new Guid("70000000-0000-0000-0000-00000000000a"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff.b@goldenbean.com", "Le Van B", true, new Guid("50000000-0000-0000-0000-000000000002"), "100000.jaBwHVU9d0AVr3qmA2kLWw==.IdJpm8+lGT/Z25/5KZl5eoOK9SQ+keuCylWiWKYMgKY=", null, null, null, null, null, "Staff", null },
                    { new Guid("70000000-0000-0000-0000-00000000000b"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "manager.d@freshlink.com", "Hoang Van E", true, new Guid("50000000-0000-0000-0000-000000000003"), "100000.44b24E3eLoX/Tn/6Ss3n/w==.7rXza6oxS2cigQoMHeKjYexG9a3ZT5FWdzQY1gApa/Q=", null, null, null, null, null, "Manager", null },
                    { new Guid("70000000-0000-0000-0000-00000000000c"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "manager.c@agriquality.com", "Pham Thi D", true, new Guid("50000000-0000-0000-0000-000000000004"), "100000.p1jNharWhbkY18w7UEmMNQ==.h5cbCKyGjyeFUhtrKcOOXPT0nNpAcAtrSZJY3GtOV3Y=", null, null, null, null, null, "Manager", null },
                    { new Guid("70000000-0000-0000-0000-00000000000d"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff.e@freshmarket.com", "Tran Thi F", true, new Guid("50000000-0000-0000-0000-000000000005"), "100000.po4yuFgTHgXEsj6fk4vYBQ==.0eVPEQ+X+wtF52FX7Y6T6LSlMzE7+eV46LYl6Yglff0=", null, null, null, null, null, "Staff", null }
                });

            migrationBuilder.InsertData(
                table: "Batches",
                columns: new[] { "Id", "BatchCode", "CreatedAt", "CurrentOrganizationId", "ExpiryDate", "OrganizationDataModelId", "ParentBatchId", "ProductId", "ProductionDate", "QRCode", "Quantity", "RemainingQuantity", "RootBatchId", "SourceQuantity", "SplitId", "Status", "UnitId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("80000000-0000-0000-0000-000000000001"), "TOMATO-20260105-001", new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("50000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new Guid("60000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, 500m, 500m, new Guid("80000000-0000-0000-0000-000000000001"), 500m, null, 2, new Guid("40000000-0000-0000-0000-000000000001"), null },
                    { new Guid("80000000-0000-0000-0000-000000000002"), "DRAGONFRUIT-20260108-001", new DateTime(2026, 1, 8, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("50000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new Guid("60000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, 300m, 300m, new Guid("80000000-0000-0000-0000-000000000002"), 300m, null, 7, new Guid("40000000-0000-0000-0000-000000000001"), null },
                    { new Guid("80000000-0000-0000-0000-000000000003"), "COFFEE-20260110-001", new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("50000000-0000-0000-0000-000000000002"), new DateTime(2027, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new Guid("60000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, 200m, 150m, new Guid("80000000-0000-0000-0000-000000000003"), 200m, null, 4, new Guid("40000000-0000-0000-0000-000000000005"), null },
                    { new Guid("80000000-0000-0000-0000-000000000004"), "RICE-20260112-001", new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("50000000-0000-0000-0000-000000000002"), new DateTime(2027, 1, 12, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new Guid("60000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Utc), null, 20m, 20m, new Guid("80000000-0000-0000-0000-000000000004"), 20m, null, 7, new Guid("40000000-0000-0000-0000-000000000009"), null },
                    { new Guid("80000000-0000-0000-0000-000000000005"), "DIST-20260115-001", new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("50000000-0000-0000-0000-000000000003"), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new Guid("60000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), null, 100m, 100m, new Guid("80000000-0000-0000-0000-000000000005"), 100m, null, 5, new Guid("40000000-0000-0000-0000-000000000001"), null },
                    { new Guid("80000000-0000-0000-0000-000000000006"), "RETAIL-20260118-001", new DateTime(2026, 1, 18, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("50000000-0000-0000-0000-000000000005"), new DateTime(2027, 1, 18, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new Guid("60000000-0000-0000-0000-000000000006"), new DateTime(2026, 1, 18, 0, 0, 0, 0, DateTimeKind.Utc), null, 50m, 50m, new Guid("80000000-0000-0000-0000-000000000006"), 50m, null, 6, new Guid("40000000-0000-0000-0000-000000000005"), null }
                });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "Message", "Title", "UpdatedAt", "UserId" },
                values: new object[] { new Guid("a0000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 15, 8, 30, 0, 0, DateTimeKind.Utc), "Batch Dragon Fruit (DRAGONFRUIT-20260108-001) has been recalled due to pesticide residue exceeding the permitted threshold.", "Batch Recall Alert", null, new Guid("70000000-0000-0000-0000-000000000001") });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "IsRead", "Message", "Title", "UpdatedAt", "UserId" },
                values: new object[] { new Guid("a0000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc), true, "The AgriTrace system has been successfully initialized with initial seed data.", "System Initialized", new DateTime(2026, 1, 1, 9, 15, 0, 0, DateTimeKind.Utc), new Guid("70000000-0000-0000-0000-000000000001") });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "Message", "Title", "UpdatedAt", "UserId" },
                values: new object[] { new Guid("a0000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 5, 14, 0, 0, 0, DateTimeKind.Utc), "Batch Organic Tomato (TOMATO-20260105-001) has transitioned to Harvested status.", "Batch Status Updated", null, new Guid("70000000-0000-0000-0000-000000000002") });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "IsRead", "Message", "Title", "UpdatedAt", "UserId" },
                values: new object[] { new Guid("a0000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), true, "Batch Dragon Fruit (DRAGONFRUIT-20260108-001) supplied by you has been recalled. Please check the details.", "Your Batch Has Been Recalled", new DateTime(2026, 1, 16, 10, 0, 0, 0, DateTimeKind.Utc), new Guid("70000000-0000-0000-0000-000000000002") });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "Message", "Title", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { new Guid("a0000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 16, 11, 0, 0, 0, DateTimeKind.Utc), "Batch Jasmine Rice (RICE-20260112-001) received a complaint about foreign objects found inside the packaging from a customer.", "Product Quality Complaint", null, new Guid("70000000-0000-0000-0000-000000000003") },
                    { new Guid("a0000000-0000-0000-0000-000000000006"), new DateTime(2026, 1, 10, 16, 0, 0, 0, DateTimeKind.Utc), "Batch Arabica Coffee (COFFEE-20260110-001) is currently in Transporting status with 150 kg remaining.", "Batch In Transit", null, new Guid("70000000-0000-0000-0000-000000000003") }
                });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "IsRead", "Message", "Title", "UpdatedAt", "UserId" },
                values: new object[] { new Guid("a0000000-0000-0000-0000-000000000007"), new DateTime(2026, 1, 18, 8, 0, 0, 0, DateTimeKind.Utc), true, "Batch Dragon Fruit (DRAGONFRUIT-20260108-001) requires additional inspection following the previous recall related to a labeling defect.", "Additional Inspection Required", new DateTime(2026, 1, 18, 8, 20, 0, 0, DateTimeKind.Utc), new Guid("70000000-0000-0000-0000-000000000004") });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "Message", "Title", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { new Guid("a0000000-0000-0000-0000-000000000008"), new DateTime(2026, 1, 20, 7, 30, 0, 0, DateTimeKind.Utc), "There are 2 batches awaiting quality inspection this week.", "New Inspection Schedule", null, new Guid("70000000-0000-0000-0000-000000000004") },
                    { new Guid("d0000000-0000-0000-0000-000000000001"), new DateTime(2026, 6, 1, 8, 5, 0, 0, DateTimeKind.Utc), "Your harvest event for batch BATCH-TOMATO-001 has been successfully recorded.", "Harvest Event Recorded", null, new Guid("70000000-0000-0000-0000-000000000002") }
                });

            migrationBuilder.InsertData(
                table: "QualityInspections",
                columns: new[] { "Id", "BatchId", "CreatedAt", "InspectionDate", "InspectionType", "InspectorId", "Notes", "OverallResult", "Status", "UpdatedAt" },
                values: new object[] { new Guid("a0000000-0000-0000-0000-000000000001"), new Guid("80000000-0000-0000-0000-000000000001"), new DateTime(2026, 6, 2, 9, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new Guid("70000000-0000-0000-0000-000000000004"), "Excellent quality. All standards met.", "PASS", 2, null });

            migrationBuilder.InsertData(
                table: "Recalls",
                columns: new[] { "Id", "BatchId", "CreatedAt", "CreatedBy", "Reason", "RecallDataModelId", "Severity", "Status", "UpdatedAt", "UserDataModelId" },
                values: new object[,]
                {
                    { new Guid("90000000-0000-0000-0000-000000000001"), new Guid("80000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("70000000-0000-0000-0000-000000000001"), "Pesticide residue detected exceeding the permitted threshold.", null, 3, 2, null, null },
                    { new Guid("90000000-0000-0000-0000-000000000002"), new Guid("80000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("70000000-0000-0000-0000-000000000003"), "Customer reported foreign objects found inside the packaging.", null, 4, 2, null, null },
                    { new Guid("90000000-0000-0000-0000-000000000003"), new Guid("80000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 18, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("70000000-0000-0000-0000-000000000001"), "Follow-up inspection after previous recall; minor labeling defect detected.", null, 1, 3, null, null }
                });

            migrationBuilder.InsertData(
                table: "SupplyChainEvents",
                columns: new[] { "Id", "BatchId", "CreatedAt", "CurrentHash", "EventData", "EventTime", "EventTypeId", "InspectionId", "Location", "OrganizationId", "PerformedByUserId", "PreviousHash", "UpdatedAt", "UserDataModelId" },
                values: new object[,]
                {
                    { new Guid("90000000-0000-0000-0000-000000000001"), new Guid("80000000-0000-0000-0000-000000000001"), new DateTime(2026, 6, 1, 8, 0, 0, 0, DateTimeKind.Utc), null, "Harvested 1000kg of tomatoes", new DateTime(2026, 6, 1, 8, 0, 0, 0, DateTimeKind.Utc), new Guid("20000000-0000-0000-0000-000000000001"), null, "Green Farm Field 1", new Guid("50000000-0000-0000-0000-000000000001"), new Guid("70000000-0000-0000-0000-000000000002"), null, null, null },
                    { new Guid("90000000-0000-0000-0000-000000000002"), new Guid("80000000-0000-0000-0000-000000000002"), new DateTime(2026, 5, 2, 10, 0, 0, 0, DateTimeKind.Utc), null, "Processed and roasted coffee beans", new DateTime(2026, 5, 2, 10, 0, 0, 0, DateTimeKind.Utc), new Guid("20000000-0000-0000-0000-000000000003"), null, "Golden Bean Factory", new Guid("50000000-0000-0000-0000-000000000002"), new Guid("70000000-0000-0000-0000-000000000003"), null, null, null }
                });

            migrationBuilder.InsertData(
                table: "Certificates",
                columns: new[] { "Id", "BatchId", "CertificateType", "CreatedAt", "FileUrl", "InspectionId", "IssuedDate", "UpdatedAt" },
                values: new object[] { new Guid("b0000000-0000-0000-0000-000000000001"), new Guid("80000000-0000-0000-0000-000000000001"), "Organic Certification", new DateTime(2026, 6, 2, 10, 0, 0, 0, DateTimeKind.Utc), "https://agritrace.com/certs/cert-001.pdf", new Guid("a0000000-0000-0000-0000-000000000001"), new DateTime(2026, 6, 2, 10, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.CreateIndex(
                name: "IX_Batches_BatchCode",
                table: "Batches",
                column: "BatchCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Batches_CurrentOrganizationId",
                table: "Batches",
                column: "CurrentOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_OrganizationDataModelId",
                table: "Batches",
                column: "OrganizationDataModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_ParentBatchId",
                table: "Batches",
                column: "ParentBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_ProductId",
                table: "Batches",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_QRCode",
                table: "Batches",
                column: "QRCode");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_RootBatchId",
                table: "Batches",
                column: "RootBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_SplitId",
                table: "Batches",
                column: "SplitId");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_UnitId",
                table: "Batches",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchMerges_NewBatchId",
                table: "BatchMerges",
                column: "NewBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchMergeSources_SourceBatchId",
                table: "BatchMergeSources",
                column: "SourceBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchSplitDetails_SplitId",
                table: "BatchSplitDetails",
                column: "SplitId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchSplitDetails_TargetBatchId",
                table: "BatchSplitDetails",
                column: "TargetBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchSplits_SourceBatchId",
                table: "BatchSplits",
                column: "SourceBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_BatchId",
                table: "Certificates",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_InspectionId",
                table: "Certificates",
                column: "InspectionId");

            migrationBuilder.CreateIndex(
                name: "IX_EventRequests_BatchId",
                table: "EventRequests",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_EventRequests_EventTypeId",
                table: "EventRequests",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EventRequests_OrganizationId",
                table: "EventRequests",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_EventRequests_RequestedByUserId",
                table: "EventRequests",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventRequests_ReviewedByUserId",
                table: "EventRequests",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTypes_Code",
                table: "EventTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InspectionLabTests_InspectionId",
                table: "InspectionLabTests",
                column: "InspectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_OrganizationTypeId",
                table: "Organizations",
                column: "OrganizationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTypes_Code",
                table: "OrganizationTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_OrganizationId",
                table: "Products",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UnitId",
                table: "Products",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityInspections_BatchId",
                table: "QualityInspections",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityInspections_InspectorId",
                table: "QualityInspections",
                column: "InspectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Recalls_BatchId",
                table: "Recalls",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Recalls_CreatedBy",
                table: "Recalls",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Recalls_RecallDataModelId",
                table: "Recalls",
                column: "RecallDataModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Recalls_UserDataModelId",
                table: "Recalls",
                column: "UserDataModelId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyChainEvents_BatchId",
                table: "SupplyChainEvents",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyChainEvents_EventTypeId",
                table: "SupplyChainEvents",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyChainEvents_OrganizationId",
                table: "SupplyChainEvents",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyChainEvents_PerformedByUserId",
                table: "SupplyChainEvents",
                column: "PerformedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyChainEvents_UserDataModelId",
                table: "SupplyChainEvents",
                column: "UserDataModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Units_Code",
                table: "Units",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_OrganizationId",
                table: "Users",
                column: "OrganizationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BatchMergeSources");

            migrationBuilder.DropTable(
                name: "BatchSplitDetails");

            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.DropTable(
                name: "EventRequests");

            migrationBuilder.DropTable(
                name: "InspectionLabTests");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Recalls");

            migrationBuilder.DropTable(
                name: "SupplyChainEvents");

            migrationBuilder.DropTable(
                name: "BatchMerges");

            migrationBuilder.DropTable(
                name: "BatchSplits");

            migrationBuilder.DropTable(
                name: "QualityInspections");

            migrationBuilder.DropTable(
                name: "EventTypes");

            migrationBuilder.DropTable(
                name: "Batches");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "OrganizationTypes");
        }
    }
}
