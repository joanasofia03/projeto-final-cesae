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
  selector: 'app-client',
  standalone: true,
  imports: [CommonModule, HttpClientModule, RouterModule, FormsModule],
  templateUrl: './client.html',
  styleUrls: ['./client.css']
})
export class ClientComponent implements OnInit {
  balances: any[] = [];
  transactions: any[] = [];
  notifications: any[] = [];
  deposit = {
    accountId: null,
    amount: null,
    channel: 'Web',
    status: 'Processed',
  };
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
  userInfo: any = null;
  balanceEvolution: { date: string; balance: number }[] = [];
  transactionError: string | null = null;
  totalBalance: number | null = null;
  totalBalanceError: string | null = null;
  averageSpending: number | null = null;
  monthlyBreakdown: any[] = [];
  averageSpendingError: string | null = null;
  userError: string | null = null;

  constructor(private http: HttpClient, private router: Router) { }

  ngOnInit(): void {
    console.log('ngOnInit called');
    this.http.get('http://localhost:5146/api/users/me').subscribe({
      next: (res) => {
        this.userInfo = res;
      },
      error: (err) => {
        this.userError = 'Failed to load user info.';
      }
    });
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
    this.loadNotifications();
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

  loadNotifications() {
    this.http.get<any[]>('http://localhost:5146/api/fact_notifications/my-notifications')
      .subscribe({
        next: data => {
          console.log('Notifications loaded:', data);
          this.notifications = data;
        },
        error: err => console.error('Failed to load notifications:', err)
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

  makeDeposit() {
    if (!this.deposit.accountId || !this.deposit.amount || this.deposit.amount <= 0) {
      alert("Please select a valid account and enter a positive amount.");
      return;
    }

    const token = localStorage.getItem('auth_token');
    if (!token) {
      alert("User is not authenticated.");
      return;
    }

    const decoded = jwtDecode<JwtPayload>(token);
    const userId = parseInt(decoded.nameid, 10);
    if (isNaN(userId)) {
      alert("Invalid user ID in token.");
      return;
    }

    const depositDto = {
      AppUser_ID: userId,
      Source_Account_ID: this.deposit.accountId,
      Destination_Account_ID: this.deposit.accountId,
      Transaction_Amount: this.deposit.amount,
      Execution_Channel: this.deposit.channel,
      Transaction_Status: this.deposit.status
    };

    this.http.post('http://localhost:5146/api/fact_transactions/deposit', depositDto)
      .subscribe({
        next: (res) => {
          alert('Deposit successful!');
          console.log('Deposit:', depositDto);
          this.loadBalances();
          this.loadTransactions();
        },
        error: (err) => {
          console.error('Deposit failed', err);
          alert('Deposit failed: ' + (err.error?.Message || 'Unknown error'));
        }
      });


  }

  makeTransaction() {
    if (
      !this.transaction.sourceAccountId ||
      !this.transaction.destinationAccountId ||
      !this.transaction.amount ||
      this.transaction.amount <= 0
    ) {
      alert("Please fill all fields and enter a valid amount.");
      return;
    }

    if (this.transaction.sourceAccountId === this.transaction.destinationAccountId) {
      alert("Source and destination accounts must be different.");
      return;
    }

    const token = localStorage.getItem('auth_token');
    if (!token) {
      alert("User is not authenticated.");
      return;
    }

    const decoded = jwtDecode<JwtPayload>(token);
    const userId = parseInt(decoded.nameid, 10);
    if (isNaN(userId)) {
      alert("Invalid user ID in token.");
      return;
    }

    const transferDto = {
      AppUser_ID: userId,
      Source_Account_ID: this.transaction.sourceAccountId,
      Destination_Account_ID: this.transaction.destinationAccountId,
      Transaction_Amount: this.transaction.amount,
      Execution_Channel: this.transaction.channel,
      Transaction_Status: this.transaction.status,
      Transaction_Type_ID: 2
    };

    this.http.post('http://localhost:5146/api/fact_transactions/create-transaction', transferDto)
      .subscribe({
        next: (res) => {
          alert('Transaction successful!');
          this.loadBalances();
          this.loadTransactions();
        },
        error: (err) => {
          console.error('Deposit failed', err);

          const backendError = err.error?.error || 'Unknown Error';
          const backendMessage = err.error?.message || 'An unknown error occurred.';

          alert(`Deposit failed: ${backendError} - ${backendMessage}`);
        }
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

  goToEdit() {
    this.router.navigate(['/client/edit-profile']);
  }

}


