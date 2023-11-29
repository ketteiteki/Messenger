using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Messenger.Services.Migrations
{
    public partial class AddUserSessionEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: new Guid("0a0abd9e-e759-4ec3-ad17-5d3c5f1b2e51"));

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: new Guid("10b80560-3742-4564-b8b0-42c98af17bfc"));

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: new Guid("16028929-a50c-4ac0-b8c3-16e112da134b"));

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: new Guid("af60db06-0111-4bd6-94bf-7ce6c7309acd"));

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: new Guid("eae466b7-b0ed-480b-93c0-d19a54847714"));

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: new Guid("619186ff-6e99-4b20-94ad-da8595f30e78"));

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Value = table.Column<byte[]>(type: "bytea", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "ChatId", "DateOfCreate", "IsEdit", "OwnerId", "ReplyToMessageId", "Text" },
                values: new object[,]
                {
                    { new Guid("01348a5c-4807-43f0-874b-fb39f0575f68"), new Guid("f69acb05-018c-4626-9e70-46fbb5dfde6f"), new DateTime(2023, 5, 28, 6, 32, 7, 789, DateTimeKind.Utc).AddTicks(6318), false, new Guid("5aef3c7f-8040-4a99-9a3d-388695e55763"), null, "привет, как дела?" },
                    { new Guid("4cff8d01-0cd3-40bd-81d1-7039bf99a4ac"), new Guid("2975dbfe-bc05-4962-ba85-e4d1b4e8f7a8"), new DateTime(2023, 5, 28, 6, 32, 7, 789, DateTimeKind.Utc).AddTicks(3960), false, new Guid("a85825ba-f99b-4177-a858-96384303ea14"), null, "привет, какие книжки почитать?" },
                    { new Guid("7d052a40-df26-457e-9b4c-2a512b473aa3"), new Guid("f69acb05-018c-4626-9e70-46fbb5dfde6f"), new DateTime(2023, 5, 28, 6, 32, 7, 789, DateTimeKind.Utc).AddTicks(5911), false, new Guid("5aef3c7f-8040-4a99-9a3d-388695e55763"), null, "привет" },
                    { new Guid("bb4ab11a-96d4-4504-af65-30720ab90d47"), new Guid("2975dbfe-bc05-4962-ba85-e4d1b4e8f7a8"), new DateTime(2023, 5, 28, 6, 32, 7, 789, DateTimeKind.Utc).AddTicks(5458), false, new Guid("5aef3c7f-8040-4a99-9a3d-388695e55763"), null, "ага" }
                });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "ChatId", "DateOfCreate", "IsEdit", "OwnerId", "ReplyToMessageId", "Text" },
                values: new object[] { new Guid("f4451ec9-f2c6-4c92-9b4a-1c3caadfc8a4"), new Guid("2975dbfe-bc05-4962-ba85-e4d1b4e8f7a8"), new DateTime(2023, 5, 28, 6, 32, 7, 789, DateTimeKind.Utc).AddTicks(4593), false, new Guid("5aef3c7f-8040-4a99-9a3d-388695e55763"), new Guid("4cff8d01-0cd3-40bd-81d1-7039bf99a4ac"), "Книги в айтишке это как предметы в школе, созданы что б отбить у тебя желание учиться..." });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "ChatId", "DateOfCreate", "IsEdit", "OwnerId", "ReplyToMessageId", "Text" },
                values: new object[] { new Guid("eb52fc3b-4e5d-4a81-9c72-43e96a1929d8"), new Guid("2975dbfe-bc05-4962-ba85-e4d1b4e8f7a8"), new DateTime(2023, 5, 28, 6, 32, 7, 789, DateTimeKind.Utc).AddTicks(5043), false, new Guid("ee677bde-c6e6-40b3-8294-5fb5e913202a"), new Guid("f4451ec9-f2c6-4c92-9b4a-1c3caadfc8a4"), "ладно" });

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId",
                table: "UserSessions",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: new Guid("01348a5c-4807-43f0-874b-fb39f0575f68"));

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: new Guid("7d052a40-df26-457e-9b4c-2a512b473aa3"));

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: new Guid("bb4ab11a-96d4-4504-af65-30720ab90d47"));

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: new Guid("eb52fc3b-4e5d-4a81-9c72-43e96a1929d8"));

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: new Guid("f4451ec9-f2c6-4c92-9b4a-1c3caadfc8a4"));

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: new Guid("4cff8d01-0cd3-40bd-81d1-7039bf99a4ac"));

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessToken = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Ip = table.Column<string>(type: "text", nullable: false),
                    RefreshToken = table.Column<Guid>(type: "uuid", nullable: false),
                    UserAgent = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "ChatId", "DateOfCreate", "IsEdit", "OwnerId", "ReplyToMessageId", "Text" },
                values: new object[,]
                {
                    { new Guid("0a0abd9e-e759-4ec3-ad17-5d3c5f1b2e51"), new Guid("f69acb05-018c-4626-9e70-46fbb5dfde6f"), new DateTime(2023, 3, 28, 19, 16, 7, 864, DateTimeKind.Utc).AddTicks(2375), false, new Guid("5aef3c7f-8040-4a99-9a3d-388695e55763"), null, "привет, как дела?" },
                    { new Guid("10b80560-3742-4564-b8b0-42c98af17bfc"), new Guid("f69acb05-018c-4626-9e70-46fbb5dfde6f"), new DateTime(2023, 3, 28, 19, 16, 7, 864, DateTimeKind.Utc).AddTicks(1788), false, new Guid("5aef3c7f-8040-4a99-9a3d-388695e55763"), null, "привет" },
                    { new Guid("619186ff-6e99-4b20-94ad-da8595f30e78"), new Guid("2975dbfe-bc05-4962-ba85-e4d1b4e8f7a8"), new DateTime(2023, 3, 28, 19, 16, 7, 863, DateTimeKind.Utc).AddTicks(2954), false, new Guid("a85825ba-f99b-4177-a858-96384303ea14"), null, "привет, какие книжки почитать?" },
                    { new Guid("af60db06-0111-4bd6-94bf-7ce6c7309acd"), new Guid("2975dbfe-bc05-4962-ba85-e4d1b4e8f7a8"), new DateTime(2023, 3, 28, 19, 16, 7, 864, DateTimeKind.Utc).AddTicks(1114), false, new Guid("5aef3c7f-8040-4a99-9a3d-388695e55763"), null, "ага" }
                });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "ChatId", "DateOfCreate", "IsEdit", "OwnerId", "ReplyToMessageId", "Text" },
                values: new object[] { new Guid("eae466b7-b0ed-480b-93c0-d19a54847714"), new Guid("2975dbfe-bc05-4962-ba85-e4d1b4e8f7a8"), new DateTime(2023, 3, 28, 19, 16, 7, 863, DateTimeKind.Utc).AddTicks(3908), false, new Guid("5aef3c7f-8040-4a99-9a3d-388695e55763"), new Guid("619186ff-6e99-4b20-94ad-da8595f30e78"), "Книги в айтишке это как предметы в школе, созданы что б отбить у тебя желание учиться..." });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "ChatId", "DateOfCreate", "IsEdit", "OwnerId", "ReplyToMessageId", "Text" },
                values: new object[] { new Guid("16028929-a50c-4ac0-b8c3-16e112da134b"), new Guid("2975dbfe-bc05-4962-ba85-e4d1b4e8f7a8"), new DateTime(2023, 3, 28, 19, 16, 7, 864, DateTimeKind.Utc).AddTicks(449), false, new Guid("ee677bde-c6e6-40b3-8294-5fb5e913202a"), new Guid("eae466b7-b0ed-480b-93c0-d19a54847714"), "ладно" });

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserId",
                table: "Sessions",
                column: "UserId");
        }
    }
}
