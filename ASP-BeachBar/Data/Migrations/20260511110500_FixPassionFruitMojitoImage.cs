using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_BeachBar.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260511110500_FixPassionFruitMojitoImage")]
    public partial class FixPassionFruitMojitoImage : Migration
    {
        private const string CocktailImageUrl = "https://images.unsplash.com/photo-1551538827-9c037cb4f32a?auto=format&fit=crop&w=900&q=80";
        private const string BurgerImageUrl = "https://images.unsplash.com/photo-1551782450-a2132b4ba21d?auto=format&fit=crop&w=900&q=80";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"""
                UPDATE [Drinks]
                SET [ImageUrl] = N'{CocktailImageUrl}'
                WHERE [Name] = N'Passion Fruit Mojito';
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"""
                UPDATE [Drinks]
                SET [ImageUrl] = N'{BurgerImageUrl}'
                WHERE [Name] = N'Passion Fruit Mojito'
                    AND [ImageUrl] = N'{CocktailImageUrl}';
                """);
        }
    }
}
