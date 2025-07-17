import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { jwtDecode } from 'jwt-decode';
import { Router } from '@angular/router';
interface JwtPayload {
  nameid: string;
  // ... other fields if you want
}
@Component({
  selector: 'app-statistics',
  standalone: true,
  imports: [CommonModule, HttpClientModule, RouterModule, FormsModule],
  templateUrl: './statistics.html',
  styleUrls: ['./statistics.css']
})
export class StatisticsComponent implements OnInit {
  balances: any[] = [];
  transactions: any[] = [];
  balanceEvolution: { date: string; balance: number }[] = [];
  totalBalance: number | null = null;
  totalBalanceError: string | null = null;
  averageSpending: number | null = null;
  monthlyBreakdown: any[] = [];
  averageSpendingError: string | null = null;

  constructor(private http: HttpClient, private router: Router) { }

  ngOnInit(): void {
    console.log('ngOnInit called');
    this.loadBalances();
    this.loadTransactions();
    this.http.get<any[]>('http://localhost:5146/api/analytics/balance-evolution')
    .subscribe({
      next: (data) => {
        this.balanceEvolution = data;
      },
      error: (err) => {
        console.error('Failed to load balance evolution:', err);
      }
    });
    this.loadTotalBalance();
    this.loadAverageSpending();
  }

  loadBalances() {
    this.http.get<any[]>('http://localhost:5146/api/dim_account/my-balance')
      .subscribe({
        next: data => { console.log('Balances loaded:', data); this.balances = data; },
        error: err => console.error('Failed to load balances:', err)
      });
  }

  loadTransactions() {
    console.log('loadTransactions called');
    this.http.get<any[]>('http://localhost:5146/api/fact_transactions/getmine')
      .subscribe({
        next: data => {
          console.log('Transactions loaded:', data);
          this.transactions = data;
        },
        error: err => console.error('Failed to load transactions:', err)
      });
  }

  loadTotalBalance() {
    console.log('Fetching total balance...');

    this.http.get<{ totalBalance: number }>('http://localhost:5146/api/analytics/total-balance')
      .subscribe({
        next: data => {
          console.log('Total balance response:', data);
          this.totalBalance = data.totalBalance;
          this.totalBalanceError = null;
        },
        error: err => {
          console.error('Error fetching total balance:', err);
          this.totalBalance = null;
          this.totalBalanceError = err.error?.message || 'Failed to fetch total balance.';
        }
      });
  }

  loadAverageSpending(): void {
    console.log('Loading average spending...');
    this.http.get<{ averageSpending: number, monthlyBreakdown: any[] }>(
      'http://localhost:5146/api/analytics/average-spending'
    ).subscribe({
      next: data => {
        console.log('Average spending response:', data);
        this.averageSpending = data.averageSpending;
        this.monthlyBreakdown = data.monthlyBreakdown;
        this.averageSpendingError = null;
      },
      error: err => {
        console.error('Error loading average spending:', err);
        this.averageSpending = null;
        this.monthlyBreakdown = [];
        this.averageSpendingError = err.error?.message || 'Failed to fetch average spending.';
      }
    });
  }

  goToClient() {
    this.router.navigate(['/client']);
  }

}


