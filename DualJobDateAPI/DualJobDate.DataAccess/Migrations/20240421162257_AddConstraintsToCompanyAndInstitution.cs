using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DualJobDate.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddConstraintsToCompanyAndInstitution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "KeyName",
                table: "Institutions",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Companies",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.CreateIndex(
                name: "IX_Institutions_KeyName",
                table: "Institutions",
                column: "KeyName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Name_InstitutionId_AcademicProgramId",
                table: "Companies",
                columns: new[] { "Name", "InstitutionId", "AcademicProgramId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Institutions_KeyName",
                table: "Institutions");

            migrationBuilder.DropIndex(
                name: "IX_Companies_Name_InstitutionId_AcademicProgramId",
                table: "Companies");

            migrationBuilder.AlterColumn<string>(
                name: "KeyName",
                table: "Institutions",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Companies",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");
        }
    }
}
