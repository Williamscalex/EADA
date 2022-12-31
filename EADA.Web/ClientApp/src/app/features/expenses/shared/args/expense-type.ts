import { assignMatchesExcept } from "@global/utils";

export interface IExpenseType{
    expenseTypeId : number;
    typeName : string;
}

export class ExpenseType implements IExpenseType{
    expenseTypeId: number = 0;
    typeName: string = '';

    constructor(data?: Partial<IExpenseType>){
        if(data)
            assignMatchesExcept(this as IExpenseType, data);
    }

    static parse(data?: Partial<IExpenseType>) : ExpenseType{
        if(data instanceof ExpenseType) return data;
        return new ExpenseType(data);
    }
}