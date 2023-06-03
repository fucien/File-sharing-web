using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web_ver_2.Migrations
{
    /// <inheritdoc />
    public partial class ini : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

 

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Email = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Email);
                });
            migrationBuilder.CreateTable(
	            name: "File",
	            columns: table => new
	            {
		            URL = table.Column<string>(type: "nvarchar(450)", nullable: false),
		            Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
		            Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
		            Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
		            Email = table.Column<string>(type: "nvarchar(255)", nullable: false),
		            UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue:DateTime.Now)
	            },
	            constraints: table =>
	            {
		            table.PrimaryKey("PK_File", x => x.URL);
                    table.ForeignKey(
                        name: "FK_File_User_Email",
                        column: x => x.Email,
                        principalTable: "User",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Cascade);
	            });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

	        migrationBuilder.DropTable(
                name: "File");

            migrationBuilder.DropTable(
                name: "User");

        }
    }
}
