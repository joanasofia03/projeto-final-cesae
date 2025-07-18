import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OpenBankAccount } from './open-bank-account';

describe('OpenBankAccount', () => {
  let component: OpenBankAccount;
  let fixture: ComponentFixture<OpenBankAccount>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OpenBankAccount]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OpenBankAccount);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
