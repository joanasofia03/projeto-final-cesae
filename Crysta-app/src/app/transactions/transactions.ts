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
  selector: 'app-transactions',
  standalone: true,
  imports: [CommonModule, HttpClientModule, RouterModule, FormsModule],
  templateUrl: './transactions.html',
  styleUrls: ['./transactions.css']
})
export class TransactionsComponent implements OnInit {
  transactions: any[] = [];
  transaction = {
    sourceAccountId: null,
    destinationAccountId: null,
    amount: null,
    channel: 'Web',
    status: 'Processed'
  };
  filters = {
    from: '',
    to: '',
    type: null
  };
  transactionTypes = [
    { id: 1, label: 'Transfer' },
    { id: 2, label: 'Deposit' }
  ];
  transactionError: string | null = null;
  userError: string | null = null;

  constructor(private http: HttpClient, private router: Router) { }

  ngOnInit(): void {
    console.log('ngOnInit called');
    this.loadTransactions();
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

  filterTransactions() {
    const params: any = {};

    if (this.filters.from) params.from = this.filters.from;
    if (this.filters.to) params.to = this.filters.to;
    if (this.filters.type) params.type = this.filters.type;

    this.http.get<any[]>('http://localhost:5146/api/fact_transactions/filter', { params })
      .subscribe({
        next: (data) => {
          this.transactions = data;
          this.transactionError = null; // clear old error if any
        },
        error: (err) => {
          if (err.error?.message) {
            this.transactionError = err.error.message;
            alert(`${this.transactionError}`);
          } else {
            this.transactionError = 'An unexpected error occurred.';
            alert('An unexpected error occurred while filtering transactions.');
          }
          this.transactions = [];
        }
      });
  }


  resetFilters() {
    this.filters = { from: '', to: '', type: null };
    this.loadTransactions();
  }

  goToClient() {
    this.router.navigate(['/client']);
  }
}


