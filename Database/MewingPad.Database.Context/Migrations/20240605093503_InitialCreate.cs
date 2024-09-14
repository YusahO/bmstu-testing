using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MewingPad.Database.Context.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "varchar(64)", nullable: false),
                    password = table.Column<string>(type: "varchar(128)", nullable: false),
                    email = table.Column<string>(type: "varchar(320)", nullable: false),
                    role = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Audiotracks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "varchar(64)", nullable: false),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    filepath = table.Column<string>(type: "text", nullable: false),
                    mean_score = table.Column<float>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audiotracks", x => x.id);
                    table.ForeignKey(
                        name: "FK_Audiotracks_Users_author_id",
                        column: x => x.author_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Playlists",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "varchar(64)", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playlists", x => x.id);
                    table.ForeignKey(
                        name: "FK_Playlists_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "varchar(64)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.id);
                    table.ForeignKey(
                        name: "FK_Tags_Users_author_id",
                        column: x => x.author_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Commentaries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    audiotrack_id = table.Column<Guid>(type: "uuid", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commentaries", x => x.id);
                    table.ForeignKey(
                        name: "FK_Commentaries_Audiotracks_audiotrack_id",
                        column: x => x.audiotrack_id,
                        principalTable: "Audiotracks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Commentaries_Users_author_id",
                        column: x => x.author_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    audiotrack_id = table.Column<Guid>(type: "uuid", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.id);
                    table.ForeignKey(
                        name: "FK_Reports_Audiotracks_audiotrack_id",
                        column: x => x.audiotrack_id,
                        principalTable: "Audiotracks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reports_Users_author_id",
                        column: x => x.author_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Scores",
                columns: table => new
                {
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    audiotrack_id = table.Column<Guid>(type: "uuid", nullable: false),
                    value = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scores", x => new { x.author_id, x.audiotrack_id });
                    table.ForeignKey(
                        name: "FK_Scores_Audiotracks_audiotrack_id",
                        column: x => x.audiotrack_id,
                        principalTable: "Audiotracks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Scores_Users_author_id",
                        column: x => x.author_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaylistsAudiotracks",
                columns: table => new
                {
                    playlist_id = table.Column<Guid>(type: "uuid", nullable: false),
                    audiotrack_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistsAudiotracks", x => new { x.audiotrack_id, x.playlist_id });
                    table.ForeignKey(
                        name: "FK_PlaylistsAudiotracks_Audiotracks_audiotrack_id",
                        column: x => x.audiotrack_id,
                        principalTable: "Audiotracks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaylistsAudiotracks_Playlists_playlist_id",
                        column: x => x.playlist_id,
                        principalTable: "Playlists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersFavourites",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    favourite_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersFavourites", x => new { x.user_id, x.favourite_id });
                    table.ForeignKey(
                        name: "FK_UsersFavourites_Playlists_favourite_id",
                        column: x => x.favourite_id,
                        principalTable: "Playlists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersFavourites_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TagsAudiotracks",
                columns: table => new
                {
                    tag_id = table.Column<Guid>(type: "uuid", nullable: false),
                    audiotrack_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagsAudiotracks", x => new { x.audiotrack_id, x.tag_id });
                    table.ForeignKey(
                        name: "FK_TagsAudiotracks_Audiotracks_audiotrack_id",
                        column: x => x.audiotrack_id,
                        principalTable: "Audiotracks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagsAudiotracks_Tags_tag_id",
                        column: x => x.tag_id,
                        principalTable: "Tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Audiotracks_author_id",
                table: "Audiotracks",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_Commentaries_audiotrack_id",
                table: "Commentaries",
                column: "audiotrack_id");

            migrationBuilder.CreateIndex(
                name: "IX_Commentaries_author_id",
                table: "Commentaries",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_user_id",
                table: "Playlists",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistsAudiotracks_playlist_id",
                table: "PlaylistsAudiotracks",
                column: "playlist_id");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_audiotrack_id",
                table: "Reports",
                column: "audiotrack_id");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_author_id",
                table: "Reports",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_audiotrack_id",
                table: "Scores",
                column: "audiotrack_id");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_author_id",
                table: "Tags",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_TagsAudiotracks_tag_id",
                table: "TagsAudiotracks",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "IX_UsersFavourites_favourite_id",
                table: "UsersFavourites",
                column: "favourite_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Commentaries");

            migrationBuilder.DropTable(
                name: "PlaylistsAudiotracks");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "Scores");

            migrationBuilder.DropTable(
                name: "TagsAudiotracks");

            migrationBuilder.DropTable(
                name: "UsersFavourites");

            migrationBuilder.DropTable(
                name: "Audiotracks");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Playlists");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
