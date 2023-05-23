import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { MatFormFieldAppearance } from '@angular/material/form-field';
import { ExpenseHelper } from '../../../services/expense-create-helper';
import { LoggerService } from 'src/app/services/logger.service';
import { ExpenseService } from 'src/app/services/expense.service';
import { first, map, of, switchMap, tap } from 'rxjs';
import { Expense } from '../../args/expense';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-expense-edit-detail',
  templateUrl: './expense-edit-detail.component.html',
  styleUrls: ['./expense-edit-detail.component.scss']
})
export class ExpenseEditDetailComponent implements OnInit {

  selectedBillingCycle: string = '1';

  constructor(public helper: ExpenseHelper, 
    private logger: LoggerService, 
    private expenseService: ExpenseService) { }

  ngOnInit(): void {

  }

  onConfirmClick() : void{
    this.helper.vm$.pipe(
      first(),
      tap(() => this.helper.loading = true),
      map(x => {
        let args: Expense = new Expense();
        const form = x.expenseForm.value;
        args= {
          expenseId: this.helper.expenseId,
          expenseName: form.expenseName,
          expenseCategoryId: form.expenseCategory,
          expenseTypeId: form.expenseType,
          description: form.description,
          costPerYear: form.costPerYear,
          costPerMonth: form.costPerMonth
        };
        return args;
      }),
      switchMap(x => this.expenseService.editExpense(x))
    ).subscribe({
      next: () => {
        Swal.fire('Expense Edited!','','success');
        this.expenseService.refresh();
        this.helper.drawerService.drawerClose();
      },
      error: err => {
        this.logger.crit(err);
        Swal.fire({
          icon:'error',
          text:'Error editing expense.'
        });
        this.helper.drawerService.drawerClose();
      }
    })
  }

  onDeleteClicked(): void {
    let expenseId: number = this.helper.expenseId;
    Swal.fire({
      icon: 'warning',
      text: `Are you sure you want to delete ${this.helper?.title}?`,
      showCancelButton: true,
      showConfirmButton: true
    }).then(result => {
      if(result.isConfirmed){
        this.expenseService.deleteExpense(expenseId).pipe(first())
        .subscribe({
          next: () => {
            this.expenseService.refresh();
            this.helper.drawerService.drawerClose();
          }
        });
      }
    }).finally(() => Swal.close());
  }
}
