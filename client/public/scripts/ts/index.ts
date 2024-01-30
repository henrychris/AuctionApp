import { login, register } from "./auth.js";

let loginRes;

SetEventListeners();

function SetEventListeners() {
  document
    .getElementById("toggleToRegisterButton")!
    .addEventListener("click", toggleForm);

  document
    .getElementById("toggleToLoginButton")!
    .addEventListener("click", toggleForm);

  document
    .getElementById("loginButton")!
    .addEventListener("click", loginInternal);

  window.addEventListener("keyup", function (event) {
    if (event.key === "Enter") {
      event.preventDefault();
      loginInternal();
    }
  });

  document
    .getElementById("registerButton")!
    .addEventListener("click", registerInternal);
}

function toggleForm() {
  const loginScreen = document.getElementById("loginScreen")!;
  const registerScreen = document.getElementById("registerScreen")!;

  if (loginScreen.style.display === "none") {
    loginScreen.style.display = "block";
    registerScreen.style.display = "none";
  } else {
    loginScreen.style.display = "none";
    registerScreen.style.display = "block";
  }
}

async function loginInternal() {
  const username = (document.getElementById("loginEmail") as HTMLInputElement)
    .value;
  const password = (
    document.getElementById("loginPassword") as HTMLInputElement
  ).value;

  if (!username || !password) {
    console.error("Username and password are required!");
    return;
  }

  try {
    loginRes = await login(username, password);
    console.log(loginRes);
    window.location.href = "./pages/rooms.html";
  } catch (error) {
    console.error("Error during login:", error);
  }
}

async function registerInternal() {
  const firstName = (
    document.getElementById("registerFirstName") as HTMLInputElement
  ).value;
  const lastName = (
    document.getElementById("registerLastName") as HTMLInputElement
  ).value;
  const email = (document.getElementById("registerEmail") as HTMLInputElement)
    .value;
  const password = (
    document.getElementById("registerPassword") as HTMLInputElement
  ).value;

  if (!firstName || !lastName || !email || !password) {
    console.error("All fields are required!");
    return;
  }

  try {
    loginRes = await register(firstName, lastName, email, password);
    console.log(loginRes);
  } catch (error) {
    console.error("Error during registration:", error);
  }
}
