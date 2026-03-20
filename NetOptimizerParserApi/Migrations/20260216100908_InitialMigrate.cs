using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NetOptimizerParserApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommutatorsTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false).Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Vendor = table.Column<string>(type: "text", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    Layer = table.Column<int>(type: "integer", nullable: false),
                    TotalPorts = table.Column<int>(type: "integer", nullable: false),
                    SfpPorts = table.Column<int>(type: "integer", nullable: false),
                    SupportsPoe = table.Column<bool>(type: "boolean", nullable: false),
                    SwitchingCapacityGbps = table.Column<double>(type: "double precision", nullable: false),
                    ForwardingRateMpps = table.Column<double>(type: "double precision", nullable: false),
                    AveragePrice = table.Column<decimal>(type: "numeric", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommutatorsTable", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommutatorsTable");
        }
    }
}
