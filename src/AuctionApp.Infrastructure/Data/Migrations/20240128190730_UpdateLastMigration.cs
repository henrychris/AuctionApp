using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLastMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomId",
                schema: "AuctionDb",
                table: "Bids");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoomId",
                schema: "AuctionDb",
                table: "Bids",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
