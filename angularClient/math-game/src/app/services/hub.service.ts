import { Injectable } from "@angular/core";
import { HttpTransportType, HubConnection, HubConnectionBuilder, HubConnectionState } from "@microsoft/signalr";
import { AuthService } from "./auth.service";
import { Observable, Subject } from "rxjs";


@Injectable()
export class HubService{
  private hubConnection: HubConnection;

  private mathExpressionSubject = new Subject<string>();
  $mathExpressionObservable: Observable<string> = this.mathExpressionSubject.asObservable();

  private missedNotificationSubject = new Subject<string>();
  $missedNotificationObservable: Observable<string> = this.missedNotificationSubject.asObservable();

  constructor(private _authService: AuthService){
    this.createHubConnection();
    this.startHubConnection();
  }

  public getHubConnection(){
    if(this.hubConnection && this.hubConnection.state != HubConnectionState.Disconnected){
      return this.hubConnection;
    }else{
      this.hubConnection.start().then(x => console.log("connected"));
      return this.hubConnection;
    }
  }

  private startHubConnection(){
    if(this.hubConnection && this.hubConnection.state == HubConnectionState.Connected){
      return;
    }

    this.hubConnection.start().then(() => this.registerListeners())
                              .catch(err => console.log(err));


  }

  private registerListeners() {
    this.listenForMathExpressions();
    this.listenForMissedNotification();
  }

  private listenForMathExpressions(): void {
      this.hubConnection.on('GetMathExpression', response => {
        this.mathExpressionSubject.next(response);
      });
  }

  private listenForMissedNotification(): void {
    this.hubConnection.on('NotifyMissed', response => {
      this.missedNotificationSubject.next(response);
    });
}

  private createHubConnection(){
    let token = this._authService.getAccessToken();
    this.hubConnection = new HubConnectionBuilder()
                          .withUrl('https://localhost:44371/answer', {
                            skipNegotiation: true,
                            transport: HttpTransportType.WebSockets,
                            accessTokenFactory: () => token
                          })
                          .build();
  }
}
