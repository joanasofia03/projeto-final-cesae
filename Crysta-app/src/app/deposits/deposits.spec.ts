import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Deposits } from './deposits';

describe('Deposits', () => {
  let component: Deposits;
  let fixture: ComponentFixture<Deposits>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Deposits]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Deposits);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
