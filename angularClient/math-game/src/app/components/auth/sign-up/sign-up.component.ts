import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../../services/api.service';
import { SignUpRequest } from '../../../models/models';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrl: './sign-up.component.scss'
})
export class SignUpComponent {

  public formGroup : FormGroup;

  constructor(private _signInFormBuilder: FormBuilder,
    private _apiService: ApiService,
    private _router: Router,
    private _snackBar: MatSnackBar){
      this.formGroup = _signInFormBuilder.group({
        'email': [null, Validators.required],
        'password': [null, Validators.required],
        'confirmPassword': [null, Validators.required]
      }, {validator:this.confirmPasswordValidator})
    }


    signUp(){
      let signUpRequest : SignUpRequest = {
        email: this.formGroup.value.email,
        password: this.formGroup.value.password
      };

      this._apiService.signUp(signUpRequest).subscribe({
        next: (response) => {
          this._snackBar.open("You have been registered successfully", "Close");
          this._router.navigateByUrl('/login');
        }
      })
    }

    confirmPasswordValidator = (control: FormGroup) => {
      const password = control.value.password
      const confirmPassword = control.value.confirmPassword

      if (password && confirmPassword && password.value !== confirmPassword.value) {
        return { confirmPassword: true };
      }

      return null;
    }
}
