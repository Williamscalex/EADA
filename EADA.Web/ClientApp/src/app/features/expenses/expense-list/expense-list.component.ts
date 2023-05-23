import { Component, OnInit } from '@angular/core';
import { catchError, first, tap } from 'rxjs';
import { ExpenseService } from 'src/app/services/expense.service';
import { LoggerService } from 'src/app/services/logger.service';
import { ExpenseHelper } from '../services/expense-create-helper';
import { Expense } from '../shared/args/expense';
import Swal from 'sweetalert2';

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

  onDeleteClicked(expense: Expense): void {
    Swal.fire({
      icon: 'warning',
      text: `Are you sure you want to delete ${expense.expenseName}?`,
      showCancelButton: true,
      showConfirmButton: true
    }).then(result => {
      if(result.isConfirmed){
        this.expenseService.deleteExpense(expense.expenseId).pipe(first())
        .subscribe({
          next: () => this.expenseService.refresh()
        });
        Swal.close();
      }
      else{
        Swal.close();
      }
    });
  }

}
