import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GameSessionComponent } from './components/game-session/game-session/game-session.component';
import { SignInComponent } from './components/auth/sign-in/sign-in.component';
import { SignUpComponent } from './components/auth/sign-up/sign-up.component';
import { authGuard } from './guards/AuthGuard';

const routes: Routes = [
  {path: 'login', component: SignInComponent},
  {path: 'signup', component: SignUpComponent},
  {path: 'game-session', component: GameSessionComponent, canActivate: [authGuard]},
  {path: '', redirectTo: 'login', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
