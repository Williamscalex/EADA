import { Component, OnInit } from '@angular/core';
import { map, Observable, switchMap, tap } from 'rxjs';
import { ExpenseService } from 'src/app/services/expense.service';
import { LoggerService } from 'src/app/services/logger.service';
import { Expense } from '../shared/args/expense';

@Component({
  selector: 'app-expense-list',
  templateUrl: './expense-list.component.html',
  styleUrls: ['./expense-list.component.scss']
})
export class ExpenseListComponent implements OnInit {

  constructor(private expenseService: ExpenseService, private logger : LoggerService) { 
  }

  columns: string[] = [
    'expenseName',
    'expenseType',
    'expenseCategory',
    'costPerMonth'
  ];

  panelOpenState = false;

  data$ = this.expenseService.getExpenses().pipe(
    tap(x => this.logger.info('Expense data is loading in....', x))
  );
  
  ngOnInit(): void {
    
  }

   getYearlyTotal(expenses : Expense[]): number{
    let sum: number = 0;
    expenses.forEach(x => {
      sum+= x.costPerMonth
    })

    return sum * 12;
  }

  getCostPerMonth(expenses: Expense[]) : number {
    let sum: number = 0;
    expenses.forEach(x => {
      sum+= x.costPerMonth
    })

    return sum;
  }

  getLargestExpense(expenses: Expense[]) : string{
    let largest : Expense = null;
    expenses.forEach(expense => {
      if(expense > largest || largest === null){
        largest = expense;
      }
    })

    return largest?.expenseName;
  } 

}
