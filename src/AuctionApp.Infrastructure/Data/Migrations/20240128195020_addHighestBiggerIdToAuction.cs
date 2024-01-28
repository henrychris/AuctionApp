using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class addHighestBiggerIdToAuction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HighestBidderId",
                schema: "AuctionDb",
                table: "Auctions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HighestBidderId",
                schema: "AuctionDb",
                table: "Auctions");
        }
    }
}