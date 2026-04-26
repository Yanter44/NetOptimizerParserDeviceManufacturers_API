using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetOptimizerParserApi.Migrations
{
    /// <inheritdoc />
    public partial class RemovedSomeshitInQosInSwitch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProtocolSupport_QosQueuesPerPort",
                table: "CommutatorsTable");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProtocolSupport_QosQueuesPerPort",
                table: "CommutatorsTable",
                type: "integer",
                nullable: true);
        }
    }
}
