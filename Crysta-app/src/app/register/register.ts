import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { UserService } from '../services/user';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './register.html',
})
export class RegisterComponent {
  userData = {
    email: '',
    fullName: '',
    phoneNumber: '',
    documentId: '',
    birthDate: '',
    region: '',
    password: ''
  };

  errorMessage: string | null = null;

  constructor(private userService: UserService, private router: Router) {}

  register() {
  this.userService.createUser(this.userData).subscribe({
  next: (res) => {
    console.log('User created:', res);
  },
  error: (err) => {
    if (err.status === 400) {
      this.errorMessage = 'Please fill in all required fields.';
    }	
    this.errorMessage = err.error || 'An error occurred';
  }
});
}

}
