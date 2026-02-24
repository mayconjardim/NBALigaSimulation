using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NBALigaSimulation.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DraftLotteries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    FirstTeam = table.Column<string>(type: "text", nullable: true),
                    FirstTeamId = table.Column<int>(type: "integer", nullable: false),
                    SecondTeam = table.Column<string>(type: "text", nullable: true),
                    SecondTeamId = table.Column<int>(type: "integer", nullable: false),
                    ThirdTeam = table.Column<string>(type: "text", nullable: true),
                    ThirdTeamId = table.Column<int>(type: "integer", nullable: false),
                    FourthTeam = table.Column<string>(type: "text", nullable: true),
                    FourthTeamId = table.Column<int>(type: "integer", nullable: false),
                    FifthTeam = table.Column<string>(type: "text", nullable: true),
                    FifthTeamId = table.Column<int>(type: "integer", nullable: false),
                    SixthTeam = table.Column<string>(type: "text", nullable: true),
                    SixthTeamId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftLotteries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameNews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameId = table.Column<int>(type: "integer", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Winner = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameNews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    LotteryCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    DraftCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    TcCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeadlineCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    RegularCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    FirstRoundCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    SecondRoundCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    ThirdRoundCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    FourthRoundCompleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Region = table.Column<string>(type: "text", nullable: true),
                    Abrv = table.Column<string>(type: "text", nullable: true),
                    Conference = table.Column<string>(type: "text", nullable: true),
                    IsHuman = table.Column<bool>(type: "boolean", nullable: false),
                    Championships = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SeasonId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Week = table.Column<string>(type: "text", nullable: true),
                    HomeTeamId = table.Column<int>(type: "integer", nullable: false),
                    AwayTeamId = table.Column<int>(type: "integer", nullable: false),
                    Happened = table.Column<bool>(type: "boolean", nullable: false),
                    GameDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Games_Teams_AwayTeamId",
                        column: x => x.AwayTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Games_Teams_HomeTeamId",
                        column: x => x.HomeTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Pos = table.Column<string>(type: "text", nullable: true),
                    Born_Year = table.Column<int>(type: "integer", nullable: true),
                    Born_Loc = table.Column<string>(type: "text", nullable: true),
                    Draft_Year = table.Column<int>(type: "integer", nullable: true),
                    Draft_Pick = table.Column<int>(type: "integer", nullable: true),
                    Draft_Round = table.Column<int>(type: "integer", nullable: true),
                    Draft_Team = table.Column<string>(type: "text", nullable: true),
                    College = table.Column<string>(type: "text", nullable: true),
                    Hgt = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<int>(type: "integer", nullable: false),
                    ImgUrl = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<int>(type: "integer", nullable: false),
                    PtModifier = table.Column<double>(type: "double precision", nullable: false),
                    RosterOrder = table.Column<int>(type: "integer", nullable: false),
                    KeyPlayer = table.Column<bool>(type: "boolean", nullable: false),
                    InjuryType = table.Column<string>(type: "text", nullable: true),
                    InjuryGamesRemaining = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Playoffs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    SeriesId = table.Column<int>(type: "integer", nullable: false),
                    Complete = table.Column<bool>(type: "boolean", nullable: true),
                    TeamOneId = table.Column<int>(type: "integer", nullable: false),
                    TeamTwoId = table.Column<int>(type: "integer", nullable: false),
                    WinsTeamOne = table.Column<int>(type: "integer", nullable: false),
                    WinsTeamTwo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playoffs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Playoffs_Teams_TeamOneId",
                        column: x => x.TeamOneId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Playoffs_Teams_TeamTwoId",
                        column: x => x.TeamTwoId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamDraftPicks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TeamId = table.Column<int>(type: "integer", nullable: false),
                    Original = table.Column<string>(type: "text", nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Round = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamDraftPicks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamDraftPicks_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamGameplans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TeamId = table.Column<int>(type: "integer", nullable: false),
                    Pace = table.Column<int>(type: "integer", nullable: false),
                    Focus = table.Column<int>(type: "integer", nullable: false),
                    Motion = table.Column<int>(type: "integer", nullable: false),
                    Defense = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamGameplans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamGameplans_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamPlayoffsStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TeamId = table.Column<int>(type: "integer", nullable: false),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    ConfRank = table.Column<int>(type: "integer", nullable: false),
                    PlayoffWins = table.Column<int>(type: "integer", nullable: false),
                    PlayoffLosses = table.Column<int>(type: "integer", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    AllowedPoints = table.Column<int>(type: "integer", nullable: false),
                    Steals = table.Column<int>(type: "integer", nullable: false),
                    AllowedStealS = table.Column<int>(type: "integer", nullable: false),
                    Rebounds = table.Column<int>(type: "integer", nullable: false),
                    AllowedRebounds = table.Column<int>(type: "integer", nullable: false),
                    Assists = table.Column<int>(type: "integer", nullable: false),
                    AllowedAssists = table.Column<int>(type: "integer", nullable: false),
                    Blocks = table.Column<int>(type: "integer", nullable: false),
                    AllowedBlocks = table.Column<int>(type: "integer", nullable: false),
                    Turnovers = table.Column<int>(type: "integer", nullable: false),
                    AllowedTurnovers = table.Column<int>(type: "integer", nullable: false),
                    FGA = table.Column<int>(type: "integer", nullable: false),
                    FGM = table.Column<int>(type: "integer", nullable: false),
                    AllowedFGA = table.Column<int>(type: "integer", nullable: false),
                    AllowedFGM = table.Column<int>(type: "integer", nullable: false),
                    TPA = table.Column<int>(type: "integer", nullable: false),
                    TPM = table.Column<int>(type: "integer", nullable: false),
                    Allowed3PA = table.Column<int>(type: "integer", nullable: false),
                    Allowed3PM = table.Column<int>(type: "integer", nullable: false),
                    FTM = table.Column<int>(type: "integer", nullable: false),
                    FTA = table.Column<int>(type: "integer", nullable: false),
                    AllowedFTM = table.Column<int>(type: "integer", nullable: false),
                    AllowedFTA = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamPlayoffsStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamPlayoffsStats_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamRegularStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TeamId = table.Column<int>(type: "integer", nullable: false),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    ConfRank = table.Column<int>(type: "integer", nullable: false),
                    Streak = table.Column<int>(type: "integer", nullable: false),
                    HomeWins = table.Column<int>(type: "integer", nullable: false),
                    HomeLosses = table.Column<int>(type: "integer", nullable: false),
                    RoadWins = table.Column<int>(type: "integer", nullable: false),
                    RoadLosses = table.Column<int>(type: "integer", nullable: false),
                    PlayoffWins = table.Column<int>(type: "integer", nullable: false),
                    PlayoffLosses = table.Column<int>(type: "integer", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    AllowedPoints = table.Column<int>(type: "integer", nullable: false),
                    Steals = table.Column<int>(type: "integer", nullable: false),
                    AllowedStealS = table.Column<int>(type: "integer", nullable: false),
                    Fouls = table.Column<int>(type: "integer", nullable: false),
                    AllowedFouls = table.Column<int>(type: "integer", nullable: false),
                    Rebounds = table.Column<int>(type: "integer", nullable: false),
                    AllowedRebounds = table.Column<int>(type: "integer", nullable: false),
                    Assists = table.Column<int>(type: "integer", nullable: false),
                    AllowedAssists = table.Column<int>(type: "integer", nullable: false),
                    Blocks = table.Column<int>(type: "integer", nullable: false),
                    AllowedBlocks = table.Column<int>(type: "integer", nullable: false),
                    Turnovers = table.Column<int>(type: "integer", nullable: false),
                    AllowedTurnovers = table.Column<int>(type: "integer", nullable: false),
                    FGA = table.Column<int>(type: "integer", nullable: false),
                    FGM = table.Column<int>(type: "integer", nullable: false),
                    AllowedFGA = table.Column<int>(type: "integer", nullable: false),
                    AllowedFGM = table.Column<int>(type: "integer", nullable: false),
                    TPA = table.Column<int>(type: "integer", nullable: false),
                    TPM = table.Column<int>(type: "integer", nullable: false),
                    Allowed3PA = table.Column<int>(type: "integer", nullable: false),
                    Allowed3PM = table.Column<int>(type: "integer", nullable: false),
                    FTM = table.Column<int>(type: "integer", nullable: false),
                    FTA = table.Column<int>(type: "integer", nullable: false),
                    AllowedFTM = table.Column<int>(type: "integer", nullable: false),
                    AllowedFTA = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamRegularStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamRegularStats_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TeamOneId = table.Column<int>(type: "integer", nullable: false),
                    TeamTwoId = table.Column<int>(type: "integer", nullable: false),
                    Response = table.Column<bool>(type: "boolean", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trades_Teams_TeamOneId",
                        column: x => x.TeamOneId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trades_Teams_TeamTwoId",
                        column: x => x.TeamTwoId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: true),
                    PasswordHash = table.Column<byte[]>(type: "bytea", nullable: true),
                    PasswordSalt = table.Column<byte[]>(type: "bytea", nullable: true),
                    DataCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TeamId = table.Column<int>(type: "integer", nullable: true),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TeamGameStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameId = table.Column<int>(type: "integer", nullable: false),
                    TeamId = table.Column<int>(type: "integer", nullable: false),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    Min = table.Column<double>(type: "double precision", nullable: false),
                    Fg = table.Column<int>(type: "integer", nullable: false),
                    Fga = table.Column<int>(type: "integer", nullable: false),
                    FgAtRim = table.Column<int>(type: "integer", nullable: false),
                    FgaAtRim = table.Column<int>(type: "integer", nullable: false),
                    FgLowPost = table.Column<int>(type: "integer", nullable: false),
                    FgaLowPost = table.Column<int>(type: "integer", nullable: false),
                    FgMidRange = table.Column<int>(type: "integer", nullable: false),
                    FgaMidRange = table.Column<int>(type: "integer", nullable: false),
                    Tp = table.Column<int>(type: "integer", nullable: false),
                    Tpa = table.Column<int>(type: "integer", nullable: false),
                    Ft = table.Column<int>(type: "integer", nullable: false),
                    Fta = table.Column<int>(type: "integer", nullable: false),
                    Orb = table.Column<int>(type: "integer", nullable: false),
                    Drb = table.Column<int>(type: "integer", nullable: false),
                    Trb = table.Column<int>(type: "integer", nullable: false),
                    Ast = table.Column<int>(type: "integer", nullable: false),
                    Tov = table.Column<int>(type: "integer", nullable: false),
                    Stl = table.Column<int>(type: "integer", nullable: false),
                    Blk = table.Column<int>(type: "integer", nullable: false),
                    Pf = table.Column<int>(type: "integer", nullable: false),
                    Pts = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamGameStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamGameStats_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamGameStats_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Drafts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    Round = table.Column<int>(type: "integer", nullable: false),
                    Pick = table.Column<int>(type: "integer", nullable: false),
                    TeamId = table.Column<int>(type: "integer", nullable: false),
                    Original = table.Column<string>(type: "text", nullable: false),
                    PlayerId = table.Column<int>(type: "integer", nullable: true),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drafts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Drafts_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Drafts_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FAOffers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    TeamId = table.Column<int>(type: "integer", nullable: false),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    Years = table.Column<int>(type: "integer", nullable: false),
                    Response = table.Column<bool>(type: "boolean", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FAOffers_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FAOffers_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerAwardCounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    PlayerOfTheGame = table.Column<int>(type: "integer", nullable: false),
                    MVP = table.Column<int>(type: "integer", nullable: false),
                    DPOY = table.Column<int>(type: "integer", nullable: false),
                    ROY = table.Column<int>(type: "integer", nullable: false),
                    SixthManOfTheYear = table.Column<int>(type: "integer", nullable: false),
                    MIP = table.Column<int>(type: "integer", nullable: false),
                    PlayerOfTheMonth = table.Column<int>(type: "integer", nullable: false),
                    PlayerOfTheWeek = table.Column<int>(type: "integer", nullable: false),
                    AllStarGames = table.Column<int>(type: "integer", nullable: false),
                    TitlesWon = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerAwardCounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerAwardCounts_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerAwards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    Award = table.Column<string>(type: "text", nullable: true),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    Team = table.Column<string>(type: "text", nullable: true),
                    Ppg = table.Column<string>(type: "text", nullable: true),
                    Rpg = table.Column<string>(type: "text", nullable: true),
                    Apg = table.Column<string>(type: "text", nullable: true),
                    Spg = table.Column<string>(type: "text", nullable: true),
                    Bpg = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerAwards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerAwards_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerContracts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    Exp = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerContracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerContracts_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerGameStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Pos = table.Column<string>(type: "text", nullable: true),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    TeamId = table.Column<int>(type: "integer", nullable: false),
                    OppAbrev = table.Column<string>(type: "text", nullable: true),
                    GameDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GameId = table.Column<int>(type: "integer", nullable: false),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    Gs = table.Column<int>(type: "integer", nullable: false),
                    Min = table.Column<double>(type: "double precision", nullable: false),
                    Fg = table.Column<int>(type: "integer", nullable: false),
                    Fga = table.Column<int>(type: "integer", nullable: false),
                    FgAtRim = table.Column<int>(type: "integer", nullable: false),
                    FgaAtRim = table.Column<int>(type: "integer", nullable: false),
                    FgLowPost = table.Column<int>(type: "integer", nullable: false),
                    FgaLowPost = table.Column<int>(type: "integer", nullable: false),
                    FgMidRange = table.Column<int>(type: "integer", nullable: false),
                    FgaMidRange = table.Column<int>(type: "integer", nullable: false),
                    Tp = table.Column<int>(type: "integer", nullable: false),
                    Tpa = table.Column<int>(type: "integer", nullable: false),
                    Ft = table.Column<int>(type: "integer", nullable: false),
                    Fta = table.Column<int>(type: "integer", nullable: false),
                    Orb = table.Column<int>(type: "integer", nullable: false),
                    Drb = table.Column<int>(type: "integer", nullable: false),
                    Ast = table.Column<int>(type: "integer", nullable: false),
                    Tov = table.Column<int>(type: "integer", nullable: false),
                    Stl = table.Column<int>(type: "integer", nullable: false),
                    Blk = table.Column<int>(type: "integer", nullable: false),
                    Pf = table.Column<int>(type: "integer", nullable: false),
                    Pts = table.Column<int>(type: "integer", nullable: false),
                    Trb = table.Column<int>(type: "integer", nullable: false),
                    CourtTime = table.Column<double>(type: "double precision", nullable: false),
                    BenchTime = table.Column<double>(type: "double precision", nullable: false),
                    Energy = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerGameStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerGameStats_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerGameStats_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerPlayoffsStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    TeamId = table.Column<int>(type: "integer", nullable: false),
                    TeamAbrv = table.Column<string>(type: "text", nullable: true),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    Games = table.Column<int>(type: "integer", nullable: false),
                    Gs = table.Column<int>(type: "integer", nullable: false),
                    Min = table.Column<double>(type: "double precision", nullable: false),
                    Fg = table.Column<int>(type: "integer", nullable: false),
                    Fga = table.Column<int>(type: "integer", nullable: false),
                    FgAtRim = table.Column<int>(type: "integer", nullable: false),
                    FgaAtRim = table.Column<int>(type: "integer", nullable: false),
                    FgLowPost = table.Column<int>(type: "integer", nullable: false),
                    FgaLowPost = table.Column<int>(type: "integer", nullable: false),
                    FgMidRange = table.Column<int>(type: "integer", nullable: false),
                    FgaMidRange = table.Column<int>(type: "integer", nullable: false),
                    Tp = table.Column<int>(type: "integer", nullable: false),
                    Tpa = table.Column<int>(type: "integer", nullable: false),
                    Ft = table.Column<int>(type: "integer", nullable: false),
                    Fta = table.Column<int>(type: "integer", nullable: false),
                    Orb = table.Column<int>(type: "integer", nullable: false),
                    Drb = table.Column<int>(type: "integer", nullable: false),
                    Ast = table.Column<int>(type: "integer", nullable: false),
                    Tov = table.Column<int>(type: "integer", nullable: false),
                    Stl = table.Column<int>(type: "integer", nullable: false),
                    Blk = table.Column<int>(type: "integer", nullable: false),
                    Pf = table.Column<int>(type: "integer", nullable: false),
                    Pts = table.Column<int>(type: "integer", nullable: false),
                    Trb = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerPlayoffsStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerPlayoffsStats_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerRatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    Diq = table.Column<int>(type: "integer", nullable: false),
                    Dnk = table.Column<int>(type: "integer", nullable: false),
                    Drb = table.Column<int>(type: "integer", nullable: false),
                    Endu = table.Column<int>(type: "integer", nullable: false),
                    Fg = table.Column<int>(type: "integer", nullable: false),
                    Ft = table.Column<int>(type: "integer", nullable: false),
                    Hgt = table.Column<int>(type: "integer", nullable: false),
                    Ins = table.Column<int>(type: "integer", nullable: false),
                    Jmp = table.Column<int>(type: "integer", nullable: false),
                    Oiq = table.Column<int>(type: "integer", nullable: false),
                    Pot = table.Column<int>(type: "integer", nullable: false),
                    Pss = table.Column<int>(type: "integer", nullable: false),
                    Reb = table.Column<int>(type: "integer", nullable: false),
                    Spd = table.Column<int>(type: "integer", nullable: false),
                    Stre = table.Column<int>(type: "integer", nullable: false),
                    Tp = table.Column<int>(type: "integer", nullable: false),
                    ScoutReport = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerRatings_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerRegularStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Pos = table.Column<string>(type: "text", nullable: true),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    TeamId = table.Column<int>(type: "integer", nullable: false),
                    TeamAbrv = table.Column<string>(type: "text", nullable: true),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    Games = table.Column<int>(type: "integer", nullable: false),
                    Gs = table.Column<int>(type: "integer", nullable: false),
                    Min = table.Column<double>(type: "double precision", nullable: false),
                    Fg = table.Column<int>(type: "integer", nullable: false),
                    Fga = table.Column<int>(type: "integer", nullable: false),
                    FgAtRim = table.Column<int>(type: "integer", nullable: false),
                    FgaAtRim = table.Column<int>(type: "integer", nullable: false),
                    FgLowPost = table.Column<int>(type: "integer", nullable: false),
                    FgaLowPost = table.Column<int>(type: "integer", nullable: false),
                    FgMidRange = table.Column<int>(type: "integer", nullable: false),
                    FgaMidRange = table.Column<int>(type: "integer", nullable: false),
                    Tp = table.Column<int>(type: "integer", nullable: false),
                    Tpa = table.Column<int>(type: "integer", nullable: false),
                    Ft = table.Column<int>(type: "integer", nullable: false),
                    Fta = table.Column<int>(type: "integer", nullable: false),
                    Orb = table.Column<int>(type: "integer", nullable: false),
                    Drb = table.Column<int>(type: "integer", nullable: false),
                    Ast = table.Column<int>(type: "integer", nullable: false),
                    Tov = table.Column<int>(type: "integer", nullable: false),
                    Stl = table.Column<int>(type: "integer", nullable: false),
                    Blk = table.Column<int>(type: "integer", nullable: false),
                    Pf = table.Column<int>(type: "integer", nullable: false),
                    Pts = table.Column<int>(type: "integer", nullable: false),
                    Trb = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerRegularStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerRegularStats_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayoffGames",
                columns: table => new
                {
                    PlayoffsId = table.Column<int>(type: "integer", nullable: false),
                    GameId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayoffGames", x => new { x.PlayoffsId, x.GameId });
                    table.ForeignKey(
                        name: "FK_PlayoffGames_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayoffGames_Playoffs_PlayoffsId",
                        column: x => x.PlayoffsId,
                        principalTable: "Playoffs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TradePicks",
                columns: table => new
                {
                    DraftPickId = table.Column<int>(type: "integer", nullable: false),
                    TradePickId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradePicks", x => new { x.DraftPickId, x.TradePickId });
                    table.ForeignKey(
                        name: "FK_TradePicks_TeamDraftPicks_DraftPickId",
                        column: x => x.DraftPickId,
                        principalTable: "TeamDraftPicks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TradePicks_Trades_TradePickId",
                        column: x => x.TradePickId,
                        principalTable: "Trades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TradePlayer",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    TradePlayerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradePlayer", x => new { x.PlayerId, x.TradePlayerId });
                    table.ForeignKey(
                        name: "FK_TradePlayer_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TradePlayer_Trades_TradePlayerId",
                        column: x => x.TradePlayerId,
                        principalTable: "Trades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Drafts_PlayerId",
                table: "Drafts",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Drafts_TeamId",
                table: "Drafts",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_FAOffers_PlayerId",
                table: "FAOffers",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_FAOffers_TeamId",
                table: "FAOffers",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_AwayTeamId",
                table: "Games",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_HomeTeamId",
                table: "Games",
                column: "HomeTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_SeasonId",
                table: "Games",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAwardCounts_PlayerId",
                table: "PlayerAwardCounts",
                column: "PlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAwards_PlayerId",
                table: "PlayerAwards",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerContracts_PlayerId",
                table: "PlayerContracts",
                column: "PlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGameStats_GameId",
                table: "PlayerGameStats",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerGameStats_PlayerId",
                table: "PlayerGameStats",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPlayoffsStats_PlayerId",
                table: "PlayerPlayoffsStats",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRatings_PlayerId",
                table: "PlayerRatings",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRegularStats_PlayerId",
                table: "PlayerRegularStats",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_TeamId",
                table: "Players",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayoffGames_GameId",
                table: "PlayoffGames",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Playoffs_TeamOneId",
                table: "Playoffs",
                column: "TeamOneId");

            migrationBuilder.CreateIndex(
                name: "IX_Playoffs_TeamTwoId",
                table: "Playoffs",
                column: "TeamTwoId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamDraftPicks_Original_Year_Round",
                table: "TeamDraftPicks",
                columns: new[] { "Original", "Year", "Round" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamDraftPicks_TeamId",
                table: "TeamDraftPicks",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamGameplans_TeamId",
                table: "TeamGameplans",
                column: "TeamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamGameStats_GameId",
                table: "TeamGameStats",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamGameStats_TeamId",
                table: "TeamGameStats",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamPlayoffsStats_TeamId",
                table: "TeamPlayoffsStats",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamRegularStats_TeamId",
                table: "TeamRegularStats",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TradePicks_TradePickId",
                table: "TradePicks",
                column: "TradePickId");

            migrationBuilder.CreateIndex(
                name: "IX_TradePlayer_TradePlayerId",
                table: "TradePlayer",
                column: "TradePlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Trades_TeamOneId",
                table: "Trades",
                column: "TeamOneId");

            migrationBuilder.CreateIndex(
                name: "IX_Trades_TeamTwoId",
                table: "Trades",
                column: "TeamTwoId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TeamId",
                table: "Users",
                column: "TeamId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DraftLotteries");

            migrationBuilder.DropTable(
                name: "Drafts");

            migrationBuilder.DropTable(
                name: "FAOffers");

            migrationBuilder.DropTable(
                name: "GameNews");

            migrationBuilder.DropTable(
                name: "PlayerAwardCounts");

            migrationBuilder.DropTable(
                name: "PlayerAwards");

            migrationBuilder.DropTable(
                name: "PlayerContracts");

            migrationBuilder.DropTable(
                name: "PlayerGameStats");

            migrationBuilder.DropTable(
                name: "PlayerPlayoffsStats");

            migrationBuilder.DropTable(
                name: "PlayerRatings");

            migrationBuilder.DropTable(
                name: "PlayerRegularStats");

            migrationBuilder.DropTable(
                name: "PlayoffGames");

            migrationBuilder.DropTable(
                name: "TeamGameplans");

            migrationBuilder.DropTable(
                name: "TeamGameStats");

            migrationBuilder.DropTable(
                name: "TeamPlayoffsStats");

            migrationBuilder.DropTable(
                name: "TeamRegularStats");

            migrationBuilder.DropTable(
                name: "TradePicks");

            migrationBuilder.DropTable(
                name: "TradePlayer");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Playoffs");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "TeamDraftPicks");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Trades");

            migrationBuilder.DropTable(
                name: "Seasons");

            migrationBuilder.DropTable(
                name: "Teams");
        }
    }
}
