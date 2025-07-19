import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { jwtDecode } from 'jwt-decode';
import { Router } from '@angular/router';

@Component({
  selector: 'app-update-password',
  standalone: true,
  imports: [CommonModule, HttpClientModule, RouterModule, FormsModule],
  templateUrl: './update-password.html',
  styleUrl: './update-password.css'
})
export class UpdatePasswordComponent implements OnInit {
  userId: number | null = null;

  currentPassword: string = '';
  newPassword: string = '';
  confirmPassword: string = '';

  errorMessage: string = '';
  successMessage: string = '';

  constructor(private http: HttpClient, private router: Router) { }

  ngOnInit(): void {
    const token = localStorage.getItem('auth_token');
    if (token) {
      const decoded: any = jwtDecode(token);
      this.userId = parseInt(decoded.nameid || decoded.sub);
    }
  }

  updatePassword(): void {
    if (!this.userId) {
      this.errorMessage = 'User not authenticated.';
      return;
    }

    if (this.newPassword !== this.confirmPassword) {
      this.errorMessage = 'New passwords do not match.';
      return;
    }

    const url = `http://localhost:5146/api/users/update-password/${this.userId}`;
    const body = {
      currentPassword: this.currentPassword,
      newPassword: this.newPassword
    };

    this.http.put(url, body).subscribe({
      next: (res: any) => {
        this.successMessage = res.message;
        this.errorMessage = '';
        this.currentPassword = '';
        this.newPassword = '';
        this.confirmPassword = '';
      },
      error: (err) => {
        if (typeof err.error === 'string') {
          this.errorMessage = err.error;
        } else if (err.error?.errors) {
          // ModelState-style error
          const messages = Object.values(err.error.errors).flat();
          this.errorMessage = messages.join(', ');
        } else {
          this.errorMessage
            = 'An unexpected error occurred.';
        }
        this.successMessage = '';
      }
    });
  }
}

