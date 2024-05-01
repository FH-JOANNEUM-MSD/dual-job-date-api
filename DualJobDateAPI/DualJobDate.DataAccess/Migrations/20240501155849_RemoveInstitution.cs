using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DualJobDate.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveInstitution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Institutions_InstitutionId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Institutions_InstitutionId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_AcademicPrograms_AcademicProgramId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Institutions_InstitutionId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Companies_InstitutionId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_Name_InstitutionId_AcademicProgramId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Activities_InstitutionId",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "Activities");

            migrationBuilder.AlterColumn<int>(
                name: "InstitutionId",
                table: "Users",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AcademicProgramId",
                table: "Users",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Name_AcademicProgramId",
                table: "Companies",
                columns: new[] { "Name", "AcademicProgramId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_AcademicPrograms_AcademicProgramId",
                table: "Users",
                column: "AcademicProgramId",
                principalTable: "AcademicPrograms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Institutions_InstitutionId",
                table: "Users",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_AcademicPrograms_AcademicProgramId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Institutions_InstitutionId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Companies_Name_AcademicProgramId",
                table: "Companies");

            migrationBuilder.AlterColumn<int>(
                name: "InstitutionId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AcademicProgramId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstitutionId",
                table: "Companies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InstitutionId",
                table: "Activities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_InstitutionId",
                table: "Companies",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Name_InstitutionId_AcademicProgramId",
                table: "Companies",
                columns: new[] { "Name", "InstitutionId", "AcademicProgramId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Activities_InstitutionId",
                table: "Activities",
                column: "InstitutionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Institutions_InstitutionId",
                table: "Activities",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Institutions_InstitutionId",
                table: "Companies",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_AcademicPrograms_AcademicProgramId",
                table: "Users",
                column: "AcademicProgramId",
                principalTable: "AcademicPrograms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Institutions_InstitutionId",
                table: "Users",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
