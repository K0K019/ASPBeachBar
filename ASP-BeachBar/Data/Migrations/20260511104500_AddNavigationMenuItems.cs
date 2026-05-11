using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_BeachBar.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260511104500_AddNavigationMenuItems")]
    public partial class AddNavigationMenuItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NavigationMenuItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Controller = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Area = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    RequiredRole = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NavigationMenuItems", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "NavigationMenuItems",
                columns: new[] { "Id", "Text", "Controller", "Action", "Area", "RequiredRole", "SortOrder", "IsActive" },
                columnTypes: new[] { "int", "nvarchar(80)", "nvarchar(80)", "nvarchar(80)", "nvarchar(80)", "nvarchar(80)", "int", "bit" },
                values: new object[,]
                {
                    { 1, "Начало", "Home", "Index", "", null, 0, true },
                    { 2, "Храна", "Products", "Index", "", null, 10, true },
                    { 3, "Напитки", "Drinks", "Index", "", null, 20, true },
                    { 4, "Събития", "Events", "Index", "", null, 30, true },
                    { 5, "Резервация", "Reservations", "Create", "", null, 40, true },
                    { 6, "Категории", "Categories", "Index", "", "Admin", 90, true },
                    { 7, "Всички резервации", "Reservations", "Index", "", "Admin", 100, true }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NavigationMenuItems");
        }
    }
}
