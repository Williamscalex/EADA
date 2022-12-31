import { Injectable } from "@angular/core";
import { map, Observable } from "rxjs";
import { ExpenseDataService } from "../api/expense-data.service";
import { Expense } from "../features/expenses/shared/args/expense";

@Injectable({
    providedIn: 'root',
})

export class ExpenseService{
     constructor(private data: ExpenseDataService){}

     public getExpenses(): Observable<Expense[]>{
        return this.data.getExpenses().pipe(
            map(e => e.map(x => Expense.parse(x)))
        )
     }
}