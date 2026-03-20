using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetOptimizerParserApi.Migrations
{
    /// <inheritdoc />
    public partial class TestMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "RoutersTable",
                newName: "Model");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "CommutatorsTable",
                newName: "Model");

            migrationBuilder.AddColumn<bool>(
                name: "IsManaged",
                table: "CommutatorsTable",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MacTableSize",
                table: "CommutatorsTable",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxVlans",
                table: "CommutatorsTable",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PoeBudgetW",
                table: "CommutatorsTable",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ThroughputGbps",
                table: "CommutatorsTable",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsManaged",
                table: "CommutatorsTable");

            migrationBuilder.DropColumn(
                name: "MacTableSize",
                table: "CommutatorsTable");

            migrationBuilder.DropColumn(
                name: "MaxVlans",
                table: "CommutatorsTable");

            migrationBuilder.DropColumn(
                name: "PoeBudgetW",
                table: "CommutatorsTable");

            migrationBuilder.DropColumn(
                name: "ThroughputGbps",
                table: "CommutatorsTable");

            migrationBuilder.RenameColumn(
                name: "Model",
                table: "RoutersTable",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Model",
                table: "CommutatorsTable",
                newName: "Name");
        }
    }
}
