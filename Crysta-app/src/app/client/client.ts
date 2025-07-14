import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-client',
  standalone: true,
  imports: [CommonModule, HttpClientModule],
  templateUrl: './client.html',
  styleUrls: ['./client.css']
})
export class ClientComponent implements OnInit {
  balances: any[] = [];
  transactions: any[] = [];
  notifications: any[] = [];

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    console.log('ngOnInit called');
    this.loadBalances();
    this.loadTransactions();
    this.loadNotifications();
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
}
