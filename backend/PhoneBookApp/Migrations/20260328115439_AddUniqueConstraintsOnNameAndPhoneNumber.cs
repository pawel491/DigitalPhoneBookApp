using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhoneBookApp.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintsOnNameAndPhoneNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PhoneContacts_Name",
                table: "PhoneContacts",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhoneContacts_PhoneNumber",
                table: "PhoneContacts",
                column: "PhoneNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PhoneContacts_Name",
                table: "PhoneContacts");

            migrationBuilder.DropIndex(
                name: "IX_PhoneContacts_PhoneNumber",
                table: "PhoneContacts");
        }
    }
}
