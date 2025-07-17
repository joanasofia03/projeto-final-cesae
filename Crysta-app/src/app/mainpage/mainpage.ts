import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-main-page',
  templateUrl: './mainpage.html',
  styleUrl: './mainpage.css'
})
export class MainPageComponent {
  constructor(private router: Router) {}

  goToLogin() {
    this.router.navigate(['/login']);
  }

  goToRegister() {
    this.router.navigate(['/register']);
  }

  goToMarketAssets() {
    this.router.navigate(['/market-assets']);
  }
}
