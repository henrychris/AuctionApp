"use strict";
(() => {
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

  // public/scripts/ts/config.ts
  var BASE_URL = "http://localhost:5000/api";

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
})();
