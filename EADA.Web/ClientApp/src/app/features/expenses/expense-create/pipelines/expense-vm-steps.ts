import { Injectable } from "@angular/core";
import { FormBuilder, FormGroup } from "@angular/forms";
import { map, Observable, of, switchMap, tap } from "rxjs";
import { ExpenseService } from "src/app/services/expense.service";
import { LoggerService } from "src/app/services/logger.service";
import { ExpenseAction, ExpenseVm } from "../../models/expense-vm";
import { Expense } from "../../shared/args/expense";

@Injectable()
export class ExpenseVmSteps {

    constructor(
        private logger: LoggerService, 
        private fb: FormBuilder, 
        private expenseService: ExpenseService){}

    public routines = {
        addExpenseVm: 
        <T extends {
            action: ExpenseAction;
            expenseId : number;
        }>(destroy$: Observable<any>):
        (source: Observable<T>) => Observable<T & {
            expenseVm: ExpenseVm;
        }> => {
            const retval = (source : Observable<T>) => source.pipe(
                this.addExpense(),
                this.addForm(),
                this.finish()
            );

            return retval;
        }
    };
 
    /**
     * Adds an existing expense or create a new expense
     * @returns 
     */
    public addExpense<T extends {
        action: ExpenseAction;
        expenseId: number;
    }>():(source:Observable<T>) => Observable<T & {
        expense: Expense;
    }>{
        return source => source.pipe(
            tap(x => this.logger.info('loading expense...')),
            switchMap(data => {
                let expense$ = data.action === 'edit'
                ? this.expenseService.getExpenseById(data.expenseId)
                : of(new Expense());
                
                return expense$.pipe(map(expense => ({...data, expense})))
            })

        );
    }
    
    /**
     * Builds our the form 
     * @returns 
     */
    public addForm<T extends {
        expense: Expense;
    }>(): (source: Observable<T>) => Observable<T & {
        form: FormGroup;
    }>{
        return source => source.pipe(
            tap(x => this.logger.info('Adding expense form....')),
            map(x => {
                const form = ExpenseVm
                    .buildForm(x.expense, this.fb);
                    return ({...x,form})
            })
        );
    }
    /**
     * Builds the ExpenseVm
     * @returns 
     */
    public finish<T extends {
        form: FormGroup;
    }>():(source: Observable<T>) => Observable<T & {
        expenseVm: ExpenseVm;
    }>{
        return source => source.pipe(
            map(data => {
                const vm = new ExpenseVm();
                vm.expenseForm = data.form;
                return {...data, expenseVm: vm}
            })
        );
    }
}