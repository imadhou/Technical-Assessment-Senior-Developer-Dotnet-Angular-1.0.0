import { ValidationProblemDetails } from "./validation-problem-details.model";

export interface ProblemDetails {
  type?: string | null;
  title?: string | null;
  status?: number | null;
  detail?: string | null;
  instance?: string | null;
  [key: string]: any;
  details: ValidationProblemDetails
}
