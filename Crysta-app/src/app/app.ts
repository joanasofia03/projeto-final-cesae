import { Component, signal } from '@angular/core';
import { Router } from '@angular/router';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class AppComponent {
  protected readonly title = signal('Crysta-app');

  constructor(private router: Router) {}

  goToLogin() {
    console.log('Button clicked!');
    this.router.navigate(['/login']); // or use '/login' if your login route is configured that way
  }
}
