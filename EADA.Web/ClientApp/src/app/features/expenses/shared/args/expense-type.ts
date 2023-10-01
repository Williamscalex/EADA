import { assignMatchesExcept } from "@global/utils";

export interface IExpenseType{
    expenseTypeId : number;
    typeName : string;
}

export interface IExpenseTypeArgs {
    expenseTypeId : number;
    typeName : string;
}

export class ExpenseType implements IExpenseType{
    expenseTypeId: number = 0;
    typeName: string = '';

    private constructor(data?: Partial<IExpenseType>){
        if(data)
            assignMatchesExcept(this as IExpenseType, data);
    }

    static parse(data?: Partial<IExpenseType>) : ExpenseType{
        if(data instanceof ExpenseType) return data;
        return new ExpenseType(data);
    }
}

export class ExpenseTypeArgs implements IExpenseTypeArgs{
    expenseTypeId: number = 0;
    typeName: string = '';

    private constructor(data?: Partial<IExpenseTypeArgs>){
        if(data) assignMatchesExcept(this as ExpenseTypeArgs,data);
    }

    public static parse(data?: Partial<IExpenseTypeArgs>): ExpenseTypeArgs{
        if(data instanceof ExpenseTypeArgs) return data;
        return new ExpenseTypeArgs(data);
    }

}