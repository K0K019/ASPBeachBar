using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_BeachBar.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260511113000_AddAboutNavigationMenuItem")]
    public partial class AddAboutNavigationMenuItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1
                    FROM [NavigationMenuItems]
                    WHERE [Controller] = N'Home' AND [Action] = N'About'
                )
                BEGIN
                    INSERT INTO [NavigationMenuItems] ([Text], [Controller], [Action], [Area], [RequiredRole], [SortOrder], [IsActive])
                    VALUES (N'За нас', N'Home', N'About', N'', NULL, 5, CAST(1 AS bit));
                END
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM [NavigationMenuItems]
                WHERE [Controller] = N'Home' AND [Action] = N'About';
                """);
        }
    }
}
