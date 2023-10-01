import { HttpClient } from "@angular/common/http";
import { ErrorNotifierService } from "src/app/services/error-notifier.service";
import { LoggerService } from "src/app/services/logger.service";
import { NavigationHelperService } from "src/app/services/navigation-helper.service";
import { EndpointBaseService } from "../abstracts/endpoint-base.service";
import { BehaviorSubject, Observable, catchError, combineLatest, shareReplay, switchMap } from "rxjs";
import { ConfigurationService } from "src/app/services/configuration.service";
import { environment } from "src/environments/environment";
import { ExpenseTypeArgs, IExpenseType, IExpenseTypeArgs } from "src/app/features/expenses/shared/args/expense-type";
import { RefreshService } from "src/app/services/refresh.service";

const root = `${ConfigurationService.baseUrl}/${environment.apiRoot}expensetype`

export class ExpenseTypeDataService extends EndpointBaseService{ 

    constructor(http: HttpClient,
        navHelper: NavigationHelperService,
        errorNotifier: ErrorNotifierService,
        private refreshService: RefreshService,
        logger: LoggerService)
    {
        super(http,navHelper,errorNotifier,logger);
    }

    private _typeRefresh = new BehaviorSubject<void>(undefined);
    typeRefresh$ = this._typeRefresh.asObservable();

    expenseTypes$ = combineLatest([
        this.typeRefresh$,
        this.refreshService.refresh$
    ]).pipe(
        switchMap(() => this.getExpenseTypes()),
        shareReplay(1)
    );

    /**Retrieves all of the @see IExpenseType in the database */
    getExpenseTypes() : Observable<IExpenseType> {
        const route = root;
        return this.get<IExpenseType[]>(route).pipe(
            catchError(this.buildEndpointErrorHandler())
        );
    }

    /**
     * Retrieves the specified @see IExpenseType from the database.
     * @param typeId 
     * @returns 
     */
    getExpenseTypeById(typeId: number): Observable<IExpenseType>{
        const route = `${root}/${typeId}`;
        return this.get<IExpenseType>(route).pipe(
            catchError(this.buildEndpointErrorHandler())
        );
    }

    /**
     * Creates a new @see ExpenseType in the database
     * @param args 
     * @returns 
     */
    createExpenseType(args: ExpenseTypeArgs): Observable<IExpenseType>{
        const route = `${root}/create`;
        return this.postJson<IExpenseType>(route,args).pipe(
            catchError(this.buildEndpointErrorHandler())
        );
    }

    /**
     * Edits an existing  @see ExpenseType
     * @param args 
     * @returns 
     */
    editExpenseType(args: ExpenseTypeArgs): Observable<IExpenseType>{
        const route = `${root}/edit`;
        return this.postJson<IExpenseType>(route,args).pipe(
            catchError(this.buildEndpointErrorHandler())
        )
    }

    /**
     * Removes the specified @see ExpenseType from the database.
     * @param expenseTypeId 
     * @returns 
     */
    deleteExpenseType(expenseTypeId: number): Observable<any>{
        const route = `${root}/${expenseTypeId}`;
        return this.deleteObj(route).pipe(
            catchError(this.buildEndpointErrorHandler())
        )
    }

    /**Calls the refresh subjects next function to refresh the data */
    refresh() : void{
        this._typeRefresh.next();
    }

}