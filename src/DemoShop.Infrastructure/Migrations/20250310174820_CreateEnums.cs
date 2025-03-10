using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:order_status.order_status", "created,completed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
