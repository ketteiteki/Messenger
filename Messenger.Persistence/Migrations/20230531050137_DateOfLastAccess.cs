using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Messenger.Services.Migrations
{
    public partial class DateOfLastAccess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateOfLastAccess",
                table: "UserSessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "ChatId", "DateOfCreate", "IsEdit", "OwnerId", "ReplyToMessageId", "Text" },
                values: new object[,]
                {
                    { new Guid("1bbb3da1-da30-499e-a866-190d22ab4590"), new Guid("f69acb05-018c-4626-9e70-46fbb5dfde6f"), new DateTime(2023, 5, 31, 5, 1, 36, 402, DateTimeKind.Utc).AddTicks(1967), false, new Guid("5aef3c7f-8040-4a99-9a3d-388695e55763"), null, "привет, как дела?" },
                    { new Guid("8f2e54bc-4eec-47d6-a2cb-44fb435a77c7"), new Guid("2975dbfe-bc05-4962-ba85-e4d1b4e8f7a8"), new DateTime(2023, 5, 31, 5, 1, 36, 401, DateTimeKind.Utc).AddTicks(9800), false, new Guid("a85825ba-f99b-4177-a858-96384303ea14"), null, "привет, какие книжки почитать?" },
                    { new Guid("ef0be2a3-74c0-4e82-b56c-562b15440701"), new Guid("f69acb05-018c-4626-9e70-46fbb5dfde6f"), new DateTime(2023, 5, 31, 5, 1, 36, 402, DateTimeKind.Utc).AddTicks(1586), false, new Guid("5aef3c7f-8040-4a99-9a3d-388695e55763"), null, "привет" },
                    { new Guid("fe3c2f65-41fc-42a4-903b-52fa3ea1076b"), new Guid("2975dbfe-bc05-4962-ba85-e4d1b4e8f7a8"), new DateTime(2023, 5, 31, 5, 1, 36, 402, DateTimeKind.Utc).AddTicks(1153), false, new Guid("5aef3c7f-8040-4a99-9a3d-388695e55763"), null, "ага" }
                });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "ChatId", "DateOfCreate", "IsEdit", "OwnerId", "ReplyToMessageId", "Text" },
                values: new object[] { new Guid("ef65d839-fbe4-4775-b3d2-229ce23c3324"), new Guid("2975dbfe-bc05-4962-ba85-e4d1b4e8f7a8"), new DateTime(2023, 5, 31, 5, 1, 36, 402, DateTimeKind.Utc).AddTicks(368), false, new Guid("5aef3c7f-8040-4a99-9a3d-388695e55763"), new Guid("8f2e54bc-4eec-47d6-a2cb-44fb435a77c7"), "Книги в айтишке это как предметы в школе, созданы что б отбить у тебя желание учиться..." });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "ChatId", "DateOfCreate", "IsEdit", "OwnerId", "ReplyToMessageId", "Text" },
                values: new object[] { new Guid("dd5bfabd-9982-4995-ba70-20c55643a5ab"), new Guid("2975dbfe-bc05-4962-ba85-e4d1b4e8f7a8"), new DateTime(2023, 5, 31, 5, 1, 36, 402, DateTimeKind.Utc).AddTicks(765), false, new Guid("ee677bde-c6e6-40b3-8294-5fb5e913202a"), new Guid("ef65d839-fbe4-4775-b3d2-229ce23c3324"), "ладно" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: new Guid("1bbb3da1-da30-499e-a866-190d22ab4590"));

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: new Guid("dd5bfabd-9982-4995-ba70-20c55643a5ab"));

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: new Guid("ef0be2a3-74c0-4e82-b56c-562b15440701"));

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: new Guid("fe3c2f65-41fc-42a4-903b-52fa3ea1076b"));

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: new Guid("ef65d839-fbe4-4775-b3d2-229ce23c3324"));

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: new Guid("8f2e54bc-4eec-47d6-a2cb-44fb435a77c7"));

            migrationBuilder.DropColumn(
                name: "DateOfLastAccess",
                table: "UserSessions");

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
        }
    }
}
