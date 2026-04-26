using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetOptimizerParserApi.Migrations
{
    /// <inheritdoc />
    public partial class FUUUCK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ThroughputGbps",
                table: "CommutatorsTable",
                newName: "PerformanceSpecs_ThroughputGbps");

            migrationBuilder.RenameColumn(
                name: "SupportsPoe",
                table: "CommutatorsTable",
                newName: "PoeSpecs_SupportsPoe");

            migrationBuilder.RenameColumn(
                name: "PoeBudgetW",
                table: "CommutatorsTable",
                newName: "PoeSpecs_PoeBudgetW");

            migrationBuilder.RenameColumn(
                name: "MaxVlans",
                table: "CommutatorsTable",
                newName: "PerformanceSpecs_MaxVlans");

            migrationBuilder.RenameColumn(
                name: "MacTableSize",
                table: "CommutatorsTable",
                newName: "PerformanceSpecs_MacTableSize");

            migrationBuilder.AddColumn<int>(
                name: "ProtocolSupport_QosQueuesPerPort",
                table: "CommutatorsTable",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ProtocolSupport_SupportsLacp",
                table: "CommutatorsTable",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ProtocolSupport_SupportsLag",
                table: "CommutatorsTable",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ProtocolSupport_SupportsLoopProtection",
                table: "CommutatorsTable",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ProtocolSupport_SupportsQos",
                table: "CommutatorsTable",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SwitchRoleType",
                table: "CommutatorsTable",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProtocolSupport_QosQueuesPerPort",
                table: "CommutatorsTable");

            migrationBuilder.DropColumn(
                name: "ProtocolSupport_SupportsLacp",
                table: "CommutatorsTable");

            migrationBuilder.DropColumn(
                name: "ProtocolSupport_SupportsLag",
                table: "CommutatorsTable");

            migrationBuilder.DropColumn(
                name: "ProtocolSupport_SupportsLoopProtection",
                table: "CommutatorsTable");

            migrationBuilder.DropColumn(
                name: "ProtocolSupport_SupportsQos",
                table: "CommutatorsTable");

            migrationBuilder.DropColumn(
                name: "SwitchRoleType",
                table: "CommutatorsTable");

            migrationBuilder.RenameColumn(
                name: "PoeSpecs_SupportsPoe",
                table: "CommutatorsTable",
                newName: "SupportsPoe");

            migrationBuilder.RenameColumn(
                name: "PoeSpecs_PoeBudgetW",
                table: "CommutatorsTable",
                newName: "PoeBudgetW");

            migrationBuilder.RenameColumn(
                name: "PerformanceSpecs_ThroughputGbps",
                table: "CommutatorsTable",
                newName: "ThroughputGbps");

            migrationBuilder.RenameColumn(
                name: "PerformanceSpecs_MaxVlans",
                table: "CommutatorsTable",
                newName: "MaxVlans");

            migrationBuilder.RenameColumn(
                name: "PerformanceSpecs_MacTableSize",
                table: "CommutatorsTable",
                newName: "MacTableSize");
        }
    }
}
