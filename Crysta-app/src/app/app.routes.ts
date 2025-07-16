import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login';
import { ClientComponent } from './client/client';
import { AdminComponent } from './admin/admin';
import { RegisterComponent } from './register/register';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'client', component: ClientComponent },
  { path: 'admin', component: AdminComponent },
  { path: 'register', component: RegisterComponent },
  // fallback
  { path: '**', redirectTo: '' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
