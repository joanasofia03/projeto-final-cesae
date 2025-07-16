import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Resgister } from './register';

describe('Resgister', () => {
  let component: Resgister;
  let fixture: ComponentFixture<Resgister>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Resgister]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Resgister);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
