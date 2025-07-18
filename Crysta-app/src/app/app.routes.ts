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

export const routes: Routes = [
  { path: '', component: MainPageComponent },
  { path: 'login', component: LoginComponent },
  { path: 'market-assets', component: MarketAssetsComponent },
  { path: 'client', component: ClientComponent },
  { path: 'client/edit-profile', component: EditProfileComponent },
  { path: 'client/make-transfer', component: MakeTransferComponent },
  { path: 'client/deposits', component: DepositsComponent },
  { path: 'client/transactions', component: TransactionsComponent },
  { path: 'client/statistics', component: StatisticsComponent },
  { path: 'client/notifications', component: NotificationsComponent },
  { path: 'admin', component: AdminComponent },
  { path: 'admin/open-bank-account', component: OpenBankAccountComponent },
  { path: 'register', component: RegisterComponent },
  // fallback
  { path: '**', redirectTo: '' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
