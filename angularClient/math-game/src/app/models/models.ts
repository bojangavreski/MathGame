export interface SignInRequest{
  email: string;
  password: string;
}

export interface SignInResponse{
  accessToken: string;
}

export interface SignUpRequest extends SignInRequest{
}

export interface SignUpResponse extends SignInRequest{
}

export interface MathExpressionDto {
  uid: string,
  mathExpression: string,
  answer: boolean | undefined,
  result: MathExpressionResultOutcome | undefined
}

export interface MathExpressionResponse{
  expressionUid: string,
  mathExpressionDisplay: string,
  questionType: QuestionType
}

export interface AnswerExpressionRequest{
  expressionUid: string,
  providedAnswer: boolean
}

export interface AnswerExpressionResponse{
  expressionUid: string,
  mathExpressionResultOutcome: MathExpressionResultOutcome
}

export interface GameSessionResponse{
  uid: string,
  positionInQueue: number | undefined,
  activePlayersInGame: number,
  currentUserScore: number
}

export enum MathExpressionResultOutcome{
  Correct = 1,
  Wrong = 2,
  Missed= 3
}

export enum ResultEnum{
  Correct = 1,
  Failed = 2
}

export enum QuestionType {
  BinaryQuestion = 1,
  InputQuestion = 2
}
