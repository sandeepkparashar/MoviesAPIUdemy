using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MoviesAPI.Migrations
{
    /// <inheritdoc />
    public partial class seeddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Picture",
                table: "People",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "UserId", "Name" },
                values: new object[,]
                {
                    { 4, "Adventure" },
                    { 5, "Animation" },
                    { 6, "Drama" },
                    { 7, "Romance" }
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "UserId", "InTheatres", "Poster", "ReleaseDate", "Summary", "Name" },
                values: new object[,]
                {
                    { 4, false, null, new DateTime(2019, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Avengers:Endgame" },
                    { 5, false, null, new DateTime(2018, 4, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Avengers:Infinity War" },
                    { 6, false, null, new DateTime(2020, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Sonic The Hedgehog" },
                    { 7, false, null, new DateTime(2020, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Emma" },
                    { 8, false, null, new DateTime(2019, 9, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Greed" }
                });

            migrationBuilder.InsertData(
                table: "People",
                columns: new[] { "UserId", "Biography", "DateOfBirth", "Name", "Picture" },
                values: new object[,]
                {
                    { 9, null, new DateTime(1962, 1, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jim Carrey", null },
                    { 10, null, new DateTime(1965, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Robert Dawney Jr.", null },
                    { 11, null, new DateTime(1981, 6, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chris Evans", null }
                });

            migrationBuilder.InsertData(
                table: "MoviesActors",
                columns: new[] { "MovieId", "PersonId", "Character", "Order" },
                values: new object[,]
                {
                    { 4, 10, "Tony Starc", 1 },
                    { 4, 11, "Steve Rogers", 2 },
                    { 5, 10, "Tony Starc", 1 },
                    { 5, 11, "Steve Rogers", 2 },
                    { 6, 9, "Dr. Ivo Robotnik", 1 }
                });

            migrationBuilder.InsertData(
                table: "MoviesGenres",
                columns: new[] { "GenreId", "MovieId" },
                values: new object[,]
                {
                    { 4, 4 },
                    { 4, 5 },
                    { 6, 4 },
                    { 6, 5 },
                    { 6, 6 },
                    { 6, 7 },
                    { 6, 8 },
                    { 7, 7 },
                    { 7, 8 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "UserId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "MoviesActors",
                keyColumns: new[] { "MovieId", "PersonId" },
                keyValues: new object[] { 4, 10 });

            migrationBuilder.DeleteData(
                table: "MoviesActors",
                keyColumns: new[] { "MovieId", "PersonId" },
                keyValues: new object[] { 4, 11 });

            migrationBuilder.DeleteData(
                table: "MoviesActors",
                keyColumns: new[] { "MovieId", "PersonId" },
                keyValues: new object[] { 5, 10 });

            migrationBuilder.DeleteData(
                table: "MoviesActors",
                keyColumns: new[] { "MovieId", "PersonId" },
                keyValues: new object[] { 5, 11 });

            migrationBuilder.DeleteData(
                table: "MoviesActors",
                keyColumns: new[] { "MovieId", "PersonId" },
                keyValues: new object[] { 6, 9 });

            migrationBuilder.DeleteData(
                table: "MoviesGenres",
                keyColumns: new[] { "GenreId", "MovieId" },
                keyValues: new object[] { 4, 4 });

            migrationBuilder.DeleteData(
                table: "MoviesGenres",
                keyColumns: new[] { "GenreId", "MovieId" },
                keyValues: new object[] { 4, 5 });

            migrationBuilder.DeleteData(
                table: "MoviesGenres",
                keyColumns: new[] { "GenreId", "MovieId" },
                keyValues: new object[] { 6, 4 });

            migrationBuilder.DeleteData(
                table: "MoviesGenres",
                keyColumns: new[] { "GenreId", "MovieId" },
                keyValues: new object[] { 6, 5 });

            migrationBuilder.DeleteData(
                table: "MoviesGenres",
                keyColumns: new[] { "GenreId", "MovieId" },
                keyValues: new object[] { 6, 6 });

            migrationBuilder.DeleteData(
                table: "MoviesGenres",
                keyColumns: new[] { "GenreId", "MovieId" },
                keyValues: new object[] { 6, 7 });

            migrationBuilder.DeleteData(
                table: "MoviesGenres",
                keyColumns: new[] { "GenreId", "MovieId" },
                keyValues: new object[] { 6, 8 });

            migrationBuilder.DeleteData(
                table: "MoviesGenres",
                keyColumns: new[] { "GenreId", "MovieId" },
                keyValues: new object[] { 7, 7 });

            migrationBuilder.DeleteData(
                table: "MoviesGenres",
                keyColumns: new[] { "GenreId", "MovieId" },
                keyValues: new object[] { 7, 8 });

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "UserId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "UserId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "UserId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "UserId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "UserId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "UserId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "UserId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "UserId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "People",
                keyColumn: "UserId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "People",
                keyColumn: "UserId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "People",
                keyColumn: "UserId",
                keyValue: 11);

            migrationBuilder.AlterColumn<string>(
                name: "Picture",
                table: "People",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
