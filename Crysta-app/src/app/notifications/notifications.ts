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
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule, HttpClientModule, RouterModule, FormsModule],
  templateUrl: './notifications.html',
  styleUrls: ['./notifications.css']
})
export class NotificationsComponent implements OnInit {
  notifications: any[] = [];
  pageSize = 4;
  currentPage = 1;

  constructor(private http: HttpClient, private router: Router) { }

  ngOnInit(): void {
    console.log('ngOnInit called');
    this.loadNotifications();
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

  get pagedNotifications() {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    return this.notifications.slice(startIndex, startIndex + this.pageSize);
  }

  get totalPages() {
    return Math.ceil(this.notifications.length / this.pageSize);
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  nextPage() {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
    }
  }

  prevPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }

  goToClient() {
    this.router.navigate(['/client']);
  }

}


