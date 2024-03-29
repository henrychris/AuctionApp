import type {
  ApiErrorResponse,
  ApiResponse,
  UserAuthResponse,
} from "./config.js";

export async function postData(
  url = "",
  data = {}
): Promise<ApiResponse<UserAuthResponse>> {
  const response = await fetch(url, {
    method: "POST",
    mode: "cors",
    cache: "no-cache",
    headers: {
      "Content-Type": "application/json",
    },
    redirect: "follow",
    referrerPolicy: "no-referrer",
    body: JSON.stringify(data),
  });

  return response.json() as unknown as ApiResponse<UserAuthResponse>;
}

export async function PostDataWithToken(url = "", data = {}, token: string) {
  const response = await fetch(url, {
    method: "POST",
    mode: "cors",
    cache: "no-cache",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    redirect: "follow",
    referrerPolicy: "no-referrer",
    body: JSON.stringify(data),
  });

  return response.json();
}

export async function PostDataWithTokenNoRes(
  url = "",
  data = {},
  token: string
) {
  const response = await fetch(url, {
    method: "POST",
    mode: "cors",
    cache: "no-cache",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    redirect: "follow",
    referrerPolicy: "no-referrer",
    body: JSON.stringify(data),
  });

  return response;
}

export async function GetDataWithToken(url = "", token: string) {
  const response = await fetch(url, {
    method: "GET",
    mode: "cors",
    cache: "no-cache",
    headers: {
      Authorization: `Bearer ${token}`,
    },
    redirect: "follow",
    referrerPolicy: "no-referrer",
  });

  return response.json();
}

export function GetErrors(error: any): string {
  const err = error as unknown as ApiErrorResponse;

  return err.errors.map((e) => e.description).join("\n");
}
