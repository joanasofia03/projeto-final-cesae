import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login';
import { ClientComponent } from './client/client';
import { AdminComponent } from './admin/admin';
import { RegisterComponent } from './register/register';
import { EditProfileComponent } from './edit-profile/edit-profile';
import { MarketAssetsComponent } from './market-assets/market-assets';
import { MainPageComponent } from './mainpage/mainpage';
import { DepositsComponent } from './deposits/deposits';
import { MakeTransferComponent } from './make-transfer/make-transfer';
import { TransactionsComponent } from './transactions/transactions';
import { StatisticsComponent } from './statistics/statistics';
import { NotificationsComponent } from './notifications/notifications';
import { OpenBankAccountComponent } from './open-bank-account/open-bank-account';
import { ManageBankAccountComponent } from './manage-bank-account/manage-bank-account';
import { NotAuthorizedComponent } from './not-authorized/not-authorized';
import { UpdatePasswordComponent } from './update-password/update-password';
import { ManageUsersComponent } from './manage-users/manage-users';
import { AuthGuard } from './guards/auth-guard';

export const routes: Routes = [
  { path: '', component: MainPageComponent },
  { path: 'login', component: LoginComponent },
  { path: 'not-authorized', component: NotAuthorizedComponent },
  { path: 'market-assets', component: MarketAssetsComponent },
  {
    path: 'client',
    component: ClientComponent,
    canActivate: [AuthGuard],
    data: { role: 'Client' },
  },
  {
    path: 'client/edit-profile', component: EditProfileComponent,
    canActivate: [AuthGuard],
    data: { role: 'Client' },
  },
  {
    path: 'client/update-password', component: UpdatePasswordComponent,
    canActivate: [AuthGuard],
    data: { role: 'Client' },
  },
  {
    path: 'client/make-transfer', component: MakeTransferComponent,
    canActivate: [AuthGuard],
    data: { role: 'Client' },
  },
  {
    path: 'client/deposits', component: DepositsComponent,
    canActivate: [AuthGuard],
    data: { role: 'Client' },
  },
  {
    path: 'client/transactions', component: TransactionsComponent,
    canActivate: [AuthGuard],
    data: { role: 'Client' },
  },
  {
    path: 'client/statistics', component: StatisticsComponent,
    canActivate: [AuthGuard],
    data: { role: 'Client' },
  },
  {
    path: 'client/notifications', component: NotificationsComponent,
    canActivate: [AuthGuard],
    data: { role: 'Client' },
  },
  {
    path: 'admin', component: AdminComponent,
    canActivate: [AuthGuard],
    data: { role: 'Administrator' },
  },
  {
    path: 'admin/open-bank-account', component: OpenBankAccountComponent,
    canActivate: [AuthGuard],
    data: { role: 'Administrator' },
  },
  {
    path: 'admin/manage-bank-account', component: ManageBankAccountComponent,
    canActivate: [AuthGuard],
    data: { role: 'Administrator' },
  },
  {
    path: 'admin/manage-user-account', component: ManageUsersComponent,
    canActivate: [AuthGuard],
    data: { role: 'Administrator' },
  },
  {
    path: 'admin/update-password', component: UpdatePasswordComponent,
    canActivate: [AuthGuard],
    data: { role: 'Administrator' },
  },
  { path: 'register', component: RegisterComponent },
  // fallback
  { path: '**', redirectTo: '' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule { }
