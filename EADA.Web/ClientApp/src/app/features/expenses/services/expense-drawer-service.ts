import { MatFormFieldAppearance } from "@angular/material/form-field";
import { MatDrawer } from "@angular/material/sidenav";

export class ExpenseDrawerService{

    private _isEdit: boolean = false;
    private _appearance: MatFormFieldAppearance = 'standard';

    constructor(private drawer: MatDrawer){ }

    get isEdit(): boolean{
        return this._isEdit;
    }

    get appearance() : MatFormFieldAppearance{
        return this._appearance;
    }

    /** Resets back to the default values of the drawer and closes the drawer. */
    drawerClose() : void{
        this._isEdit = false;
        this._appearance = 'standard';
        this.drawer.close()
    }

    openDrawer() : void{
        this.drawer.open();
    }

    /**
     * changes the properties to display the 'Edit' appearance or not 
     * based on the provided boolean.
     * @param isEdit boolean value that represent if the state is in edit or not
     */
    onEditClicked() : void{
        if(this._isEdit){
            this._isEdit = false;
            this._appearance = 'standard'
        } else{
            this._isEdit = true;
            this._appearance = 'outline';
        }
    }

    /**Used on the drawer's change event to reset back to default */
    onToggleChanged(): void {
        this._isEdit = false;
        this._appearance = 'standard';
    }
}