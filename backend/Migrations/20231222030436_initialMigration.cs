using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class initialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "room",
                columns: table => new
                {
                    code = table.Column<string>(type: "text", nullable: false),
                    word = table.Column<string>(type: "text", nullable: true),
                    host = table.Column<string>(type: "text", nullable: true),
                    current_drawer = table.Column<string>(type: "text", nullable: true),
                    game_started = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    name = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    connection_id = table.Column<string>(type: "text", nullable: true),
                    score = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    room_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_room_room_id",
                        column: x => x.room_id,
                        principalTable: "room",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_name_room_id",
                table: "user",
                columns: new[] { "name", "room_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_room_id",
                table: "user",
                column: "room_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "room");
        }
    }
}
