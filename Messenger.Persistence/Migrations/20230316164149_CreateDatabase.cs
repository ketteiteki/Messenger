using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Messenger.Services.Migrations
{
    public partial class CreateDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Nickname = table.Column<string>(type: "text", nullable: false),
                    Bio = table.Column<string>(type: "text", nullable: true),
                    AvatarLink = table.Column<string>(type: "text", nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    PasswordSalt = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessToken = table.Column<string>(type: "text", nullable: false),
                    RefreshToken = table.Column<Guid>(type: "uuid", nullable: false),
                    Ip = table.Column<string>(type: "text", nullable: false),
                    UserAgent = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    Link = table.Column<string>(type: "text", nullable: false),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BanUserByChats",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChatId = table.Column<Guid>(type: "uuid", nullable: false),
                    BanDateOfExpire = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BanUserByChats", x => new { x.ChatId, x.UserId });
                    table.ForeignKey(
                        name: "FK_BanUserByChats_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    AvatarLink = table.Column<string>(type: "text", nullable: true),
                    LastMessageId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chats_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ChatUsers",
                columns: table => new
                {
                    ChatId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CanSendMedia = table.Column<bool>(type: "boolean", nullable: false),
                    MuteDateOfExpire = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatUsers", x => new { x.ChatId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ChatUsers_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeletedDialogByUsers",
                columns: table => new
                {
                    ChatId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeletedDialogByUsers", x => new { x.ChatId, x.UserId });
                    table.ForeignKey(
                        name: "FK_DeletedDialogByUsers_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeletedDialogByUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    IsEdit = table.Column<bool>(type: "boolean", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReplyToMessageId = table.Column<Guid>(type: "uuid", nullable: true),
                    ChatId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOfCreate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_Messages_ReplyToMessageId",
                        column: x => x.ReplyToMessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Messages_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleUserByChats",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChatId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleTitle = table.Column<string>(type: "text", nullable: false),
                    RoleColor = table.Column<int>(type: "integer", nullable: false),
                    CanBanUser = table.Column<bool>(type: "boolean", nullable: false),
                    CanChangeChatData = table.Column<bool>(type: "boolean", nullable: false),
                    CanGivePermissionToUser = table.Column<bool>(type: "boolean", nullable: false),
                    CanAddAndRemoveUserToConversation = table.Column<bool>(type: "boolean", nullable: false),
                    IsOwner = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleUserByChats", x => new { x.ChatId, x.UserId });
                    table.ForeignKey(
                        name: "FK_RoleUserByChats_ChatUsers_ChatId_UserId",
                        columns: x => new { x.ChatId, x.UserId },
                        principalTable: "ChatUsers",
                        principalColumns: new[] { "ChatId", "UserId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleUserByChats_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeletedMessageByUsers",
                columns: table => new
                {
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeletedMessageByUsers", x => new { x.MessageId, x.UserId });
                    table.ForeignKey(
                        name: "FK_DeletedMessageByUsers_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeletedMessageByUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Chats",
                columns: new[] { "Id", "AvatarLink", "LastMessageId", "Name", "OwnerId", "Title", "Type" },
                values: new object[] { new Guid("f69acb05-018c-4626-9e70-46fbb5dfde6f"), null, null, null, null, null, 0 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvatarLink", "Bio", "DisplayName", "Nickname", "PasswordHash", "PasswordSalt" },
                values: new object[,]
                {
                    { new Guid("5aef3c7f-8040-4a99-9a3d-388695e55763"), null, "the best account", "kami no me", "kaminome", "gzF/n+F8YPd/IvNrALE/XtGhhoJhtRs+PP3eco6JYzB36pVy2TGyj/4+68GXGws4EiIjSAkPKutdJuj6tb0d7A==", "fh1cbqngj2gJnAoolbwi6e6tPVGwUnrLVsCX1l7UbD+Nxz72Y8F4ucWNaBa0kLopPAyFWHesvCfZX7OSlqG3ZVAjYTUIa+YCV3TXwgNnQARH0KptctnRHczMzlk5D0bmHra29Zc3rGkWpsxtVGhuayb/FIUGPG92Md0G8d6v2GI=" },
                    { new Guid("9f40e295-8b43-4329-b37e-d267deee6c4a"), null, "why alex?", "alex1", "alex1234", "gzF/n+F8YPd/IvNrALE/XtGhhoJhtRs+PP3eco6JYzB36pVy2TGyj/4+68GXGws4EiIjSAkPKutdJuj6tb0d7A==", "fh1cbqngj2gJnAoolbwi6e6tPVGwUnrLVsCX1l7UbD+Nxz72Y8F4ucWNaBa0kLopPAyFWHesvCfZX7OSlqG3ZVAjYTUIa+YCV3TXwgNnQARH0KptctnRHczMzlk5D0bmHra29Zc3rGkWpsxtVGhuayb/FIUGPG92Md0G8d6v2GI=" },
                    { new Guid("a85825ba-f99b-4177-a858-96384303ea14"), null, "I'm Bob", "bob1", "bob1234", "gzF/n+F8YPd/IvNrALE/XtGhhoJhtRs+PP3eco6JYzB36pVy2TGyj/4+68GXGws4EiIjSAkPKutdJuj6tb0d7A==", "fh1cbqngj2gJnAoolbwi6e6tPVGwUnrLVsCX1l7UbD+Nxz72Y8F4ucWNaBa0kLopPAyFWHesvCfZX7OSlqG3ZVAjYTUIa+YCV3TXwgNnQARH0KptctnRHczMzlk5D0bmHra29Zc3rGkWpsxtVGhuayb/FIUGPG92Md0G8d6v2GI=" },
                    { new Guid("ee677bde-c6e6-40b3-8294-5fb5e913202a"), null, "cool status", "alice1", "alice1234", "gzF/n+F8YPd/IvNrALE/XtGhhoJhtRs+PP3eco6JYzB36pVy2TGyj/4+68GXGws4EiIjSAkPKutdJuj6tb0d7A==", "fh1cbqngj2gJnAoolbwi6e6tPVGwUnrLVsCX1l7UbD+Nxz72Y8F4ucWNaBa0kLopPAyFWHesvCfZX7OSlqG3ZVAjYTUIa+YCV3TXwgNnQARH0KptctnRHczMzlk5D0bmHra29Zc3rGkWpsxtVGhuayb/FIUGPG92Md0G8d6v2GI=" }
                });

            migrationBuilder.InsertData(
                table: "ChatUsers",
                columns: new[] { "ChatId", "UserId", "CanSendMedia", "MuteDateOfExpire" },
                values: new object[,]
                {
                    { new Guid("f69acb05-018c-4626-9e70-46fbb5dfde6f"), new Guid("5aef3c7f-8040-4a99-9a3d-388695e55763"), true, null },
                    { new Guid("f69acb05-018c-4626-9e70-46fbb5dfde6f"), new Guid("ee677bde-c6e6-40b3-8294-5fb5e913202a"), true, null }
                });

            migrationBuilder.InsertData(
                table: "Chats",
                columns: new[] { "Id", "AvatarLink", "LastMessageId", "Name", "OwnerId", "Title", "Type" },
                values: new object[,]
                {
                    { new Guid("2975dbfe-bc05-4962-ba85-e4d1b4e8f7a8"), null, null, "DotNetRuChat", new Guid("5aef3c7f-8040-4a99-9a3d-388695e55763"), "DotNetRuChat", 1 },
                    { new Guid("2b56ee19-fe9c-4fab-884b-ff7d85a9f337"), null, null, "dotnettalks", new Guid("ee677bde-c6e6-40b3-8294-5fb5e913202a"), ".NET Talks", 1 }
                });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "ChatId", "DateOfCreate", "IsEdit", "OwnerId", "ReplyToMessageId", "Text" },
                values: new object[,]
                {
                    { new Guid("a1077a79-a662-4e44-8ece-c9ce64c5a2d0"), new Guid("f69acb05-018c-4626-9e70-46fbb5dfde6f"), new DateTime(2023, 3, 16, 16, 41, 48, 727, DateTimeKind.Utc).AddTicks(8026), false, new Guid("5aef3c7f-8040-4a99-9a3d-388695e55763"), null, "привет, как дела?" },
                    { new Guid("b1f3e620-7d31-46f0-ab07-791829539bf8"), new Guid("f69acb05-018c-4626-9e70-46fbb5dfde6f"), new DateTime(2023, 3, 16, 16, 41, 48, 727, DateTimeKind.Utc).AddTicks(7512), false, new Guid("5aef3c7f-8040-4a99-9a3d-388695e55763"), null, "привет" }
                });

            migrationBuilder.InsertData(
                table: "ChatUsers",
                columns: new[] { "ChatId", "UserId", "CanSendMedia", "MuteDateOfExpire" },
                values: new object[,]
                {
                    { new Guid("2975dbfe-bc05-4962-ba85-e4d1b4e8f7a8"), new Guid("5aef3c7f-8040-4a99-9a3d-388695e55763"), true, null },
                    { new Guid("2975dbfe-bc05-4962-ba85-e4d1b4e8f7a8"), new Guid("9f40e295-8b43-4329-b37e-d267deee6c4a"), true, null },
                    { new Guid("2975dbfe-bc05-4962-ba85-e4d1b4e8f7a8"), new Guid("a85825ba-f99b-4177-a858-96384303ea14"), true, null },
                    { new Guid("2975dbfe-bc05-4962-ba85-e4d1b4e8f7a8"), new Guid("ee677bde-c6e6-40b3-8294-5fb5e913202a"), true, null },
                    { new Guid("2b56ee19-fe9c-4fab-884b-ff7d85a9f337"), new Guid("9f40e295-8b43-4329-b37e-d267deee6c4a"), true, null },
                    { new Guid("2b56ee19-fe9c-4fab-884b-ff7d85a9f337"), new Guid("a85825ba-f99b-4177-a858-96384303ea14"), true, null },
                    { new Guid("2b56ee19-fe9c-4fab-884b-ff7d85a9f337"), new Guid("ee677bde-c6e6-40b3-8294-5fb5e913202a"), true, null }
                });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "ChatId", "DateOfCreate", "IsEdit", "OwnerId", "ReplyToMessageId", "Text" },
                values: new object[,]
                {
                    { new Guid("6f6d7947-488d-4e7c-be7e-bfbf04bfef6f"), new Guid("2975dbfe-bc05-4962-ba85-e4d1b4e8f7a8"), new DateTime(2023, 3, 16, 16, 41, 48, 727, DateTimeKind.Utc).AddTicks(4520), false, new Guid("a85825ba-f99b-4177-a858-96384303ea14"), null, "привет, какие книжки почитать?" },
                    { new Guid("ab52cc53-2afd-49df-b2c4-345851bcbee1"), new Guid("2975dbfe-bc05-4962-ba85-e4d1b4e8f7a8"), new DateTime(2023, 3, 16, 16, 41, 48, 727, DateTimeKind.Utc).AddTicks(6862), false, new Guid("5aef3c7f-8040-4a99-9a3d-388695e55763"), null, "ага" }
                });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "ChatId", "DateOfCreate", "IsEdit", "OwnerId", "ReplyToMessageId", "Text" },
                values: new object[] { new Guid("d4b826c1-5f1c-4ae6-ac2a-8ee7b5b75e8a"), new Guid("2975dbfe-bc05-4962-ba85-e4d1b4e8f7a8"), new DateTime(2023, 3, 16, 16, 41, 48, 727, DateTimeKind.Utc).AddTicks(5787), false, new Guid("5aef3c7f-8040-4a99-9a3d-388695e55763"), new Guid("6f6d7947-488d-4e7c-be7e-bfbf04bfef6f"), "Книги в айтишке это как предметы в школе, созданы что б отбить у тебя желание учиться..." });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "ChatId", "DateOfCreate", "IsEdit", "OwnerId", "ReplyToMessageId", "Text" },
                values: new object[] { new Guid("5e162143-381e-4b77-b4fd-f5f028844eb5"), new Guid("2975dbfe-bc05-4962-ba85-e4d1b4e8f7a8"), new DateTime(2023, 3, 16, 16, 41, 48, 727, DateTimeKind.Utc).AddTicks(6339), false, new Guid("ee677bde-c6e6-40b3-8294-5fb5e913202a"), new Guid("d4b826c1-5f1c-4ae6-ac2a-8ee7b5b75e8a"), "ладно" });

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_MessageId",
                table: "Attachments",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_BanUserByChats_UserId",
                table: "BanUserByChats",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_LastMessageId",
                table: "Chats",
                column: "LastMessageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chats_OwnerId",
                table: "Chats",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatUsers_UserId",
                table: "ChatUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DeletedDialogByUsers_UserId",
                table: "DeletedDialogByUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DeletedMessageByUsers_UserId",
                table: "DeletedMessageByUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId",
                table: "Messages",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_OwnerId",
                table: "Messages",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReplyToMessageId",
                table: "Messages",
                column: "ReplyToMessageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleUserByChats_UserId",
                table: "RoleUserByChats",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserId",
                table: "Sessions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Messages_MessageId",
                table: "Attachments",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BanUserByChats_Chats_ChatId",
                table: "BanUserByChats",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Messages_LastMessageId",
                table: "Chats",
                column: "LastMessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Messages_LastMessageId",
                table: "Chats");

            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropTable(
                name: "BanUserByChats");

            migrationBuilder.DropTable(
                name: "DeletedDialogByUsers");

            migrationBuilder.DropTable(
                name: "DeletedMessageByUsers");

            migrationBuilder.DropTable(
                name: "RoleUserByChats");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "ChatUsers");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
