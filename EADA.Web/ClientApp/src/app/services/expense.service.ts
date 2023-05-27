import { Injectable } from "@angular/core";
import { BehaviorSubject, map, Observable, switchAll } from "rxjs";
import { ExpenseDataService } from "../api/expense-data.service";
import { Expense, IExpense } from "../features/expenses/shared/args/expense";

@Injectable({
    providedIn: 'root',
})

export class ExpenseService{
     constructor(private data: ExpenseDataService){}

     private refreshSubject: BehaviorSubject<void> = new BehaviorSubject<void>(null);

     public getExpenses(): Observable<Expense[]>{
        return this.refreshSubject.pipe(
            map(() => this.data.getExpenses()),
            map(e => e.pipe(
              map(data => data.map(x => Expense.parse(x)))
            ))
          ).pipe(
            switchAll()
          );
     }

     public createExpense(args: Expense): Observable<IExpense>{
        return this.data.createExpense(args);
     }

     public editExpense(args: Expense): Observable<IExpense>{
      return this.data.editExpense(args);
     }

     public getExpenseById(expenseId: number) : Observable<Expense>{
        return this.data.getExpenseById(expenseId).pipe(
            map(e => Expense.parse(e))
        );
     }

     public deleteExpense(expenseId: number): Observable<any>{
      return this.data.deleteExpense(expenseId);
     }

     public refresh() : void {
        this.refreshSubject.next();
     }
}