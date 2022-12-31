import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ExpenseListComponent } from './expense-list/expense-list.component';

const routes : Routes  = [
  {
    path: '',
    data: { title: 'Expenses'},
    children: [
      {
        path: '',
        component: ExpenseListComponent,
        data: {title:'Expense List'}
      }
    ]
  }
] 

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports:[RouterModule]
})
export class ExpensesRoutingModule { }
