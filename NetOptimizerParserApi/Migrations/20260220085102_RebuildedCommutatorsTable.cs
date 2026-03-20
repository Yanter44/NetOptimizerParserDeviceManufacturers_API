using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetOptimizerParserApi.Migrations
{
    /// <inheritdoc />
    public partial class RebuildedCommutatorsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForwardingRateMpps",
                table: "CommutatorsTable");

            migrationBuilder.DropColumn(
                name: "SwitchingCapacityGbps",
                table: "CommutatorsTable");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ForwardingRateMpps",
                table: "CommutatorsTable",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SwitchingCapacityGbps",
                table: "CommutatorsTable",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
