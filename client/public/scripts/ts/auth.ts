import { postData } from "./helper.ts";
import { BASE_URL } from "./config.ts";
import type { ApiResponse, UserAuthResponse } from "./config.ts";

export async function login(
  email: string,
  password: string
): Promise<UserAuthResponse | undefined> {
  const res = await postData(`${BASE_URL}/auth/login`, {
    emailAddress: email,
    password: password,
  });

  if (res.success) {
    return res.data;
  }
}

export async function register(
  firstName: string,
  lastName: string,
  email: string,
  password: string
): Promise<UserAuthResponse | undefined> {
  const role = "User";

  const res = await postData(`${BASE_URL}/auth/register`, {
    firstName: firstName,
    lastName: lastName,
    emailAddress: email,
    password: password,
    role: role,
  });

  if (res.success) {
    return res.data;
  }
}
