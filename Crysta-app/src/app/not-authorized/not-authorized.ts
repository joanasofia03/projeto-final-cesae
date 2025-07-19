import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-not-authorized',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './not-authorized.html',
  styleUrl: './not-authorized.css'
})
export class NotAuthorizedComponent {

  constructor(private router: Router) { }

  goToHome() {
    this.router.navigate(['/']);
  }
}

