export interface ValidationProblemDetails{
  errors?: [{propertyName: string, errorMessage: string}];
  message: string;
}
