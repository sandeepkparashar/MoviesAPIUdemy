using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviesAPI.Migrations
{
    /// <inheritdoc />
    public partial class AdminRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"Insert into [AspNetRoles] (UserId, [Name], [NormalizedName]) 
                    values ('60e32053-a3b5-4fdf-9e58-191e79137bf3','Admin','Admin');
                    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Delete from [AspNetRoles] where UserId='60e32053-a3b5-4fdf-9e58-191e79137bf3'");
        }
    }
}
