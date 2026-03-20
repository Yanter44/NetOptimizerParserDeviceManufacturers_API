using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NetOptimizerParserApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedRoutersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SfpPorts",
                table: "CommutatorsTable");

            migrationBuilder.DropColumn(
                name: "TotalPorts",
                table: "CommutatorsTable");

            migrationBuilder.AlterColumn<string>(
                name: "Vendor",
                table: "CommutatorsTable",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Model",
                table: "CommutatorsTable",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "AveragePrice",
                table: "CommutatorsTable",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<string>(
                name: "Ports",
                table: "CommutatorsTable",
                type: "jsonb",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RoutersTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Vendor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ModelName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    HasWiFi = table.Column<bool>(type: "boolean", nullable: false),
                    WiFiStandard = table.Column<string>(type: "text", nullable: false),
                    MaxWirelessSpeed = table.Column<double>(type: "double precision", nullable: false),
                    AntennaCount = table.Column<int>(type: "integer", nullable: false),
                    AntennaGain = table.Column<double>(type: "double precision", nullable: false),
                    FrequencyBands = table.Column<string>(type: "text", nullable: false),
                    FirewallThroughputGbps = table.Column<double>(type: "double precision", nullable: false),
                    VpnThroughputMbps = table.Column<double>(type: "double precision", nullable: false),
                    MaxSessions = table.Column<int>(type: "integer", nullable: false),
                    PowerType = table.Column<string>(type: "text", nullable: false),
                    SupportsOspf = table.Column<bool>(type: "boolean", nullable: false),
                    SupportsBgp = table.Column<bool>(type: "boolean", nullable: false),
                    SupportsMpls = table.Column<bool>(type: "boolean", nullable: false),
                    SupportsVrrp = table.Column<bool>(type: "boolean", nullable: false),
                    SupportsIpsec = table.Column<bool>(type: "boolean", nullable: false),
                    SupportsNat = table.Column<bool>(type: "boolean", nullable: false),
                    AvveragePrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Ports = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutersTable", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoutersTable");

            migrationBuilder.DropColumn(
                name: "Ports",
                table: "CommutatorsTable");

            migrationBuilder.AlterColumn<string>(
                name: "Vendor",
                table: "CommutatorsTable",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Model",
                table: "CommutatorsTable",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<decimal>(
                name: "AveragePrice",
                table: "CommutatorsTable",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddColumn<int>(
                name: "SfpPorts",
                table: "CommutatorsTable",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalPorts",
                table: "CommutatorsTable",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
