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

  // public/scripts/ts/index.ts
  var loginRes;
  SetEventListeners();
  function SetEventListeners() {
    document.getElementById("toggleToRegisterButton").addEventListener("click", toggleForm);
    document.getElementById("toggleToLoginButton").addEventListener("click", toggleForm);
    document.getElementById("loginButton").addEventListener("click", loginInternal);
    window.addEventListener("keyup", function(event) {
      const loginScreen = document.getElementById("loginScreen");
      const registerScreen = document.getElementById("registerScreen");
      if (event.key === "Enter") {
        if (loginScreen.style.display === "none") {
          event.preventDefault();
          registerInternal();
        } else {
          event.preventDefault();
          loginInternal();
        }
      }
    });
    document.getElementById("registerButton").addEventListener("click", registerInternal);
  }
  function toggleForm() {
    const loginScreen = document.getElementById("loginScreen");
    const registerScreen = document.getElementById("registerScreen");
    if (loginScreen.style.display === "none") {
      loginScreen.style.display = "block";
      registerScreen.style.display = "none";
    } else {
      loginScreen.style.display = "none";
      registerScreen.style.display = "block";
    }
  }
  async function loginInternal() {
    const username = document.getElementById("loginEmail").value;
    const password = document.getElementById("loginPassword").value;
    if (!username || !password) {
      console.error("Username and password are required!");
      return;
    }
    try {
      loginRes = await login(username, password);
      console.log(loginRes);
      if (loginRes) {
        window.location.href = "./pages/rooms.html";
      }
    } catch (error) {
      console.error("Error during login:", error);
    }
  }
  async function registerInternal() {
    const firstName = document.getElementById("registerFirstName").value;
    const lastName = document.getElementById("registerLastName").value;
    const email = document.getElementById("registerEmail").value;
    const password = document.getElementById("registerPassword").value;
    if (!firstName || !lastName || !email || !password) {
      console.error("All fields are required!");
      return;
    }
    try {
      loginRes = await register(firstName, lastName, email, password);
      console.log(loginRes);
      if (loginRes) {
        window.location.href = "./pages/rooms.html";
      }
    } catch (error) {
      console.error("Error during registration:", error);
    }
  }
})();
