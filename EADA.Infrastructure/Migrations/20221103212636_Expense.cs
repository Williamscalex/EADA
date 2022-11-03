using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EADA.Infrastructure.Migrations
{
    public partial class Expense : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpensesCategories",
                columns: table => new
                {
                    ExpenseCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpensesCategories", x => x.ExpenseCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseTypes",
                columns: table => new
                {
                    ExpenseTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseTypes", x => x.ExpenseTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    ExpenseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpenseName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExpenseTypeId = table.Column<int>(type: "int", nullable: false),
                    ExpenseCategoryId = table.Column<int>(type: "int", nullable: false),
                    CostPerMonth = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.ExpenseId);
                    table.ForeignKey(
                        name: "FK_Expenses_ExpensesCategories_ExpenseCategoryId",
                        column: x => x.ExpenseCategoryId,
                        principalTable: "ExpensesCategories",
                        principalColumn: "ExpenseCategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Expenses_ExpenseTypes_ExpenseTypeId",
                        column: x => x.ExpenseTypeId,
                        principalTable: "ExpenseTypes",
                        principalColumn: "ExpenseTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseCategoryId",
                table: "Expenses",
                column: "ExpenseCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseId",
                table: "Expenses",
                column: "ExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseName",
                table: "Expenses",
                column: "ExpenseName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseTypeId",
                table: "Expenses",
                column: "ExpenseTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "ExpensesCategories");

            migrationBuilder.DropTable(
                name: "ExpenseTypes");
        }
    }
}
