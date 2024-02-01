var __require = (id) => {
  return import.meta.require(id);
};

// public/scripts/ts/helper.ts
async function postData(url = "", data = {}) {
  const response = await fetch(url, {
    method: "POST",
    mode: "cors",
    cache: "no-cache",
    headers: {
      "Content-Type": "application/json"
    },
    redirect: "follow",
    referrerPolicy: "no-referrer",
    body: JSON.stringify(data)
  });
  return response.json();
}
async function PostDataWithToken(url = "", data = {}, token) {
  const response = await fetch(url, {
    method: "POST",
    mode: "cors",
    cache: "no-cache",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`
    },
    redirect: "follow",
    referrerPolicy: "no-referrer",
    body: JSON.stringify(data)
  });
  return response.json();
}
async function PostDataWithTokenNoRes(url = "", data = {}, token) {
  const response = await fetch(url, {
    method: "POST",
    mode: "cors",
    cache: "no-cache",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`
    },
    redirect: "follow",
    referrerPolicy: "no-referrer",
    body: JSON.stringify(data)
  });
  return response;
}
async function GetDataWithToken(url = "", token) {
  const response = await fetch(url, {
    method: "GET",
    mode: "cors",
    cache: "no-cache",
    headers: {
      Authorization: `Bearer ${token}`
    },
    redirect: "follow",
    referrerPolicy: "no-referrer"
  });
  return response.json();
}

// public/scripts/ts/config.ts
var BASE_URL = "http://localhost:5000/api";
var BASE_URL_SIGNALR = "http://localhost:5000/auctionHub";
var ADMIN_EMAIL = "test@email.com";
var ADMIN_PASSWORD = "testPassword123@";
var USER_EMAIL = "test2@hotmail.com";
var USER_PASSWORD = "testPassword123@";

// public/scripts/ts/auth.ts
async function login(email, password) {
  const res = await postData(`${BASE_URL}/auth/login`, {
    emailAddress: email,
    password
  });
  if (res.success) {
    localStorage.setItem("loginRes", JSON.stringify(res.data));
    return res.data;
  }
}
async function register(firstName, lastName, email, password) {
  const role = "User";
  const res = await postData(`${BASE_URL}/auth/register`, {
    firstName,
    lastName,
    emailAddress: email,
    password,
    role
  });
  if (res.success) {
    localStorage.setItem("loginRes", JSON.stringify(res.data));
    return res.data;
  }
}
export {
  register,
  login
};
