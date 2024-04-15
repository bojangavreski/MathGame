import { Component, OnInit } from '@angular/core';
import { HubConnection } from '@microsoft/signalr';
import { AnswerExpressionRequest, AnswerExpressionResponse, MathExpressionDto, MathExpressionResponse, MathExpressionResultOutcome } from '../../../models/models';
import { HubService } from '../../../services/hub.service';
import { ApiService } from '../../../services/api.service';

@Component({
  selector: 'app-game-session',
  templateUrl: './game-session.component.html',
  styleUrl: './game-session.component.scss'
})
export class GameSessionComponent implements OnInit{

  private connection: HubConnection;
  public expressions : MathExpressionDto[] = [];
  userScore: number;
  activePlayersInGame: number;

  constructor(private _hubService: HubService,
              private _apiService: ApiService) {
    this.connection = this._hubService.getHubConnection();
  }

  ngOnInit(): void {
    this.joinGameSession();
    this.subscribeToSignalrEvents();
  }

  private subscribeToSignalrEvents() {
    this.mathExpressionProviderSubscription();
    this.missedExpressionNotification();
  }


  missedExpressionNotification(){
    this._hubService.$missedNotificationObservable.subscribe({
      next: response => {
        this.handleMissedNotification(response);
      }
    })
  }

  handleMissedNotification(response: string) {
    let expressionUid = JSON.parse(response).MathExpressionUid;

    let itemIndex = this.expressions.findIndex(x => x.uid === expressionUid);
    let item = this.expressions[itemIndex];
    let updatedList = this.expressions;

    updatedList[itemIndex] = {
      ...item, result: MathExpressionResultOutcome.Wrong
    }

    this.expressions = updatedList;
  }

  mathExpressionProviderSubscription(){
    this._hubService.$mathExpressionObservable.subscribe({
      next: response => {
        this.handleUpdateExpressions(response);
      }
    })
  }
  handleUpdateExpressions(response: string) {

    let parsedMathExpressionResponse = JSON.parse(response);

    if(!this.expressions.some(x => x.uid === parsedMathExpressionResponse.ExpressionUid)){
      this.expressions.push(this.mapMathExpressionResponseToDto(parsedMathExpressionResponse));
    }

  }

  answerExpression(expressionUid: string, providedAnswer: boolean){
    console.log(`${expressionUid} ${providedAnswer}`);

    let answerExpressionRequest = this.createAnswerRequest(expressionUid, providedAnswer);

    this._apiService.answerExpression(answerExpressionRequest).subscribe({
      next: (response: AnswerExpressionResponse) => {
        this.updateAnsweredExpression(response, providedAnswer);
      }
    })
  }

  private updateAnsweredExpression(answerResponse: AnswerExpressionResponse, providedAnswer: boolean){
    let itemIndex = this.expressions.findIndex(x => x.uid === answerResponse.expressionUid);
    let updatedList = this.expressions;

    updatedList[itemIndex] = {
      uid: answerResponse.expressionUid,
      mathExpression: this.expressions[itemIndex].mathExpression,
      answer: providedAnswer,
      result: answerResponse.mathExpressionResultOutcome
    }

    this.expressions = updatedList;

    if(answerResponse.mathExpressionResultOutcome === MathExpressionResultOutcome.Correct){
      this.userScore++;
    }
  }

  private createAnswerRequest(expressionUid: string, providedAnswer: boolean) : AnswerExpressionRequest{
    return {
      expressionUid: expressionUid,
      providedAnswer: providedAnswer
    } as AnswerExpressionRequest;
  }

  private joinGameSession(){
    this._apiService.joinGameSession().subscribe(response => {
      this.userScore = response.currentUserScore,
      this.activePlayersInGame = response.activePlayersInGame
    });
  }

  private mapMathExpressionResponseToDto(mathExpressionResponse: any): MathExpressionDto{

    return {
      uid: mathExpressionResponse.ExpressionUid,
      mathExpression: mathExpressionResponse.MathExpressionDisplay
    } as MathExpressionDto;
  }

}
