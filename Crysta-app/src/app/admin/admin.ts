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
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, HttpClientModule, RouterModule, FormsModule],
  templateUrl: './admin.html',
  styleUrls: ['./admin.css']
})
export class AdminComponent implements OnInit {
  userInfo: any;
  userError: string | null = null;

  constructor(private http: HttpClient, private router: Router) { }

  ngOnInit() {
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

  goToOpenBankAccount() {
    this.router.navigate(['/admin/open-bank-account']);
  }

  goToManageBankAccount() {
    this.router.navigate(['/admin/manage-bank-account']);
  }

  goToManageUserAccount() {
    this.router.navigate(['/admin/manage-user-account']);
  }

  goToUpdatePassword() {
    this.router.navigate(['/admin/update-password']);
  }

}
