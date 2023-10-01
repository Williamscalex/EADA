import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { catchError, Observable } from "rxjs";
import { Expense, IExpense } from "src/app/features/expenses/shared/args/expense";
import { ConfigurationService } from "src/app/services/configuration.service";
import { ErrorNotifierService } from "src/app/services/error-notifier.service";
import { LoggerService } from "src/app/services/logger.service";
import { NavigationHelperService } from "src/app/services/navigation-helper.service";
import { environment } from "src/environments/environment";
import { EndpointBaseService } from "../abstracts/endpoint-base.service";

const root = `${ConfigurationService.baseUrl}/${environment.apiRoot}expense`;

@Injectable({
    providedIn: 'root',
})
export class ExpenseDataService extends EndpointBaseService{
    constructor(http: HttpClient,
        navHelper: NavigationHelperService,
        errorNotifier: ErrorNotifierService,
        logger: LoggerService)
    {
        super(http,navHelper,errorNotifier,logger);
    }

    /**
     * Gets all expenses
     * @returns IExpense
     */
    public getExpenses(): Observable<IExpense[]>{
        const route = `${root}`;
        return this.get<IExpense[]>(route).pipe(
            catchError(this.buildEndpointErrorHandler())
        );
    }

    /**
     * Gets the expense by its id
     * @param expenseId 
     * @returns IExpense
     */
    public getExpenseById(expenseId:number): Observable<IExpense>{
        const route = `${root}/${expenseId}`;
        return this.get<IExpense>(route).pipe(
            catchError(this.buildEndpointErrorHandler())
        );
    }
    
    /**
     * edits an existing expense
     * @param args 
     * @returns IExpense
     */
    public editExpense(args: Expense) : Observable<IExpense>{
        const route = `${root}/edit/${args.expenseId}`;
        return this.postJson<IExpense>(route,args).pipe(
            catchError(this.buildEndpointErrorHandler(true))
        );
    }

    /**
     * Creates a new expense.
     * @param args 
     * @returns IExpense
     */
    public createExpense(args: Expense) : Observable<IExpense>{
        const route = `${root}/create`;
        return this.postJson<IExpense>(route,args).pipe(
            catchError(this.buildEndpointErrorHandler(true))
        );
    }

    /**
     * Deletes the specified expense using its id.
     * @param expenseId 
     */
    public deleteExpense(expenseId: number): Observable<any>{
        const route = `${root}/${expenseId}`;
        return this.deleteObj(route).pipe(
            catchError(this.buildEndpointErrorHandler(true))
        )
    }
}