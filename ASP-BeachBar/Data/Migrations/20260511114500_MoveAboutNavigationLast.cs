using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_BeachBar.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260511114500_MoveAboutNavigationLast")]
    public partial class MoveAboutNavigationLast : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE [NavigationMenuItems]
                SET [SortOrder] = 110
                WHERE [Controller] = N'Home' AND [Action] = N'About';
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE [NavigationMenuItems]
                SET [SortOrder] = 5
                WHERE [Controller] = N'Home' AND [Action] = N'About';
                """);
        }
    }
}
