import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { SignInRequest } from '../../../models/models';
import { ApiService } from '../../../services/api.service';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrl: './sign-in.component.scss'
})
export class SignInComponent {

  public formGroup: FormGroup;

  constructor(private _signInFormBuilder: FormBuilder,
              private _apiService: ApiService,
              private _authService: AuthService,
              private _router: Router){
    this.formGroup = _signInFormBuilder.group({
      'email': [null, Validators.required],
      'password': [null, Validators.required]
    })
  }


  public signIn(){

    let request: SignInRequest = {
      email: this.formGroup.value.email,
      password: this.formGroup.value.password,
    }

    this._apiService.signIn(request).subscribe({
      next:  (response) =>{
        this._authService.saveTokenInLocalStorage(response.accessToken);
        this._router.navigateByUrl('/game-session');
      },
      error: (error) =>{
        console.log(error);
      }
    })
  }
}
