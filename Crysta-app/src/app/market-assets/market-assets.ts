// market-assets.component.ts
import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-market-assets',
  templateUrl: './market-assets.html',
  styleUrls: ['./market-assets.css'],
  imports: [CommonModule, FormsModule],
  providers: [DatePipe]
})

export class MarketAssetsComponent implements OnInit {
  assets: any[] = [];
  selectedAssetId: number | null = null;
  history: any[] = [];
  error: string = '';
  startDate: string = '';
  endDate: string = '';

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.loadAssets();
  }

  loadAssets(): void {
    this.http.get<any[]>('http://localhost:5146/api/dim_market_asset/getall').subscribe({
      next: (data) => {
        this.assets = data;
        console.log('Assets loaded:', this.assets);
        this.error = '';
      },
      error: (err) => {
        this.error = 'Failed to load market assets.';
      }
    });
  }

  loadHistory(assetId: number): void {
    this.selectedAssetId = assetId;
    this.http.get<any[]>(`http://localhost:5146/api/Fact_Market_Asset_History/getall/${assetId}`).subscribe({
      next: (data) => {
        this.history = data;
        console.log('History loaded for asset ID', assetId, ':', this.history);
        this.error = '';
      },
      error: (err) => {
        this.history = [];
        this.error = typeof err.error === 'string' ? err.error : 'Failed to load history.';
      }
    });
  }

  get filteredHistory() {
    return this.history.filter(h => {
      const historyDate = new Date(h.date);
      const start = this.startDate ? new Date(this.startDate) : null;
      const end = this.endDate ? new Date(this.endDate) : null;

      return (!start || historyDate >= start) &&
        (!end || historyDate <= end);
    });
  }

}
