import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MarketAssets } from './market-assets';

describe('MarketAssets', () => {
  let component: MarketAssets;
  let fixture: ComponentFixture<MarketAssets>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MarketAssets]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MarketAssets);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
