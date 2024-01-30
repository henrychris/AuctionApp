export const BASE_URL = "http://localhost:5030/api";

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
