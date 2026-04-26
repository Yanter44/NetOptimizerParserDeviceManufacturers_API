using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetOptimizerParserApi.Migrations
{
    /// <inheritdoc />
    public partial class RemovedSupportQosInSwitchProtocols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProtocolSupport_SupportsQos",
                table: "CommutatorsTable");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ProtocolSupport_SupportsQos",
                table: "CommutatorsTable",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
