import { NgModule } from '@angular/core';
import { ExpenseListComponent } from './expense-list/expense-list.component';
import { ExpensesRoutingModule } from './expenses-routing.module';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from 'src/app/shared/shared.module';



@NgModule({
  declarations: [
    ExpenseListComponent
  ],
  imports: [
    ExpensesRoutingModule,
    ReactiveFormsModule,
    SharedModule
  ]
})
export class ExpensesModule { }
