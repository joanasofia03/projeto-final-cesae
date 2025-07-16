import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-edit-profile',
   standalone: true,
  imports: [CommonModule, HttpClientModule, RouterModule, FormsModule],
  templateUrl: './edit-profile.html',
  styleUrls: ['./edit-profile.css'],
})
export class EditProfileComponent implements OnInit {
  userId: number | null = null;
  userData = {
    fullName: '',
    email: '',
    phoneNumber: '',
    region: '',
    birthDate: ''
  };

  message: string = '';
  error: string = '';

  constructor(private http: HttpClient, private router: Router) {}

  ngOnInit(): void {
    // Get current user data
    this.http.get<any>('http://localhost:5146/api/users/me').subscribe({
      next: (res) => {
        this.userId = res.id;
        this.userData = {
          fullName: res.fullName || '',
          email: res.email || '',
          phoneNumber: res.phoneNumber || '',
          region: res.region || '',
          birthDate: res.birthDate?.substring(0, 10) || '' // format for input[type=date]
        };
      },
      error: (err) => {
        this.error = 'Failed to load user profile.';
      }
    });
  }

  updateProfile(): void {
    if (!this.userId) {
      this.error = 'User ID not available.';
      return;
    }

    this.http.put(`http://localhost:5146/api/users/update-user/${this.userId}`, this.userData).subscribe({
      next: (res: any) => {
        this.message = res.message || 'User updated successfully.';
        this.error = '';
      },
      error: (err) => {
      // If backend returns a plain string or model state errors
      if (typeof err.error === 'string') {
        this.error = err.error;
      } else if (err.error?.errors) {
        // ModelState-style error
        const messages = Object.values(err.error.errors).flat();
        this.error = messages.join(', ');
      } else {
        this.error = 'An unexpected error occurred.';
      }
      this.message = '';
    }
  });
}

  goToClientPage() {
    this.router.navigate(['/client']);
  }
}
