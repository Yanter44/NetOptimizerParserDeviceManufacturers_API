using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NetOptimizerParserApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedPcEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PcTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Vendor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    HardwareSpecs_CpuModel = table.Column<string>(type: "text", nullable: false),
                    HardwareSpecs_RamAmountGb = table.Column<int>(type: "integer", nullable: false),
                    HardwareSpecs_RamType = table.Column<string>(type: "text", nullable: false),
                    HardwareSpecs_StorageAmountGb = table.Column<int>(type: "integer", nullable: false),
                    HardwareSpecs_StorageType = table.Column<string>(type: "text", nullable: false),
                    WifiOptions_HasWiFi = table.Column<bool>(type: "boolean", nullable: false),
                    AveragePrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Ports = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PcTable", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PcTable");
        }
    }
}
