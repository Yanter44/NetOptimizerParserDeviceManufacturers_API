using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetOptimizerParserApi.Migrations
{
    /// <inheritdoc />
    public partial class SomeFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirewallThroughputGbps",
                table: "RoutersTable");

            migrationBuilder.DropColumn(
                name: "MaxSessions",
                table: "RoutersTable");

            migrationBuilder.DropColumn(
                name: "PowerType",
                table: "RoutersTable");

            migrationBuilder.DropColumn(
                name: "SupportsBgp",
                table: "RoutersTable");

            migrationBuilder.DropColumn(
                name: "SupportsMpls",
                table: "RoutersTable");

            migrationBuilder.DropColumn(
                name: "VpnThroughputMbps",
                table: "RoutersTable");

            migrationBuilder.RenameColumn(
                name: "WiFiStandard",
                table: "RoutersTable",
                newName: "WifiOptions_WiFiStandard");

            migrationBuilder.RenameColumn(
                name: "SupportsVrrp",
                table: "RoutersTable",
                newName: "ProtocolSupport_SupportsVrrp");

            migrationBuilder.RenameColumn(
                name: "SupportsOspf",
                table: "RoutersTable",
                newName: "ProtocolSupport_SupportsOspf");

            migrationBuilder.RenameColumn(
                name: "SupportsNat",
                table: "RoutersTable",
                newName: "ProtocolSupport_SupportsNat");

            migrationBuilder.RenameColumn(
                name: "SupportsIpsec",
                table: "RoutersTable",
                newName: "ProtocolSupport_SupportsIpsec");

            migrationBuilder.RenameColumn(
                name: "MaxWirelessSpeed",
                table: "RoutersTable",
                newName: "WifiOptions_MaxWirelessSpeed");

            migrationBuilder.RenameColumn(
                name: "HasWiFi",
                table: "RoutersTable",
                newName: "WifiOptions_HasWiFi");

            migrationBuilder.RenameColumn(
                name: "FrequencyBands",
                table: "RoutersTable",
                newName: "WifiOptions_FrequencyBands");

            migrationBuilder.RenameColumn(
                name: "AntennaGain",
                table: "RoutersTable",
                newName: "WifiOptions_AntennaGain");

            migrationBuilder.RenameColumn(
                name: "AntennaCount",
                table: "RoutersTable",
                newName: "WifiOptions_AntennaCount");

            migrationBuilder.RenameColumn(
                name: "ModelName",
                table: "RoutersTable",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Model",
                table: "CommutatorsTable",
                newName: "Name");

            migrationBuilder.AlterColumn<string>(
                name: "WifiOptions_WiFiStandard",
                table: "RoutersTable",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "ProtocolSupport_SupportsVrrp",
                table: "RoutersTable",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "ProtocolSupport_SupportsOspf",
                table: "RoutersTable",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "ProtocolSupport_SupportsNat",
                table: "RoutersTable",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "ProtocolSupport_SupportsIpsec",
                table: "RoutersTable",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<double>(
                name: "WifiOptions_MaxWirelessSpeed",
                table: "RoutersTable",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<bool>(
                name: "WifiOptions_HasWiFi",
                table: "RoutersTable",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "WifiOptions_FrequencyBands",
                table: "RoutersTable",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<double>(
                name: "WifiOptions_AntennaGain",
                table: "RoutersTable",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<int>(
                name: "WifiOptions_AntennaCount",
                table: "RoutersTable",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "PerformanceSpecs_FlashMb",
                table: "RoutersTable",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PerformanceSpecs_RamMb",
                table: "RoutersTable",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PerformanceSpecs_RoutingThroughputGbps",
                table: "RoutersTable",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PerformanceSpecs_FlashMb",
                table: "RoutersTable");

            migrationBuilder.DropColumn(
                name: "PerformanceSpecs_RamMb",
                table: "RoutersTable");

            migrationBuilder.DropColumn(
                name: "PerformanceSpecs_RoutingThroughputGbps",
                table: "RoutersTable");

            migrationBuilder.RenameColumn(
                name: "WifiOptions_WiFiStandard",
                table: "RoutersTable",
                newName: "WiFiStandard");

            migrationBuilder.RenameColumn(
                name: "WifiOptions_MaxWirelessSpeed",
                table: "RoutersTable",
                newName: "MaxWirelessSpeed");

            migrationBuilder.RenameColumn(
                name: "WifiOptions_HasWiFi",
                table: "RoutersTable",
                newName: "HasWiFi");

            migrationBuilder.RenameColumn(
                name: "WifiOptions_FrequencyBands",
                table: "RoutersTable",
                newName: "FrequencyBands");

            migrationBuilder.RenameColumn(
                name: "WifiOptions_AntennaGain",
                table: "RoutersTable",
                newName: "AntennaGain");

            migrationBuilder.RenameColumn(
                name: "WifiOptions_AntennaCount",
                table: "RoutersTable",
                newName: "AntennaCount");

            migrationBuilder.RenameColumn(
                name: "ProtocolSupport_SupportsVrrp",
                table: "RoutersTable",
                newName: "SupportsVrrp");

            migrationBuilder.RenameColumn(
                name: "ProtocolSupport_SupportsOspf",
                table: "RoutersTable",
                newName: "SupportsOspf");

            migrationBuilder.RenameColumn(
                name: "ProtocolSupport_SupportsNat",
                table: "RoutersTable",
                newName: "SupportsNat");

            migrationBuilder.RenameColumn(
                name: "ProtocolSupport_SupportsIpsec",
                table: "RoutersTable",
                newName: "SupportsIpsec");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "RoutersTable",
                newName: "ModelName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "CommutatorsTable",
                newName: "Model");

            migrationBuilder.AlterColumn<string>(
                name: "WiFiStandard",
                table: "RoutersTable",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "MaxWirelessSpeed",
                table: "RoutersTable",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "HasWiFi",
                table: "RoutersTable",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FrequencyBands",
                table: "RoutersTable",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "AntennaGain",
                table: "RoutersTable",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AntennaCount",
                table: "RoutersTable",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "SupportsVrrp",
                table: "RoutersTable",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "SupportsOspf",
                table: "RoutersTable",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "SupportsNat",
                table: "RoutersTable",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "SupportsIpsec",
                table: "RoutersTable",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FirewallThroughputGbps",
                table: "RoutersTable",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "MaxSessions",
                table: "RoutersTable",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PowerType",
                table: "RoutersTable",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "SupportsBgp",
                table: "RoutersTable",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SupportsMpls",
                table: "RoutersTable",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "VpnThroughputMbps",
                table: "RoutersTable",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
