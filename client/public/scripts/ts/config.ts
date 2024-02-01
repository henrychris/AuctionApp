export const BASE_URL = "http://localhost:5000/api";
export const BASE_URL_SIGNALR = "http://localhost:5000/auctionHub";

export const ADMIN_EMAIL = "test@email.com";
export const ADMIN_PASSWORD = "testPassword123@";

export const USER_EMAIL = "test2@hotmail.com";
export const USER_PASSWORD = "testPassword123@";

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  note: string;
  data: T;
}

export interface UserAuthResponse {
  id: string;
  role: string;
  accessToken: string;
}

export interface ErrorDetail {
  code: string;
  description: string;
}

export interface ApiErrorResponse {
  success: boolean;
  message: string;
  errors: ErrorDetail[];
}
