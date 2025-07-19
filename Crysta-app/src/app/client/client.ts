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
  // ... other fields
}
@Component({
  selector: 'app-client',
  standalone: true,
  imports: [CommonModule, HttpClientModule, RouterModule, FormsModule],
  templateUrl: './client.html',
  styleUrls: ['./client.css']
})
export class ClientComponent implements OnInit {
  userInfo: any = null;
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
  }

  goToEdit() {
    this.router.navigate(['/client/edit-profile']);
  }

  goToDeposits() {
    this.router.navigate(['/client/deposits']);
  }

  goToMakeTransfer() {
    this.router.navigate(['/client/make-transfer']);
  }

  goToNotifications() {
    this.router.navigate(['/client/notifications']);
  }

  goToStatistics() {
    this.router.navigate(['/client/statistics']);
  }

 goToTransactions() {
    this.router.navigate(['/client/transactions']);
  }

 goToUpdatePassword() {
    this.router.navigate(['/client/update-password']);
  }
}


