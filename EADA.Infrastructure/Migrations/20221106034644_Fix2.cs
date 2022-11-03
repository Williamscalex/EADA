using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EADA.Infrastructure.Migrations
{
    public partial class Fix2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpensesCategories_ExpenseCategoryId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpenseTypes_ExpenseTypeId",
                table: "Expenses");

            migrationBuilder.CreateTable(
                name: "ExpenseExpenseCategory",
                columns: table => new
                {
                    CategoriesExpenseCategoryId = table.Column<int>(type: "int", nullable: false),
                    ExpenseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseExpenseCategory", x => new { x.CategoriesExpenseCategoryId, x.ExpenseId });
                    table.ForeignKey(
                        name: "FK_ExpenseExpenseCategory_Expenses_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "Expenses",
                        principalColumn: "ExpenseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpenseExpenseCategory_ExpensesCategories_CategoriesExpenseCategoryId",
                        column: x => x.CategoriesExpenseCategoryId,
                        principalTable: "ExpensesCategories",
                        principalColumn: "ExpenseCategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseExpenseType",
                columns: table => new
                {
                    ExpenseId = table.Column<int>(type: "int", nullable: false),
                    TypesExpenseTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseExpenseType", x => new { x.ExpenseId, x.TypesExpenseTypeId });
                    table.ForeignKey(
                        name: "FK_ExpenseExpenseType_Expenses_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "Expenses",
                        principalColumn: "ExpenseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpenseExpenseType_ExpenseTypes_TypesExpenseTypeId",
                        column: x => x.TypesExpenseTypeId,
                        principalTable: "ExpenseTypes",
                        principalColumn: "ExpenseTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseExpenseCategory_ExpenseId",
                table: "ExpenseExpenseCategory",
                column: "ExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseExpenseType_TypesExpenseTypeId",
                table: "ExpenseExpenseType",
                column: "TypesExpenseTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpensesCategories_ExpenseCategoryId",
                table: "Expenses",
                column: "ExpenseCategoryId",
                principalTable: "ExpensesCategories",
                principalColumn: "ExpenseCategoryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpenseTypes_ExpenseTypeId",
                table: "Expenses",
                column: "ExpenseTypeId",
                principalTable: "ExpenseTypes",
                principalColumn: "ExpenseTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpensesCategories_ExpenseCategoryId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpenseTypes_ExpenseTypeId",
                table: "Expenses");

            migrationBuilder.DropTable(
                name: "ExpenseExpenseCategory");

            migrationBuilder.DropTable(
                name: "ExpenseExpenseType");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpensesCategories_ExpenseCategoryId",
                table: "Expenses",
                column: "ExpenseCategoryId",
                principalTable: "ExpensesCategories",
                principalColumn: "ExpenseCategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpenseTypes_ExpenseTypeId",
                table: "Expenses",
                column: "ExpenseTypeId",
                principalTable: "ExpenseTypes",
                principalColumn: "ExpenseTypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
