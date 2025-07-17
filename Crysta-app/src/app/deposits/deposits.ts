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
  selector: 'app-deposits',
  standalone: true,
  imports: [CommonModule, HttpClientModule, RouterModule, FormsModule],
  templateUrl: './deposits.html',
  styleUrls: ['./deposits.css']
})
export class DepositsComponent implements OnInit {
  balances: any[] = [];
  transactions: any[] = [];
  deposit = {
    accountId: null,
    amount: null,
    channel: 'Web',
    status: 'Processed',
  };

  constructor(private http: HttpClient, private router: Router) { }

  ngOnInit(): void {
    console.log('ngOnInit called');
    this.loadBalances();
    this.loadTransactions();
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

  goToClient() {
    this.router.navigate(['/client']);
  }

}


