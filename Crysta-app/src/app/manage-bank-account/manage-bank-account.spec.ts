import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageBankAccount } from './manage-bank-account';

describe('ManageBankAccount', () => {
  let component: ManageBankAccount;
  let fixture: ComponentFixture<ManageBankAccount>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManageBankAccount]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManageBankAccount);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
