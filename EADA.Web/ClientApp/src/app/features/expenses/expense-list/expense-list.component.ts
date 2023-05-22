import { Component, OnInit } from '@angular/core';
import { tap } from 'rxjs';
import { ExpenseService } from 'src/app/services/expense.service';
import { LoggerService } from 'src/app/services/logger.service';
import { ExpenseHelper } from '../services/expense-create-helper';
import { Expense } from '../shared/args/expense';

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

  onRowClick(expense: Expense){
    this.logger.info('clicked');
    if(this.showDetails === false){
      this.showDetails = true;
    }
    else{
      this.showDetails = false;
    }
  }

}
