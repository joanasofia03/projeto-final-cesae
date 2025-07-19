import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-manage-bank-account',
  standalone: true,
  imports: [CommonModule, HttpClientModule, RouterModule, FormsModule],
  templateUrl: './manage-bank-account.html',
  styleUrl: './manage-bank-account.css'
})
export class ManageBankAccountComponent implements OnInit {
  accounts: any[] = [];
  successMessage: string = '';
  errorMessage: string = '';

  constructor(private http: HttpClient, private router: Router) { }

  ngOnInit(): void {
    this.loadAccounts();
  }

  loadAccounts() {
    this.http.get<any[]>('http://localhost:5146/api/dim_account/getall').subscribe({
      next: async data => {
        this.accounts = data;
        
        const nameFetches = this.accounts.map(account =>
          this.http.get<any>(`http://localhost:5146/api/users/${account.appUser_ID}`).toPromise()
            .then(user => account.ownerName = user.fullName)
            .catch(() => account.ownerName = 'Unknown')
        );

        await Promise.all(nameFetches);
        console.log('Accounts loaded:', this.accounts);
      },
      error: err => {
        console.error(err);
        this.errorMessage = 'Failed to load accounts.';
      }
    });
  }

  updateAccount(account: any) {
    const url = `http://localhost:5146/api/dim_account/update/${account.account_ID}`;
    const body = {
      account_Type: account.account_Type,
      account_Status: account.account_Status
    };

    this.http.put(url, body).subscribe({
      next: () => this.successMessage = `Account ${account.account_ID} updated.`,
      error: err => {
        console.error(err);
        this.errorMessage = `Failed to update account ${account.account_ID}.`;
      }
    });
  }

  toggleStatus(account: any) {
    account.account_Status = account.account_Status === 'Active' ? 'Inactive' : 'Active';
    this.updateAccount(account);
  }

  deleteAccount(accountId: number) {
    if (!confirm('Are you sure you want to delete this account?')) return;

    const url = `http://localhost:5146/api/dim_account/delete/${accountId}`;
    this.http.delete(url).subscribe({
      next: () => {
        this.successMessage = `Account ${accountId} deleted.`;
        this.accounts = this.accounts.filter(a => a.ID !== accountId);
      },
      error: err => {
        console.error(err);
        this.errorMessage = `Could not delete account ${accountId}. ${err.error}`;
      }
    });
  }
}

