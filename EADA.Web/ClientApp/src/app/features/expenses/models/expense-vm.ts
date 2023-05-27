import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from "@angular/forms";
import { assignMatchesExcept, newId } from "@global/utils";
import { Expense } from "../shared/args/expense";

export type ExpenseAction = 'create'| 'edit';

/**
 * Custom validator for the cost per year and cost per month form fields.
 * This will check if the form has a value for at least one of these fields, if not,
 * an form error will be thrown.
 * @returns 
 */
function atLeastOneValueValidator(): ValidatorFn {
    return (control: AbstractControl): {[key: string]: any} | null => {
      const costPerYear = control.get('costPerYear');
      const costPerMonth = control.get('costPerMonth');
  
      if (!costPerYear.value && !costPerMonth.value) {
        return { 'atLeastOneValueRequired': true };
      }
  
      return null;
    };
}

export class ExpenseVm{
    
    rowId: string = newId();
    expenseArgs: Expense= null;
    expenseForm: FormGroup = null;
    action: ExpenseAction = 'create';
    expense: Expense = null;

    constructor(expense: Expense, form: FormGroup)
    constructor(data?: Partial<ExpenseVm>)
    constructor(data?: Partial<ExpenseVm> | Expense, expenseForm: FormGroup = null){
        if(data){
            if(data instanceof Expense){
                this.expenseArgs = Expense.parse(data);
                this.expenseForm = expenseForm ?? ExpenseVm.buildForm(this.expenseArgs,new FormBuilder());
            }
            else{
                assignMatchesExcept(this as ExpenseVm, data);
            }
        }
    }
    

    static buildForm(args: Expense, fb: FormBuilder): FormGroup {
        const form= fb.group({
            expenseName: fb.control(args.expenseName ?? null ,[Validators.required, Validators.maxLength(50)]),
            expenseType: fb.control(args.expenseType.expenseTypeId ?? null, [Validators.required]),
            expenseCategory: fb.control(args.expenseCategory.expenseCategoryId ?? null,[Validators.required]),
            costPerMonth: fb.control(args?.costPerMonth ?? null),
            costPerYear: fb.control(args?.costPerYear ?? null),
            description: fb.control(args.description ?? '', [Validators.maxLength(150)])
        }, {validators: atLeastOneValueValidator()});

        return form;
    }

    static parse(data?: Partial<ExpenseVm>): ExpenseVm{
        if(data instanceof ExpenseVm){
            return data;
        }
        return new ExpenseVm(data);
    }
}