import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExpenseEditDetailComponent } from './expense-edit-detail.component';

describe('ExpenseEditDetailComponent', () => {
  let component: ExpenseEditDetailComponent;
  let fixture: ComponentFixture<ExpenseEditDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ExpenseEditDetailComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ExpenseEditDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
