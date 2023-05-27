import { NgModule } from '@angular/core';
import { ExpenseListComponent } from './expense-list/expense-list.component';
import { ExpensesRoutingModule } from './expenses-routing.module';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from 'src/app/shared/shared.module';
import { ExpenseCreateComponent } from './expense-create/expense-create.component';
import { ExpenseHelper } from './services/expense-create-helper';
import { ExpenseVmSteps } from './expense-create/pipelines/expense-vm-steps';
import { ExpenseEditDetailComponent } from './shared/components/expense-edit-detail/expense-edit-detail.component';



@NgModule({
  declarations: [
    ExpenseListComponent,
    ExpenseCreateComponent,
    ExpenseEditDetailComponent
  ],
  imports: [
    ExpensesRoutingModule,
    ReactiveFormsModule,
    SharedModule
  ],
  providers:[ExpenseHelper, ExpenseVmSteps]
})
export class ExpensesModule { }
