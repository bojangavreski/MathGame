import { Injectable } from "@angular/core";


@Injectable()
export class AuthService{

  private readonly ACCESS_TOKEN_KEY = 'access_token';

  public saveTokenInLocalStorage(token: string){
    sessionStorage.setItem(this.ACCESS_TOKEN_KEY, token);
  }

  public getAccessToken() : string{
    let token = sessionStorage.getItem(this.ACCESS_TOKEN_KEY);
    if(token) return token;
    return '';
  }

  public isLoggedIn(): boolean {
    if(typeof(Storage) !== "undefined"){
      return sessionStorage !== undefined && sessionStorage.getItem(this.ACCESS_TOKEN_KEY) !== null
    }
    return false;
  }
}
