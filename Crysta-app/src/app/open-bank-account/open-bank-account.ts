import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-open-bank-account',
  standalone: true,
  imports: [CommonModule, HttpClientModule, RouterModule, FormsModule],
  templateUrl: './open-bank-account.html',
  styleUrl: './open-bank-account.css'
})
export class OpenBankAccountComponent implements OnInit {
  accountData = {
    userId: 0,
    account_Type: '',
    account_Status: '',
    currency: ''
  };
  users: any[] = [];
  message: string = '';
  errorMessage: string = '';

  constructor(private http: HttpClient, private router: Router) { }

  ngOnInit() {
    this.http.get('http://localhost:5146/api/users/getall').subscribe({
      next: (data) => {
        console.log('Users loaded:', data);
        this.users = data as any[];
      },
      error: (error) => {
        console.error('Failed to load users:', error);
        this.message = 'Error loading users. Make sure you are logged in as an administrator.';
      }
    });
  }

  openAccount() {
    const url = 'http://localhost:5146/api/dim_account/create-account';

    this.http.post(url, this.accountData).subscribe({
      next: (response) => {
        this.message = 'Account successfully created!';
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = err.message || 'Failed to create account.';
      }
    });
  }
}

