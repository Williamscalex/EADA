import { Component, OnInit } from '@angular/core';
import { first, map, of, switchMap, tap } from 'rxjs';
import { ExpenseService } from 'src/app/services/expense.service';
import { LoggerService } from 'src/app/services/logger.service';
import { ExpenseHelper } from '../services/expense-create-helper';
import { Expense } from '../shared/args/expense';
import { MatDrawer } from '@angular/material/sidenav';
import { ExpenseDrawerService } from '../services/expense-drawer-service';

@Component({
  selector: 'app-expense-list',
  templateUrl: './expense-list.component.html',
  styleUrls: ['./expense-list.component.scss']
})
export class ExpenseListComponent implements OnInit {

  constructor(public helper: ExpenseHelper, private expenseService: ExpenseService, private logger : LoggerService) { 
  }

  panelOpenState = false;
  showDetails = false;

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

  onCreateClicked(): void{
    this.helper.create();
  }

  onRowClick(drawer: MatDrawer, expense: Expense){
     of(expense).pipe(
      first(),
      tap(() => {
        this.helper.expenseId = expense.expenseId,
        this.helper.title= expense.expenseName,
        this.helper.drawer = drawer;
      }),
      map(data => this.helper.load(data.expenseId,'edit'))
     ).subscribe({
      next: () => {
        this.helper.drawerService = new ExpenseDrawerService(drawer);
        this.helper.drawerService.openDrawer();
      },
      error: err => this.logger.crit('Error loading edit form...', err)
     });
  }

  monthCostCheck(expense: Expense): string{
    if(expense.costPerMonth === 0 || expense.costPerMonth == null){
      return 'N/A'
    } else{
      return `$  ${expense.costPerMonth}`;
    }
  }

  yearCostCheck(expense: Expense): string{
    if(expense.costPerYear === 0 || expense.costPerYear == null){
      return 'N/A'
    } else {
      return `$  ${expense.costPerYear}`;
    }
  }
}
