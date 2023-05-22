import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSelectChange } from '@angular/material/select';
import { catchError, first, map, of, switchMap, takeUntil, tap } from 'rxjs';
import { ExpenseService } from 'src/app/services/expense.service';
import { LoggerService } from 'src/app/services/logger.service';
import Swal from 'sweetalert2';
import { ExpenseDialogData } from '../models/expense-dialog-data';
import { ExpenseHelper } from '../services/expense-create-helper';
import { Expense } from '../shared/args/expense';

@Component({
  selector: 'app-expense-create',
  templateUrl: './expense-create.component.html',
  styleUrls: ['./expense-create.component.scss']
})
export class ExpenseCreateComponent implements OnInit {

  constructor(
    public helper: ExpenseHelper,
    private logger: LoggerService,
    public dialogRef: MatDialogRef<ExpenseCreateComponent>,
    private expenseService: ExpenseService,
    @Inject(MAT_DIALOG_DATA) public data: ExpenseDialogData
    ) { }

  vm$ = this.helper.vm$;

  selectedBillingCycle: string = '1';
  disabled: boolean = false;

  ngOnInit(): void {
    of(this.data).pipe(
      takeUntil(this.helper.destroy$)
    ).subscribe(data => this.helper.load(data.expenseId,data.action,this.dialogRef))
  }

  onSubmit() : void{
    this.disabled = true;
    this.logger.info('submitting form values...');
    this.vm$.pipe(
      first(),
      map(x => {
        x.expenseForm.markAllAsTouched();
        const formVal = x.expenseForm.value;
        let args: Expense = new Expense();

        args = {
          expenseId: 0,
          expenseName: formVal.expenseName,
          expenseTypeId: formVal.expenseType,
          expenseCategoryId: formVal.expenseCategory,
          costPerMonth: formVal.costPerMonth,
          costPerYear: formVal.costPerYear,
          description: formVal.description
        };

        this.logger.info('args: ', args);

        return args;
      }),
      switchMap(x => this.expenseService.createExpense(x).pipe(
        catchError(err => {
          Swal.fire({
            title: 'Error!',
            text: 'Failed to created expense',
            icon: 'error',
            confirmButtonText: 'Ok'
          });
          this.disabled = false;
          this.logger.crit(err);
          throw err;
        })
      ))
    ).subscribe(() => {
      Swal.fire({
        position: 'top',
        icon: 'success',
        title: 'Expense Created!',
        showConfirmButton: false,
        timer: 1500
      })
      this.disabled = false;
      this.expenseService.refresh();
      this.dialogRef.close();
    });
  }

  onCancelClicked() : void{
    this.helper.closeDialog();
  }
}
