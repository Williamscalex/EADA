import { assignMatchesExcept } from "@global/utils";

export interface IExpenseCategory{
    expenseCategoryId : number;
    categoryName : string;
}

export class ExpenseCategory implements IExpenseCategory{
    expenseCategoryId: number = 0;
    categoryName: string = '';

    constructor(data?: Partial<IExpenseCategory>){
        if(data)
            assignMatchesExcept(this as IExpenseCategory, data);
    }

    static parse(data?: Partial<IExpenseCategory>) : ExpenseCategory{
        if(data instanceof ExpenseCategory) return data;
        return new ExpenseCategory(data);
    }
}