import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-manage-users',
  standalone: true,
  templateUrl: './manage-users.html',
  styleUrl: './manage-users.css',
  imports: [CommonModule, FormsModule, HttpClientModule, RouterModule]
})
export class ManageUsersComponent implements OnInit {
  users: any[] = [];
  filteredUsers: any[] = [];

  searchUserId: string = '';
  searchName: string = '';

  successMessage: string = '';
  errorMessage: string = '';

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.http.get<any[]>('http://localhost:5146/api/users/getall').subscribe({
      next: data => {
        this.users = data.filter(user => user.fullName !== 'Administrator');
        this.filteredUsers = [...this.users];
        console.log('Found users: ', this.filteredUsers);
      },
      error: err => {
        this.errorMessage = err.error || 'Failed to load users.';
      }
    });
  }

  filterUsers(): void {
    const idFilter = this.searchUserId.toLowerCase();
    const nameFilter = this.searchName.toLowerCase();

    this.filteredUsers = this.users.filter(user => {
      const idMatch = user.id.toString().includes(idFilter);
      const nameMatch = (user.fullName || '').toLowerCase().includes(nameFilter);
      return idMatch && nameMatch;
    });
  }

  deleteUser(userId: number): void {
    if (!confirm(`Are you sure you want to delete user ID ${userId}?`)) return;

    this.http.delete(`http://localhost:5146/api/users/delete/${userId}`).subscribe({
      next: (res: any) => {
        this.successMessage = res.message;
        this.errorMessage = '';
        this.users = this.users.filter(user => user.id !== userId);
        this.filterUsers();
      },
      error: err => {
        this.successMessage = '';
        this.errorMessage = err.error || 'Could not delete user.';
      }
    });
  }
}
