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
  selector: 'app-make-transfer',
  standalone: true,
  imports: [CommonModule, HttpClientModule, RouterModule, FormsModule],
  templateUrl: './make-transfer.html',
  styleUrls: ['./make-transfer.css']
})
export class MakeTransferComponent implements OnInit {
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
    this.loadBalances();
    this.loadTransactions();
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
      Transaction_Type_ID: 1
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

  goToClient() {
    this.router.navigate(['/client']);
  }

}


