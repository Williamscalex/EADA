import { assignMatchesExcept } from '@global/utils';
import {ExpenseCategory} from './expense-category';
import { ExpenseType } from './expense-type';

export interface IExpense{
    expenseId : number;
    expenseName: string;
    expenseType?: ExpenseType;
    expenseCategory?: ExpenseCategory
    costPerMonth?: number;
    costPerYear?:number;
    description: string;
    expenseTypeId: number;
    expenseCategoryId: number;
}

export class Expense implements IExpense{
    expenseId: number = 0;
    expenseName: string = '';
    expenseType?: ExpenseType = new ExpenseType();
    expenseCategory?: ExpenseCategory = new ExpenseCategory();
    costPerMonth: number = null;
    costPerYear: number = null;
    description: string = '';
    expenseTypeId: number = 0;
    expenseCategoryId: number = 0;


    constructor(data?: Partial<IExpense>){
        if(data)
            assignMatchesExcept(this as IExpense,data);
    }

    static parse(data?:Partial<IExpense>):Expense{
        if(data instanceof Expense) return data;
        return new Expense(data);
    }
}