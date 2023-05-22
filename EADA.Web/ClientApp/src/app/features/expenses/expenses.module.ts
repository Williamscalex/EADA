import { NgModule } from '@angular/core';
import { ExpenseListComponent } from './expense-list/expense-list.component';
import { ExpensesRoutingModule } from './expenses-routing.module';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from 'src/app/shared/shared.module';
import { ExpenseCreateComponent } from './expense-create/expense-create.component';
import { ExpenseHelper } from './services/expense-create-helper';
import { ExpenseVmSteps } from './expense-create/pipelines/expense-vm-steps';



@NgModule({
  declarations: [
    ExpenseListComponent,
    ExpenseCreateComponent
  ],
  imports: [
    ExpensesRoutingModule,
    ReactiveFormsModule,
    SharedModule
  ],
  providers:[ExpenseHelper, ExpenseVmSteps]
})
export class ExpensesModule { }
