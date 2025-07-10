using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crysta.Migrations
{
    /// <inheritdoc />
    public partial class MigrationUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dim_Account",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Account_Type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Account_Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AppUser_ID = table.Column<int>(type: "int", nullable: true),
                    Opening_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dim_Account", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Dim_Account_AppUser_AppUser_ID",
                        column: x => x.AppUser_ID,
                        principalTable: "AppUser",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Dim_Market_Asset",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Asset_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Asset_Type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Symbol = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Base_Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    API_Source = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dim_Market_Asset", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Dim_Time",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    date_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    date_Year = table.Column<int>(type: "int", nullable: true),
                    date_Month = table.Column<int>(type: "int", nullable: true),
                    date_Quarter = table.Column<int>(type: "int", nullable: true),
                    Weekday_Name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Is_Weekend = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dim_Time", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Dim_Transaction_Type",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Dim_Transaction_Type_Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dim_Transaction_Type", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Fact_Market_Asset_History",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Asset_ID = table.Column<int>(type: "int", nullable: true),
                    Time_ID = table.Column<int>(type: "int", nullable: true),
                    Open_Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Close_Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Trading_Volume = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fact_Market_Asset_History", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Fact_Market_Asset_History_Dim_Market_Asset_Asset_ID",
                        column: x => x.Asset_ID,
                        principalTable: "Dim_Market_Asset",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fact_Market_Asset_History_Dim_Time_Time_ID",
                        column: x => x.Time_ID,
                        principalTable: "Dim_Time",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Fact_Notifications",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppUser_ID = table.Column<int>(type: "int", nullable: true),
                    Time_ID = table.Column<int>(type: "int", nullable: true),
                    Notification_Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Channel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Fact_Notifications_Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fact_Notifications", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Fact_Notifications_AppUser_AppUser_ID",
                        column: x => x.AppUser_ID,
                        principalTable: "AppUser",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fact_Notifications_Dim_Time_Time_ID",
                        column: x => x.Time_ID,
                        principalTable: "Dim_Time",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Fact_Transactions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Source_Account_ID = table.Column<int>(type: "int", nullable: true),
                    Destination_Account_ID = table.Column<int>(type: "int", nullable: true),
                    Time_ID = table.Column<int>(type: "int", nullable: true),
                    Transaction_Type_ID = table.Column<int>(type: "int", nullable: true),
                    AppUser_ID = table.Column<int>(type: "int", nullable: true),
                    Transaction_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Balance_After_Transaction = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Execution_Channel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Transaction_Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fact_Transactions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Fact_Transactions_AppUser_AppUser_ID",
                        column: x => x.AppUser_ID,
                        principalTable: "AppUser",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fact_Transactions_Dim_Account_Destination_Account_ID",
                        column: x => x.Destination_Account_ID,
                        principalTable: "Dim_Account",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fact_Transactions_Dim_Account_Source_Account_ID",
                        column: x => x.Source_Account_ID,
                        principalTable: "Dim_Account",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fact_Transactions_Dim_Time_Time_ID",
                        column: x => x.Time_ID,
                        principalTable: "Dim_Time",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fact_Transactions_Dim_Transaction_Type_Transaction_Type_ID",
                        column: x => x.Transaction_Type_ID,
                        principalTable: "Dim_Transaction_Type",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dim_Account_AppUser_ID",
                table: "Dim_Account",
                column: "AppUser_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Fact_Market_Asset_History_Asset_ID",
                table: "Fact_Market_Asset_History",
                column: "Asset_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Fact_Market_Asset_History_Time_ID",
                table: "Fact_Market_Asset_History",
                column: "Time_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Fact_Notifications_AppUser_ID",
                table: "Fact_Notifications",
                column: "AppUser_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Fact_Notifications_Time_ID",
                table: "Fact_Notifications",
                column: "Time_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Fact_Transactions_AppUser_ID",
                table: "Fact_Transactions",
                column: "AppUser_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Fact_Transactions_Destination_Account_ID",
                table: "Fact_Transactions",
                column: "Destination_Account_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Fact_Transactions_Source_Account_ID",
                table: "Fact_Transactions",
                column: "Source_Account_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Fact_Transactions_Time_ID",
                table: "Fact_Transactions",
                column: "Time_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Fact_Transactions_Transaction_Type_ID",
                table: "Fact_Transactions",
                column: "Transaction_Type_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fact_Market_Asset_History");

            migrationBuilder.DropTable(
                name: "Fact_Notifications");

            migrationBuilder.DropTable(
                name: "Fact_Transactions");

            migrationBuilder.DropTable(
                name: "Dim_Market_Asset");

            migrationBuilder.DropTable(
                name: "Dim_Account");

            migrationBuilder.DropTable(
                name: "Dim_Time");

            migrationBuilder.DropTable(
                name: "Dim_Transaction_Type");
        }
    }
}
