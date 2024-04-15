import { HttpClient, HttpHeaders, HttpParamsOptions } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { AnswerExpressionRequest, AnswerExpressionResponse, GameSessionResponse, SignInRequest, SignInResponse, SignUpRequest, SignUpResponse } from "../models/models";
import { AuthService } from "./auth.service";

@Injectable()
export class ApiService{

  constructor(private _httpClient: HttpClient,
              private _authService: AuthService){

  }

  public signIn(signInRequest: SignInRequest): Observable<SignInResponse>{

    var content = JSON.stringify(signInRequest);

    const httpOptions = {
      headers: new HttpHeaders({'Content-Type': 'application/json'}),
    }

    return this._httpClient.post<SignInResponse>("https://localhost:44371/login", content, httpOptions);
  }

  public signUp(signUpRequest: SignUpRequest): Observable<SignUpResponse>{

    var content = JSON.stringify(signUpRequest);

    const httpOptions = {
      headers: new HttpHeaders({'Content-Type': 'application/json'}),
    }

    return this._httpClient.post<SignUpResponse>("https://localhost:44371/register", content, httpOptions);
  }

  public joinGameSession(): Observable<GameSessionResponse>{

    const token = this._authService.getAccessToken();
    const httpOptions = {
      headers: new HttpHeaders({'Content-Type': 'application/json', 'Authorization': `Bearer ${token}`}),
    }

    return this._httpClient.post<GameSessionResponse>("https://localhost:44371/GameSession/join", null, httpOptions);
  }

  public answerExpression(answerExpresisonRequest: AnswerExpressionRequest): Observable<AnswerExpressionResponse>{
    var content = JSON.stringify(answerExpresisonRequest);

    const token = this._authService.getAccessToken();
    const httpOptions = {
      headers: new HttpHeaders({'Content-Type': 'application/json', 'Authorization': `Bearer ${token}`}),
    }

    return this._httpClient.post<AnswerExpressionResponse>("https://localhost:44371/MathExpression/answer", content, httpOptions);
  }
}
