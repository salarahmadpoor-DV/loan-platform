using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matchi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MatchiBaselineAlignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessProviders_Businesses_BusinessId",
                schema: "dbo",
                table: "BusinessProviders");

            migrationBuilder.DropForeignKey(
                name: "FK_BusinessProviders_Providers_ProviderId",
                schema: "dbo",
                table: "BusinessProviders");

            migrationBuilder.DropForeignKey(
                name: "FK_ProviderServices_Providers_ProviderId",
                schema: "dbo",
                table: "ProviderServices");

            migrationBuilder.DropForeignKey(
                name: "FK_ProviderServices_Services_ServiceId",
                schema: "dbo",
                table: "ProviderServices");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Introductions_IntroductionId",
                schema: "dbo",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_ServiceCategories_CategoryId",
                schema: "dbo",
                table: "Services");

            migrationBuilder.DropTable(
                name: "BankQuestion",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Introductions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LoanRequest",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RequestAnswers",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Banks",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "QuestionOptions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ServiceRequests",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ServiceQuestions",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_Users_Mobile",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_ProviderServices_ProviderId",
                schema: "dbo",
                table: "ProviderServices");

            migrationBuilder.DropColumn(
                name: "TargetType",
                schema: "dbo",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "dbo",
                table: "Providers");

            migrationBuilder.RenameColumn(
                name: "TargetId",
                schema: "dbo",
                table: "Reviews",
                newName: "DealId");

            migrationBuilder.RenameColumn(
                name: "IntroductionId",
                schema: "dbo",
                table: "Reviews",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_IntroductionId",
                schema: "dbo",
                table: "Reviews",
                newName: "IX_Reviews_CustomerId");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                schema: "dbo",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Services",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                schema: "dbo",
                table: "Services",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "dbo",
                table: "Services",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                schema: "dbo",
                table: "Services",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "dbo",
                table: "Services",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                schema: "dbo",
                table: "Services",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "ServiceCategories",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                schema: "dbo",
                table: "ServiceCategories",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                schema: "dbo",
                table: "ServiceCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "dbo",
                table: "ServiceCategories",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AlterColumn<byte>(
                name: "Rating",
                schema: "dbo",
                table: "Reviews",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Reviews",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                schema: "dbo",
                table: "Reviews",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                schema: "dbo",
                table: "Reviews",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BusinessId",
                schema: "dbo",
                table: "Reviews",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProviderId",
                schema: "dbo",
                table: "Reviews",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "dbo",
                table: "ProviderServices",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "dbo",
                table: "ProviderServices",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "ProviderServices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                schema: "dbo",
                table: "ProviderServices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Rating",
                schema: "dbo",
                table: "Providers",
                type: "decimal(3,2)",
                precision: 3,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(double),
                oldType: "float",
                oldDefaultValue: 0.0);

            migrationBuilder.AlterColumn<decimal>(
                name: "Lng",
                schema: "dbo",
                table: "Providers",
                type: "decimal(9,6)",
                precision: 9,
                scale: 6,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Lat",
                schema: "dbo",
                table: "Providers",
                type: "decimal(9,6)",
                precision: 9,
                scale: 6,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Providers",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                schema: "dbo",
                table: "Providers",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "CompletedJobCount",
                schema: "dbo",
                table: "Providers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "dbo",
                table: "Providers",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReviewCount",
                schema: "dbo",
                table: "Providers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "dbo",
                table: "Providers",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Active");

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                schema: "dbo",
                table: "Providers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "dbo",
                table: "BusinessProviders",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Active",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                schema: "dbo",
                table: "BusinessProviders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "JoinedAt",
                schema: "dbo",
                table: "BusinessProviders",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "sysutcdatetime()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                schema: "dbo",
                table: "BusinessProviders",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "BusinessProviders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                schema: "dbo",
                table: "BusinessProviders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Lng",
                schema: "dbo",
                table: "Businesses",
                type: "decimal(9,6)",
                precision: 9,
                scale: 6,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Lat",
                schema: "dbo",
                table: "Businesses",
                type: "decimal(9,6)",
                precision: 9,
                scale: 6,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Businesses",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                schema: "dbo",
                table: "Businesses",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                schema: "dbo",
                table: "Businesses",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                schema: "dbo",
                table: "Businesses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompletedJobCount",
                schema: "dbo",
                table: "Businesses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "dbo",
                table: "Businesses",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "District",
                schema: "dbo",
                table: "Businesses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LogoMediaId",
                schema: "dbo",
                table: "Businesses",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                schema: "dbo",
                table: "Businesses",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OwnerUserId",
                schema: "dbo",
                table: "Businesses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Province",
                schema: "dbo",
                table: "Businesses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Rating",
                schema: "dbo",
                table: "Businesses",
                type: "decimal(3,2)",
                precision: 3,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ReviewCount",
                schema: "dbo",
                table: "Businesses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "dbo",
                table: "Businesses",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Active");

            migrationBuilder.CreateTable(
                name: "BusinessAvailabilities",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessId = table.Column<long>(type: "bigint", nullable: false),
                    DayOfWeek = table.Column<byte>(type: "tinyint", nullable: false),
                    TimeFrom = table.Column<TimeSpan>(type: "time", nullable: false),
                    TimeTo = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessAvailabilities", x => x.Id);
                    table.CheckConstraint("CK_BusinessAvailabilities_DayOfWeek", "[DayOfWeek]>=(0) AND [DayOfWeek]<=(6)");
                    table.CheckConstraint("CK_BusinessAvailabilities_Time", "[TimeFrom]<[TimeTo]");
                    table.ForeignKey(
                        name: "FK_BusinessAvailabilities_Businesses",
                        column: x => x.BusinessId,
                        principalSchema: "dbo",
                        principalTable: "Businesses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BusinessPortfolios",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "sysutcdatetime()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessPortfolios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessPortfolios_Businesses",
                        column: x => x.BusinessId,
                        principalSchema: "dbo",
                        principalTable: "Businesses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BusinessServiceAreas",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessId = table.Column<long>(type: "bigint", nullable: false),
                    AreaType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Province = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    District = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Lat = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    Lng = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    Radius = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessServiceAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessServiceAreas_Businesses",
                        column: x => x.BusinessId,
                        principalSchema: "dbo",
                        principalTable: "Businesses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BusinessServices",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CanCustomerChooseProvider = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    MinPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessServices", x => x.Id);
                    table.CheckConstraint("CK_BusinessServices_PriceRange", "[MinPrice] IS NULL OR [MaxPrice] IS NULL OR [MinPrice]<=[MaxPrice]");
                    table.ForeignKey(
                        name: "FK_BusinessServices_Businesses",
                        column: x => x.BusinessId,
                        principalSchema: "dbo",
                        principalTable: "Businesses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BusinessServices_Services",
                        column: x => x.ServiceId,
                        principalSchema: "dbo",
                        principalTable: "Services",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_Users",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Media",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StorageKey = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "sysutcdatetime()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.Id);
                    table.CheckConstraint("CK_Media_Size", "[Size]>=(0)");
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductCategories_Parent",
                        column: x => x.ParentId,
                        principalSchema: "dbo",
                        principalTable: "ProductCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProviderAvailabilities",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderId = table.Column<long>(type: "bigint", nullable: false),
                    DayOfWeek = table.Column<byte>(type: "tinyint", nullable: false),
                    TimeFrom = table.Column<TimeSpan>(type: "time", nullable: false),
                    TimeTo = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderAvailabilities", x => x.Id);
                    table.CheckConstraint("CK_ProviderAvailabilities_DayOfWeek", "[DayOfWeek]>=(0) AND [DayOfWeek]<=(6)");
                    table.CheckConstraint("CK_ProviderAvailabilities_Time", "[TimeFrom]<[TimeTo]");
                    table.ForeignKey(
                        name: "FK_ProviderAvailabilities_Providers",
                        column: x => x.ProviderId,
                        principalSchema: "dbo",
                        principalTable: "Providers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProviderPortfolios",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "sysutcdatetime()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderPortfolios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderPortfolios_Providers",
                        column: x => x.ProviderId,
                        principalSchema: "dbo",
                        principalTable: "Providers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProviderServiceAreas",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderId = table.Column<long>(type: "bigint", nullable: false),
                    AreaType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Province = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    District = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Lat = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    Lng = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    Radius = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderServiceAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderServiceAreas_Providers",
                        column: x => x.ProviderId,
                        principalSchema: "dbo",
                        principalTable: "Providers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceAttributes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceAttributes_Services",
                        column: x => x.ServiceId,
                        principalSchema: "dbo",
                        principalTable: "Services",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TrustScores",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EntityId = table.Column<long>(type: "bigint", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CalculatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "sysutcdatetime()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrustScores", x => x.Id);
                    table.CheckConstraint("CK_TrustScores_Score", "[Score]>=(0) AND [Score]<=(100)");
                });

            migrationBuilder.CreateTable(
                name: "Verifications",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EntityId = table.Column<long>(type: "bigint", nullable: false),
                    VerificationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpireAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Verifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    RequestType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Open"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                    table.CheckConstraint("CK_Requests_RequestType", "[RequestType]=N'Hybrid' OR [RequestType]=N'Service' OR [RequestType]=N'Product'");
                    table.ForeignKey(
                        name: "FK_Requests_Customers",
                        column: x => x.CustomerId,
                        principalSchema: "dbo",
                        principalTable: "Customers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BusinessPortfolioMedia",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PortfolioId = table.Column<long>(type: "bigint", nullable: false),
                    MediaId = table.Column<long>(type: "bigint", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessPortfolioMedia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessPortfolioMedia_Media",
                        column: x => x.MediaId,
                        principalSchema: "dbo",
                        principalTable: "Media",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BusinessPortfolioMedia_Portfolios",
                        column: x => x.PortfolioId,
                        principalSchema: "dbo",
                        principalTable: "BusinessPortfolios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductAttributes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductCategoryId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAttributes_ProductCategories",
                        column: x => x.ProductCategoryId,
                        principalSchema: "dbo",
                        principalTable: "ProductCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Brand = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SKU = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_ProductCategories",
                        column: x => x.CategoryId,
                        principalSchema: "dbo",
                        principalTable: "ProductCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProviderPortfolioMedia",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PortfolioId = table.Column<long>(type: "bigint", nullable: false),
                    MediaId = table.Column<long>(type: "bigint", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderPortfolioMedia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderPortfolioMedia_Media",
                        column: x => x.MediaId,
                        principalSchema: "dbo",
                        principalTable: "Media",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProviderPortfolioMedia_Portfolios",
                        column: x => x.PortfolioId,
                        principalSchema: "dbo",
                        principalTable: "ProviderPortfolios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                schema: "dbo",
                columns: table => new
                {
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    PermissionId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "sysutcdatetime()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions",
                        column: x => x.PermissionId,
                        principalSchema: "dbo",
                        principalTable: "Permissions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles",
                        column: x => x.RoleId,
                        principalSchema: "dbo",
                        principalTable: "Roles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "dbo",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "sysutcdatetime()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles",
                        column: x => x.RoleId,
                        principalSchema: "dbo",
                        principalTable: "Roles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserRoles_Users",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProviderCapabilities",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceAttributeId = table.Column<long>(type: "bigint", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderCapabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderCapabilities_Providers",
                        column: x => x.ProviderId,
                        principalSchema: "dbo",
                        principalTable: "Providers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProviderCapabilities_ServiceAttributes",
                        column: x => x.ServiceAttributeId,
                        principalSchema: "dbo",
                        principalTable: "ServiceAttributes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ServiceAttributeOptions",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceAttributeId = table.Column<long>(type: "bigint", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceAttributeOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceAttributeOptions_ServiceAttributes",
                        column: x => x.ServiceAttributeId,
                        principalSchema: "dbo",
                        principalTable: "ServiceAttributes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VerificationDocuments",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VerificationId = table.Column<long>(type: "bigint", nullable: false),
                    MediaId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "sysutcdatetime()"),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VerificationDocuments_Media",
                        column: x => x.MediaId,
                        principalSchema: "dbo",
                        principalTable: "Media",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VerificationDocuments_Verifications",
                        column: x => x.VerificationId,
                        principalSchema: "dbo",
                        principalTable: "Verifications",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Conversations",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<long>(type: "bigint", nullable: false),
                    BusinessId = table.Column<long>(type: "bigint", nullable: true),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "sysutcdatetime()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conversations_Businesses",
                        column: x => x.BusinessId,
                        principalSchema: "dbo",
                        principalTable: "Businesses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Conversations_Customers",
                        column: x => x.CustomerId,
                        principalSchema: "dbo",
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Conversations_Requests",
                        column: x => x.RequestId,
                        principalSchema: "dbo",
                        principalTable: "Requests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Proposals",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<long>(type: "bigint", nullable: false),
                    BusinessId = table.Column<long>(type: "bigint", nullable: true),
                    ProviderId = table.Column<long>(type: "bigint", nullable: true),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DeliveryFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ProposedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ProposedTimeFrom = table.Column<TimeSpan>(type: "time", nullable: true),
                    ProposedTimeTo = table.Column<TimeSpan>(type: "time", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Pending"),
                    ExpireAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proposals", x => x.Id);
                    table.CheckConstraint("CK_Proposals_Party", "[BusinessId] IS NOT NULL AND [ProviderId] IS NULL OR [BusinessId] IS NULL AND [ProviderId] IS NOT NULL");
                    table.CheckConstraint("CK_Proposals_Prices", "[TotalPrice]>=(0) AND [DeliveryFee]>=(0)");
                    table.CheckConstraint("CK_Proposals_Time", "[ProposedTimeFrom] IS NULL OR [ProposedTimeTo] IS NULL OR [ProposedTimeFrom]<[ProposedTimeTo]");
                    table.ForeignKey(
                        name: "FK_Proposals_Businesses",
                        column: x => x.BusinessId,
                        principalSchema: "dbo",
                        principalTable: "Businesses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Proposals_Providers",
                        column: x => x.ProviderId,
                        principalSchema: "dbo",
                        principalTable: "Providers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Proposals_Requests",
                        column: x => x.RequestId,
                        principalSchema: "dbo",
                        principalTable: "Requests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RequestLocations",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<long>(type: "bigint", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Province = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    District = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Lat = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    Lng = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestLocations_Requests",
                        column: x => x.RequestId,
                        principalSchema: "dbo",
                        principalTable: "Requests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RequestSchedules",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    TimeFrom = table.Column<TimeSpan>(type: "time", nullable: true),
                    TimeTo = table.Column<TimeSpan>(type: "time", nullable: true),
                    IsFlexible = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestSchedules", x => x.Id);
                    table.CheckConstraint("CK_RequestSchedules_Time", "[TimeFrom] IS NULL OR [TimeTo] IS NULL OR [TimeFrom]<[TimeTo]");
                    table.ForeignKey(
                        name: "FK_RequestSchedules_Requests",
                        column: x => x.RequestId,
                        principalSchema: "dbo",
                        principalTable: "Requests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RequestServices",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false, defaultValue: 1m),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestServices", x => x.Id);
                    table.CheckConstraint("CK_RequestServices_Quantity", "[Quantity]>(0)");
                    table.ForeignKey(
                        name: "FK_RequestServices_Requests",
                        column: x => x.RequestId,
                        principalSchema: "dbo",
                        principalTable: "Requests",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RequestServices_Services",
                        column: x => x.ServiceId,
                        principalSchema: "dbo",
                        principalTable: "Services",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductAttributeOptions",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductAttributeId = table.Column<long>(type: "bigint", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributeOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAttributeOptions_ProductAttributes",
                        column: x => x.ProductAttributeId,
                        principalSchema: "dbo",
                        principalTable: "ProductAttributes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BusinessProducts",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MinOrderQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    LeadTimeDays = table.Column<int>(type: "int", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessProducts", x => x.Id);
                    table.CheckConstraint("CK_BusinessProducts_LeadTimeDays", "[LeadTimeDays] IS NULL OR [LeadTimeDays]>=(0)");
                    table.CheckConstraint("CK_BusinessProducts_MinOrderQuantity", "[MinOrderQuantity] IS NULL OR [MinOrderQuantity]>(0)");
                    table.CheckConstraint("CK_BusinessProducts_Price", "[Price] IS NULL OR [Price]>=(0)");
                    table.ForeignKey(
                        name: "FK_BusinessProducts_Businesses",
                        column: x => x.BusinessId,
                        principalSchema: "dbo",
                        principalTable: "Businesses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BusinessProducts_Products",
                        column: x => x.ProductId,
                        principalSchema: "dbo",
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductAttributeValues",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    ProductAttributeId = table.Column<long>(type: "bigint", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributeValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAttributeValues_ProductAttributes",
                        column: x => x.ProductAttributeId,
                        principalSchema: "dbo",
                        principalTable: "ProductAttributes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductAttributeValues_Products",
                        column: x => x.ProductId,
                        principalSchema: "dbo",
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProviderProducts",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MinOrderQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: true),
                    LeadTimeDays = table.Column<int>(type: "int", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderProducts", x => x.Id);
                    table.CheckConstraint("CK_ProviderProducts_LeadTimeDays", "[LeadTimeDays] IS NULL OR [LeadTimeDays]>=(0)");
                    table.CheckConstraint("CK_ProviderProducts_MinOrderQuantity", "[MinOrderQuantity] IS NULL OR [MinOrderQuantity]>(0)");
                    table.CheckConstraint("CK_ProviderProducts_Price", "[Price] IS NULL OR [Price]>=(0)");
                    table.ForeignKey(
                        name: "FK_ProviderProducts_Products",
                        column: x => x.ProductId,
                        principalSchema: "dbo",
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProviderProducts_Providers",
                        column: x => x.ProviderId,
                        principalSchema: "dbo",
                        principalTable: "Providers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RequestProducts",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: true),
                    ProductCategoryId = table.Column<long>(type: "bigint", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false, defaultValue: 1m),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestProducts", x => x.Id);
                    table.CheckConstraint("CK_RequestProducts_Quantity", "[Quantity]>(0)");
                    table.ForeignKey(
                        name: "FK_RequestProducts_ProductCategories",
                        column: x => x.ProductCategoryId,
                        principalSchema: "dbo",
                        principalTable: "ProductCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RequestProducts_Products",
                        column: x => x.ProductId,
                        principalSchema: "dbo",
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RequestProducts_Requests",
                        column: x => x.RequestId,
                        principalSchema: "dbo",
                        principalTable: "Requests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ConversationParticipants",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConversationId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "sysutcdatetime()"),
                    LeftAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConversationParticipants_Conversations",
                        column: x => x.ConversationId,
                        principalSchema: "dbo",
                        principalTable: "Conversations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ConversationParticipants_Users",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConversationId = table.Column<long>(type: "bigint", nullable: false),
                    SenderUserId = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "sysutcdatetime()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Conversations",
                        column: x => x.ConversationId,
                        principalSchema: "dbo",
                        principalTable: "Conversations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Messages_Users",
                        column: x => x.SenderUserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Deals",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<long>(type: "bigint", nullable: false),
                    ProposalId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Active"),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "sysutcdatetime()"),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deals", x => x.Id);
                    table.CheckConstraint("CK_Deals_TotalPrice", "[TotalPrice]>=(0)");
                    table.ForeignKey(
                        name: "FK_Deals_Customers",
                        column: x => x.CustomerId,
                        principalSchema: "dbo",
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Deals_Proposals",
                        column: x => x.ProposalId,
                        principalSchema: "dbo",
                        principalTable: "Proposals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Deals_Requests",
                        column: x => x.RequestId,
                        principalSchema: "dbo",
                        principalTable: "Requests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProposalItems",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProposalId = table.Column<long>(type: "bigint", nullable: false),
                    ItemType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: true),
                    ServiceId = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false, defaultValue: 1m),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProposalItems", x => x.Id);
                    table.CheckConstraint("CK_ProposalItems_Prices", "[UnitPrice]>=(0) AND [TotalPrice]>=(0)");
                    table.CheckConstraint("CK_ProposalItems_Quantity", "[Quantity]>(0)");
                    table.CheckConstraint("CK_ProposalItems_Type", "[ItemType]=N'Product' AND [ProductId] IS NOT NULL AND [ServiceId] IS NULL OR [ItemType]=N'Service' AND [ProductId] IS NULL AND [ServiceId] IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_ProposalItems_Products",
                        column: x => x.ProductId,
                        principalSchema: "dbo",
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProposalItems_Proposals",
                        column: x => x.ProposalId,
                        principalSchema: "dbo",
                        principalTable: "Proposals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProposalItems_Services",
                        column: x => x.ServiceId,
                        principalSchema: "dbo",
                        principalTable: "Services",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RequestServiceAttributes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestServiceId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceAttributeId = table.Column<long>(type: "bigint", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestServiceAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestServiceAttributes_RequestServices",
                        column: x => x.RequestServiceId,
                        principalSchema: "dbo",
                        principalTable: "RequestServices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RequestServiceAttributes_ServiceAttributes",
                        column: x => x.ServiceAttributeId,
                        principalSchema: "dbo",
                        principalTable: "ServiceAttributes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RequestProductAttributes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestProductId = table.Column<long>(type: "bigint", nullable: false),
                    ProductAttributeId = table.Column<long>(type: "bigint", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestProductAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestProductAttributes_ProductAttributes",
                        column: x => x.ProductAttributeId,
                        principalSchema: "dbo",
                        principalTable: "ProductAttributes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RequestProductAttributes_RequestProducts",
                        column: x => x.RequestProductId,
                        principalSchema: "dbo",
                        principalTable: "RequestProducts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Cancellations",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<long>(type: "bigint", nullable: false),
                    DealId = table.Column<long>(type: "bigint", nullable: true),
                    CancelledByUserId = table.Column<long>(type: "bigint", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "sysutcdatetime()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cancellations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cancellations_Deals",
                        column: x => x.DealId,
                        principalSchema: "dbo",
                        principalTable: "Deals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Cancellations_Requests",
                        column: x => x.RequestId,
                        principalSchema: "dbo",
                        principalTable: "Requests",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Cancellations_Users",
                        column: x => x.CancelledByUserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Complaints",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<long>(type: "bigint", nullable: false),
                    DealId = table.Column<long>(type: "bigint", nullable: true),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    BusinessId = table.Column<long>(type: "bigint", nullable: true),
                    ProviderId = table.Column<long>(type: "bigint", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Open"),
                    Resolution = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Complaints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Complaints_Businesses",
                        column: x => x.BusinessId,
                        principalSchema: "dbo",
                        principalTable: "Businesses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Complaints_Customers",
                        column: x => x.CustomerId,
                        principalSchema: "dbo",
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Complaints_Deals",
                        column: x => x.DealId,
                        principalSchema: "dbo",
                        principalTable: "Deals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Complaints_Providers",
                        column: x => x.ProviderId,
                        principalSchema: "dbo",
                        principalTable: "Providers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Complaints_Requests",
                        column: x => x.RequestId,
                        principalSchema: "dbo",
                        principalTable: "Requests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductDeliveries",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DealId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Pending"),
                    Address = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Province = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    District = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Lat = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    Lng = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    ScheduledDate = table.Column<DateOnly>(type: "date", nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrackingCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDeliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductDeliveries_Deals",
                        column: x => x.DealId,
                        principalSchema: "dbo",
                        principalTable: "Deals",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ServiceExecutions",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DealId = table.Column<long>(type: "bigint", nullable: false),
                    BusinessId = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Pending"),
                    ScheduledDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ScheduledTimeFrom = table.Column<TimeSpan>(type: "time", nullable: true),
                    ScheduledTimeTo = table.Column<TimeSpan>(type: "time", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceExecutions", x => x.Id);
                    table.CheckConstraint("CK_ServiceExecutions_Time", "[ScheduledTimeFrom] IS NULL OR [ScheduledTimeTo] IS NULL OR [ScheduledTimeFrom]<[ScheduledTimeTo]");
                    table.ForeignKey(
                        name: "FK_ServiceExecutions_Businesses",
                        column: x => x.BusinessId,
                        principalSchema: "dbo",
                        principalTable: "Businesses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServiceExecutions_Deals",
                        column: x => x.DealId,
                        principalSchema: "dbo",
                        principalTable: "Deals",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ExecutionAssignments",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceExecutionId = table.Column<long>(type: "bigint", nullable: false),
                    ProviderId = table.Column<long>(type: "bigint", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Assigned"),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "sysutcdatetime()"),
                    StartAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExecutionAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExecutionAssignments_Providers",
                        column: x => x.ProviderId,
                        principalSchema: "dbo",
                        principalTable: "Providers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExecutionAssignments_ServiceExecutions",
                        column: x => x.ServiceExecutionId,
                        principalSchema: "dbo",
                        principalTable: "ServiceExecutions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "UX_Users_Mobile",
                schema: "dbo",
                table: "Users",
                column: "Mobile",
                unique: true,
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "UX_Services_Slug",
                schema: "dbo",
                table: "Services",
                column: "Slug",
                unique: true,
                filter: "([Slug] IS NOT NULL AND [IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "UX_ServiceCategories_Slug",
                schema: "dbo",
                table: "ServiceCategories",
                column: "Slug",
                unique: true,
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BusinessId",
                schema: "dbo",
                table: "Reviews",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_DealId",
                schema: "dbo",
                table: "Reviews",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProviderId",
                schema: "dbo",
                table: "Reviews",
                column: "ProviderId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Reviews_Rating",
                schema: "dbo",
                table: "Reviews",
                sql: "[Rating]>=(1) AND [Rating]<=(5)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Reviews_Target",
                schema: "dbo",
                table: "Reviews",
                sql: "[BusinessId] IS NOT NULL OR [ProviderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_ProviderServices",
                schema: "dbo",
                table: "ProviderServices",
                columns: new[] { "ProviderId", "ServiceId" },
                unique: true,
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "UX_Providers_UserId",
                schema: "dbo",
                table: "Providers",
                column: "UserId",
                unique: true,
                filter: "([IsDeleted]=(0))");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Providers_Rating",
                schema: "dbo",
                table: "Providers",
                sql: "[Rating]>=(0) AND [Rating]<=(5)");

            migrationBuilder.CreateIndex(
                name: "UX_BusinessProviders_Active",
                schema: "dbo",
                table: "BusinessProviders",
                columns: new[] { "BusinessId", "ProviderId" },
                unique: true,
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_LogoMediaId",
                schema: "dbo",
                table: "Businesses",
                column: "LogoMediaId");

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_OwnerUserId",
                schema: "dbo",
                table: "Businesses",
                column: "OwnerUserId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Businesses_Rating",
                schema: "dbo",
                table: "Businesses",
                sql: "[Rating]>=(0) AND [Rating]<=(5)");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessAvailabilities",
                schema: "dbo",
                table: "BusinessAvailabilities",
                columns: new[] { "BusinessId", "DayOfWeek" });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessPortfolioMedia_MediaId",
                schema: "dbo",
                table: "BusinessPortfolioMedia",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessPortfolioMedia_PortfolioId",
                schema: "dbo",
                table: "BusinessPortfolioMedia",
                columns: new[] { "PortfolioId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessPortfolios_BusinessId",
                schema: "dbo",
                table: "BusinessPortfolios",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessProducts_ProductId",
                schema: "dbo",
                table: "BusinessProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "UX_BusinessProducts",
                schema: "dbo",
                table: "BusinessProducts",
                columns: new[] { "BusinessId", "ProductId" },
                unique: true,
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessServiceAreas",
                schema: "dbo",
                table: "BusinessServiceAreas",
                columns: new[] { "BusinessId", "City", "District" });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessServices_ServiceId",
                schema: "dbo",
                table: "BusinessServices",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "UX_BusinessServices",
                schema: "dbo",
                table: "BusinessServices",
                columns: new[] { "BusinessId", "ServiceId" },
                unique: true,
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_Cancellations_CancelledByUserId",
                schema: "dbo",
                table: "Cancellations",
                column: "CancelledByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Cancellations_DealId",
                schema: "dbo",
                table: "Cancellations",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_Cancellations_RequestId",
                schema: "dbo",
                table: "Cancellations",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_BusinessId",
                schema: "dbo",
                table: "Complaints",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_CustomerId",
                schema: "dbo",
                table: "Complaints",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_DealId_Status",
                schema: "dbo",
                table: "Complaints",
                columns: new[] { "DealId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_ProviderId",
                schema: "dbo",
                table: "Complaints",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_RequestId_Status",
                schema: "dbo",
                table: "Complaints",
                columns: new[] { "RequestId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ConversationParticipants_ConversationId",
                schema: "dbo",
                table: "ConversationParticipants",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationParticipants_UserId",
                schema: "dbo",
                table: "ConversationParticipants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_BusinessId",
                schema: "dbo",
                table: "Conversations",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_CustomerId",
                schema: "dbo",
                table: "Conversations",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_RequestId",
                schema: "dbo",
                table: "Conversations",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "UX_Customers_UserId",
                schema: "dbo",
                table: "Customers",
                column: "UserId",
                unique: true,
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_CustomerId_Status",
                schema: "dbo",
                table: "Deals",
                columns: new[] { "CustomerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Deals_RequestId_Status",
                schema: "dbo",
                table: "Deals",
                columns: new[] { "RequestId", "Status" });

            migrationBuilder.CreateIndex(
                name: "UX_Deals_ProposalId",
                schema: "dbo",
                table: "Deals",
                column: "ProposalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionAssignments_ProviderId",
                schema: "dbo",
                table: "ExecutionAssignments",
                columns: new[] { "ProviderId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionAssignments_ServiceExecutionId",
                schema: "dbo",
                table: "ExecutionAssignments",
                column: "ServiceExecutionId");

            migrationBuilder.CreateIndex(
                name: "UX_Media_StorageKey",
                schema: "dbo",
                table: "Media",
                column: "StorageKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ConversationId_CreatedAt",
                schema: "dbo",
                table: "Messages",
                columns: new[] { "ConversationId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderUserId",
                schema: "dbo",
                table: "Messages",
                column: "SenderUserId");

            migrationBuilder.CreateIndex(
                name: "UX_Permissions_Code",
                schema: "dbo",
                table: "Permissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Permissions_Name",
                schema: "dbo",
                table: "Permissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_ProductAttributeOptions_Value",
                schema: "dbo",
                table: "ProductAttributeOptions",
                columns: new[] { "ProductAttributeId", "Value" },
                unique: true,
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "UX_ProductAttributes_Code",
                schema: "dbo",
                table: "ProductAttributes",
                columns: new[] { "ProductCategoryId", "Code" },
                unique: true,
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValues_ProductAttributeId",
                schema: "dbo",
                table: "ProductAttributeValues",
                column: "ProductAttributeId");

            migrationBuilder.CreateIndex(
                name: "UX_ProductAttributeValues_ProductAttribute",
                schema: "dbo",
                table: "ProductAttributeValues",
                columns: new[] { "ProductId", "ProductAttributeId" },
                unique: true,
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_ParentId",
                schema: "dbo",
                table: "ProductCategories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "UX_ProductCategories_Slug",
                schema: "dbo",
                table: "ProductCategories",
                column: "Slug",
                unique: true,
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDeliveries_DealId",
                schema: "dbo",
                table: "ProductDeliveries",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                schema: "dbo",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "UX_Products_SKU",
                schema: "dbo",
                table: "Products",
                column: "SKU",
                unique: true,
                filter: "([SKU] IS NOT NULL AND [IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "UX_Products_Slug",
                schema: "dbo",
                table: "Products",
                column: "Slug",
                unique: true,
                filter: "([Slug] IS NOT NULL AND [IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_ProposalItems_ProductId",
                schema: "dbo",
                table: "ProposalItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProposalItems_ProposalId",
                schema: "dbo",
                table: "ProposalItems",
                columns: new[] { "ProposalId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_ProposalItems_ServiceId",
                schema: "dbo",
                table: "ProposalItems",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_BusinessId",
                schema: "dbo",
                table: "Proposals",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_ProviderId",
                schema: "dbo",
                table: "Proposals",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_RequestId_Status",
                schema: "dbo",
                table: "Proposals",
                columns: new[] { "RequestId", "Status", "CreateDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ProviderAvailabilities",
                schema: "dbo",
                table: "ProviderAvailabilities",
                columns: new[] { "ProviderId", "DayOfWeek" });

            migrationBuilder.CreateIndex(
                name: "IX_ProviderCapabilities",
                schema: "dbo",
                table: "ProviderCapabilities",
                columns: new[] { "ProviderId", "ServiceAttributeId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProviderCapabilities_ServiceAttributeId",
                schema: "dbo",
                table: "ProviderCapabilities",
                column: "ServiceAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderPortfolioMedia_MediaId",
                schema: "dbo",
                table: "ProviderPortfolioMedia",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderPortfolioMedia_PortfolioId",
                schema: "dbo",
                table: "ProviderPortfolioMedia",
                columns: new[] { "PortfolioId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_ProviderPortfolios_ProviderId",
                schema: "dbo",
                table: "ProviderPortfolios",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderProducts_ProductId",
                schema: "dbo",
                table: "ProviderProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "UX_ProviderProducts",
                schema: "dbo",
                table: "ProviderProducts",
                columns: new[] { "ProviderId", "ProductId" },
                unique: true,
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderServiceAreas",
                schema: "dbo",
                table: "ProviderServiceAreas",
                columns: new[] { "ProviderId", "City", "District" });

            migrationBuilder.CreateIndex(
                name: "IX_RequestLocations_RequestId",
                schema: "dbo",
                table: "RequestLocations",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestProductAttributes_ProductAttributeId",
                schema: "dbo",
                table: "RequestProductAttributes",
                column: "ProductAttributeId");

            migrationBuilder.CreateIndex(
                name: "UX_RequestProductAttributes",
                schema: "dbo",
                table: "RequestProductAttributes",
                columns: new[] { "RequestProductId", "ProductAttributeId" },
                unique: true,
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_RequestProducts_CategoryId",
                schema: "dbo",
                table: "RequestProducts",
                column: "ProductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestProducts_ProductId",
                schema: "dbo",
                table: "RequestProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestProducts_RequestId",
                schema: "dbo",
                table: "RequestProducts",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_CustomerId_Status",
                schema: "dbo",
                table: "Requests",
                columns: new[] { "CustomerId", "Status", "CreateDate" });

            migrationBuilder.CreateIndex(
                name: "IX_RequestSchedules_RequestId_Date",
                schema: "dbo",
                table: "RequestSchedules",
                columns: new[] { "RequestId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_RequestServiceAttributes_ServiceAttributeId",
                schema: "dbo",
                table: "RequestServiceAttributes",
                column: "ServiceAttributeId");

            migrationBuilder.CreateIndex(
                name: "UX_RequestServiceAttributes",
                schema: "dbo",
                table: "RequestServiceAttributes",
                columns: new[] { "RequestServiceId", "ServiceAttributeId" },
                unique: true,
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_RequestServices_RequestId",
                schema: "dbo",
                table: "RequestServices",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestServices_ServiceId",
                schema: "dbo",
                table: "RequestServices",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                schema: "dbo",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "UX_Roles_Code",
                schema: "dbo",
                table: "Roles",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Roles_Name",
                schema: "dbo",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_ServiceAttributeOptions_Value",
                schema: "dbo",
                table: "ServiceAttributeOptions",
                columns: new[] { "ServiceAttributeId", "Value" },
                unique: true,
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "UX_ServiceAttributes_Code",
                schema: "dbo",
                table: "ServiceAttributes",
                columns: new[] { "ServiceId", "Code" },
                unique: true,
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceExecutions_BusinessId",
                schema: "dbo",
                table: "ServiceExecutions",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceExecutions_DealId",
                schema: "dbo",
                table: "ServiceExecutions",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_TrustScores_Entity",
                schema: "dbo",
                table: "TrustScores",
                columns: new[] { "EntityType", "EntityId", "CalculatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "dbo",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationDocuments_MediaId",
                schema: "dbo",
                table: "VerificationDocuments",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationDocuments_VerificationId",
                schema: "dbo",
                table: "VerificationDocuments",
                column: "VerificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Verifications_Entity",
                schema: "dbo",
                table: "Verifications",
                columns: new[] { "EntityType", "EntityId", "Status" });

            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_LogoMedia",
                schema: "dbo",
                table: "Businesses",
                column: "LogoMediaId",
                principalSchema: "dbo",
                principalTable: "Media",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_Users",
                schema: "dbo",
                table: "Businesses",
                column: "OwnerUserId",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessProviders_Businesses",
                schema: "dbo",
                table: "BusinessProviders",
                column: "BusinessId",
                principalSchema: "dbo",
                principalTable: "Businesses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessProviders_Providers",
                schema: "dbo",
                table: "BusinessProviders",
                column: "ProviderId",
                principalSchema: "dbo",
                principalTable: "Providers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Providers_Users",
                schema: "dbo",
                table: "Providers",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderServices_Providers",
                schema: "dbo",
                table: "ProviderServices",
                column: "ProviderId",
                principalSchema: "dbo",
                principalTable: "Providers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderServices_Services",
                schema: "dbo",
                table: "ProviderServices",
                column: "ServiceId",
                principalSchema: "dbo",
                principalTable: "Services",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Businesses",
                schema: "dbo",
                table: "Reviews",
                column: "BusinessId",
                principalSchema: "dbo",
                principalTable: "Businesses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Customers",
                schema: "dbo",
                table: "Reviews",
                column: "CustomerId",
                principalSchema: "dbo",
                principalTable: "Customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Deals",
                schema: "dbo",
                table: "Reviews",
                column: "DealId",
                principalSchema: "dbo",
                principalTable: "Deals",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Providers",
                schema: "dbo",
                table: "Reviews",
                column: "ProviderId",
                principalSchema: "dbo",
                principalTable: "Providers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_ServiceCategories",
                schema: "dbo",
                table: "Services",
                column: "CategoryId",
                principalSchema: "dbo",
                principalTable: "ServiceCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_LogoMedia",
                schema: "dbo",
                table: "Businesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_Users",
                schema: "dbo",
                table: "Businesses");

            migrationBuilder.DropForeignKey(
                name: "FK_BusinessProviders_Businesses",
                schema: "dbo",
                table: "BusinessProviders");

            migrationBuilder.DropForeignKey(
                name: "FK_BusinessProviders_Providers",
                schema: "dbo",
                table: "BusinessProviders");

            migrationBuilder.DropForeignKey(
                name: "FK_Providers_Users",
                schema: "dbo",
                table: "Providers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProviderServices_Providers",
                schema: "dbo",
                table: "ProviderServices");

            migrationBuilder.DropForeignKey(
                name: "FK_ProviderServices_Services",
                schema: "dbo",
                table: "ProviderServices");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Businesses",
                schema: "dbo",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Customers",
                schema: "dbo",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Deals",
                schema: "dbo",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Providers",
                schema: "dbo",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_ServiceCategories",
                schema: "dbo",
                table: "Services");

            migrationBuilder.DropTable(
                name: "BusinessAvailabilities",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BusinessPortfolioMedia",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BusinessProducts",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BusinessServiceAreas",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BusinessServices",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Cancellations",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Complaints",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ConversationParticipants",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ExecutionAssignments",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Messages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ProductAttributeOptions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ProductAttributeValues",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ProductDeliveries",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ProposalItems",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ProviderAvailabilities",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ProviderCapabilities",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ProviderPortfolioMedia",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ProviderProducts",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ProviderServiceAreas",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RequestLocations",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RequestProductAttributes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RequestSchedules",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RequestServiceAttributes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RolePermissions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ServiceAttributeOptions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TrustScores",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "VerificationDocuments",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BusinessPortfolios",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ServiceExecutions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Conversations",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ProviderPortfolios",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ProductAttributes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RequestProducts",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RequestServices",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Permissions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ServiceAttributes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Media",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Verifications",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Deals",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Proposals",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ProductCategories",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Requests",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Customers",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "UX_Users_Mobile",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "UX_Services_Slug",
                schema: "dbo",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "UX_ServiceCategories_Slug",
                schema: "dbo",
                table: "ServiceCategories");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_BusinessId",
                schema: "dbo",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_DealId",
                schema: "dbo",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ProviderId",
                schema: "dbo",
                table: "Reviews");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Reviews_Rating",
                schema: "dbo",
                table: "Reviews");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Reviews_Target",
                schema: "dbo",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "UX_ProviderServices",
                schema: "dbo",
                table: "ProviderServices");

            migrationBuilder.DropIndex(
                name: "UX_Providers_UserId",
                schema: "dbo",
                table: "Providers");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Providers_Rating",
                schema: "dbo",
                table: "Providers");

            migrationBuilder.DropIndex(
                name: "UX_BusinessProviders_Active",
                schema: "dbo",
                table: "BusinessProviders");

            migrationBuilder.DropIndex(
                name: "IX_Businesses_LogoMediaId",
                schema: "dbo",
                table: "Businesses");

            migrationBuilder.DropIndex(
                name: "IX_Businesses_OwnerUserId",
                schema: "dbo",
                table: "Businesses");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Businesses_Rating",
                schema: "dbo",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "dbo",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                schema: "dbo",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "dbo",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Slug",
                schema: "dbo",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                schema: "dbo",
                table: "ServiceCategories");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "dbo",
                table: "ServiceCategories");

            migrationBuilder.DropColumn(
                name: "BusinessId",
                schema: "dbo",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                schema: "dbo",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "dbo",
                table: "ProviderServices");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "dbo",
                table: "ProviderServices");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "ProviderServices");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                schema: "dbo",
                table: "ProviderServices");

            migrationBuilder.DropColumn(
                name: "CompletedJobCount",
                schema: "dbo",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "dbo",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "ReviewCount",
                schema: "dbo",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "dbo",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "dbo",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                schema: "dbo",
                table: "BusinessProviders");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "BusinessProviders");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                schema: "dbo",
                table: "BusinessProviders");

            migrationBuilder.DropColumn(
                name: "City",
                schema: "dbo",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "CompletedJobCount",
                schema: "dbo",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "dbo",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "District",
                schema: "dbo",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "LogoMediaId",
                schema: "dbo",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "Mobile",
                schema: "dbo",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "OwnerUserId",
                schema: "dbo",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "Province",
                schema: "dbo",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "Rating",
                schema: "dbo",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "ReviewCount",
                schema: "dbo",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "dbo",
                table: "Businesses");

            migrationBuilder.RenameColumn(
                name: "DealId",
                schema: "dbo",
                table: "Reviews",
                newName: "TargetId");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                schema: "dbo",
                table: "Reviews",
                newName: "IntroductionId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_CustomerId",
                schema: "dbo",
                table: "Reviews",
                newName: "IX_Reviews_IntroductionId");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Users",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                schema: "dbo",
                table: "Users",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Services",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                schema: "dbo",
                table: "Services",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "ServiceCategories",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                schema: "dbo",
                table: "ServiceCategories",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AlterColumn<int>(
                name: "Rating",
                schema: "dbo",
                table: "Reviews",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Reviews",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                schema: "dbo",
                table: "Reviews",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                schema: "dbo",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetType",
                schema: "dbo",
                table: "Reviews",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<double>(
                name: "Rating",
                schema: "dbo",
                table: "Providers",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(decimal),
                oldType: "decimal(3,2)",
                oldPrecision: 3,
                oldScale: 2,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<double>(
                name: "Lng",
                schema: "dbo",
                table: "Providers",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,6)",
                oldPrecision: 9,
                oldScale: 6,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Lat",
                schema: "dbo",
                table: "Providers",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,6)",
                oldPrecision: 9,
                oldScale: 6,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Providers",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                schema: "dbo",
                table: "Providers",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "dbo",
                table: "Providers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "dbo",
                table: "BusinessProviders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldDefaultValue: "Active");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                schema: "dbo",
                table: "BusinessProviders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "JoinedAt",
                schema: "dbo",
                table: "BusinessProviders",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "sysutcdatetime()");

            migrationBuilder.AlterColumn<double>(
                name: "Lng",
                schema: "dbo",
                table: "Businesses",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,6)",
                oldPrecision: 9,
                oldScale: 6,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Lat",
                schema: "dbo",
                table: "Businesses",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,6)",
                oldPrecision: 9,
                oldScale: 6,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Businesses",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                schema: "dbo",
                table: "Businesses",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                schema: "dbo",
                table: "Businesses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Banks",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateDate = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EitaaLink = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RubikaLink = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    SeoDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SeoTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ShortDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Slug = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TelegramLink = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceQuestions",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceQuestions_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "dbo",
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequests",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Lat = table.Column<double>(type: "float", nullable: true),
                    Lng = table.Column<double>(type: "float", nullable: true),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceId1 = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceRequests_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "dbo",
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceRequests_Services_ServiceId1",
                        column: x => x.ServiceId1,
                        principalSchema: "dbo",
                        principalTable: "Services",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BankQuestion",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankQuestion_Banks_BankId",
                        column: x => x.BankId,
                        principalSchema: "dbo",
                        principalTable: "Banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LoanRequest",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanRequest_Banks_BankId",
                        column: x => x.BankId,
                        principalSchema: "dbo",
                        principalTable: "Banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoanRequest_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionOptions",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceQuestionId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionOptions_ServiceQuestions_ServiceQuestionId",
                        column: x => x.ServiceQuestionId,
                        principalSchema: "dbo",
                        principalTable: "ServiceQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Introductions",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceRequestId = table.Column<long>(type: "bigint", nullable: false),
                    AssignedProviderId = table.Column<long>(type: "bigint", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TargetId = table.Column<long>(type: "bigint", nullable: false),
                    TargetType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Introductions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Introductions_ServiceRequests_ServiceRequestId",
                        column: x => x.ServiceRequestId,
                        principalSchema: "dbo",
                        principalTable: "ServiceRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestAnswers",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SelectedOptionId = table.Column<long>(type: "bigint", nullable: true),
                    ServiceRequestId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceQuestionId = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestAnswers_QuestionOptions_SelectedOptionId",
                        column: x => x.SelectedOptionId,
                        principalSchema: "dbo",
                        principalTable: "QuestionOptions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RequestAnswers_ServiceRequests_ServiceRequestId",
                        column: x => x.ServiceRequestId,
                        principalSchema: "dbo",
                        principalTable: "ServiceRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Mobile",
                schema: "dbo",
                table: "Users",
                column: "Mobile",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProviderServices_ProviderId",
                schema: "dbo",
                table: "ProviderServices",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_BankQuestion_BankId",
                schema: "dbo",
                table: "BankQuestion",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_Banks_IsActive_SortOrder",
                schema: "dbo",
                table: "Banks",
                columns: new[] { "IsActive", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Banks_Slug",
                schema: "dbo",
                table: "Banks",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Banks_Title",
                schema: "dbo",
                table: "Banks",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Introductions_ServiceRequestId",
                schema: "dbo",
                table: "Introductions",
                column: "ServiceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRequest_BankId",
                schema: "dbo",
                table: "LoanRequest",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRequest_UserId",
                schema: "dbo",
                table: "LoanRequest",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_ServiceQuestionId",
                schema: "dbo",
                table: "QuestionOptions",
                column: "ServiceQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestAnswers_SelectedOptionId",
                schema: "dbo",
                table: "RequestAnswers",
                column: "SelectedOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestAnswers_ServiceRequestId",
                schema: "dbo",
                table: "RequestAnswers",
                column: "ServiceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceQuestions_ServiceId",
                schema: "dbo",
                table: "ServiceQuestions",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_ServiceId",
                schema: "dbo",
                table: "ServiceRequests",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_ServiceId1",
                schema: "dbo",
                table: "ServiceRequests",
                column: "ServiceId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessProviders_Businesses_BusinessId",
                schema: "dbo",
                table: "BusinessProviders",
                column: "BusinessId",
                principalSchema: "dbo",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessProviders_Providers_ProviderId",
                schema: "dbo",
                table: "BusinessProviders",
                column: "ProviderId",
                principalSchema: "dbo",
                principalTable: "Providers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderServices_Providers_ProviderId",
                schema: "dbo",
                table: "ProviderServices",
                column: "ProviderId",
                principalSchema: "dbo",
                principalTable: "Providers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderServices_Services_ServiceId",
                schema: "dbo",
                table: "ProviderServices",
                column: "ServiceId",
                principalSchema: "dbo",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Introductions_IntroductionId",
                schema: "dbo",
                table: "Reviews",
                column: "IntroductionId",
                principalSchema: "dbo",
                principalTable: "Introductions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_ServiceCategories_CategoryId",
                schema: "dbo",
                table: "Services",
                column: "CategoryId",
                principalSchema: "dbo",
                principalTable: "ServiceCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
