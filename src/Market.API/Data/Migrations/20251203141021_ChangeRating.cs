using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Market.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Rating",
                table: "Users",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AddColumn<int>(
                name: "RatingsCount",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RatingsCount",
                table: "Users");

            migrationBuilder.AlterColumn<byte>(
                name: "Rating",
                table: "Users",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
