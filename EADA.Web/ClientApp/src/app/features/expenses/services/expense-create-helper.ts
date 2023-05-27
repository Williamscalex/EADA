import { Injectable } from "@angular/core";
import { MatDialog, MatDialogConfig, MatDialogRef } from "@angular/material/dialog";
import { BehaviorSubject, combineLatest, debounceTime, filter, first, map, of, shareReplay, Subject, switchMap, takeUntil, tap } from "rxjs";
import { LoggerService } from "src/app/services/logger.service";
import { ExpenseCreateComponent } from "../expense-create/expense-create.component";
import { ExpenseVmSteps } from "../expense-create/pipelines/expense-vm-steps";
import { ExpenseDialogData } from "../models/expense-dialog-data";
import { ExpenseAction, ExpenseVm } from "../models/expense-vm";
import { MatDrawer } from "@angular/material/sidenav";
import { ExpenseDrawerService } from "./expense-drawer-service";

@Injectable()
export class ExpenseHelper{
    constructor(private logger: LoggerService,
         private expenseVmSteps: ExpenseVmSteps,
         private dialog: MatDialog){ }

    public loading: boolean = false;
    public saving: boolean= false;
    public expenseId: number = 0;
    public dialogRef: MatDialogRef<ExpenseCreateComponent> = null;
    private _expenseVm: ExpenseVm = null;
    private _loaded$=  new BehaviorSubject<{expenseVm: ExpenseVm}>(null);
    private _destroy$ = new Subject();
    public action: ExpenseAction = null;
    public title: string = '';
    public drawer: MatDrawer = null;
    public drawerService: ExpenseDrawerService = null;

    public destroy$ = this._destroy$.pipe(
        debounceTime(100),
        tap(() => this.logger.info('Expense Creator Session Destroyed')),
        shareReplay(1)
    );

    dialogOptions: MatDialogConfig = {
        width: '600px',
        height: '460px',
        disableClose: true
    };

    vm$ = this._loaded$.pipe(
        tap(x => this._expenseVm = x?.expenseVm),
        map(x => this._expenseVm),
        shareReplay(1)
    );

    public loaded$ = this.vm$.pipe(
        filter(vm => !!vm),
        map(() => null)
    );

    load(expenseId: number,
         action: ExpenseAction,
         dialog?: MatDialogRef<ExpenseCreateComponent>){
        this.loading = true;
        this.action = action;
        this.dialogRef = dialog;
        of({expenseId, action}).pipe(
            this.expenseVmSteps.routines.addExpenseVm(this.destroy$),
            takeUntil(this.destroy$)
        ).subscribe({
            next: vm => {
                this._loaded$.next(vm);
                this.vm$.pipe(
                    first(),
                    tap(_vm => this.logger.info('Expense Vm Loaded: ', _vm))
                ).subscribe();
                this.loading = false;
            },
            error: err => {
                if(this.dialogRef){
                    this.dialogRef.close();
                }
                this.logger.crit('failed to load vm');
                this.destroy()
            }
        });
    }

    closeDialog(){
        this.vm$.pipe(
            first(),
            map(vm => {
                this.dialogRef.close()
            })
        ).subscribe();
    }

    destroy() : void{
        this._destroy$.next(null);
        this._loaded$.next(null);
        this.loading = false;
        this.saving = false;
    }

    create(): void{
        let data: ExpenseDialogData = {
            expenseId: 0,
            action: 'create'
        };

        this.dialogOptions.data = data;

        const ref = this.dialog.open(ExpenseCreateComponent, this.dialogOptions);

        ref.afterClosed().subscribe(result => {
            this.logger.info('Data', result)
        });
    }
}