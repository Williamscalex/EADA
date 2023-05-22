import { ExpenseAction } from "./expense-vm";

export type ExpenseDialogData = {
    expenseId: number;
    action: ExpenseAction;
}