import { postData, leaveRoom, BASE_URL } from "./helper.js";

import {
  showChatScreen,
  showLoginScreen,
  showRoomListScreen,
} from "./showScreen.js";

async function main() {
  try {
    // Wait for the login to complete and get the token
    // loginRes = await login();
    // Start the SignalR connection after obtaining the token
    // await startSignalRConnection();
    // Add any other initialization steps here if needed
  } catch (error) {
    // console.error("Error during initialization:", error);
  }
}

async function login() {
  const res = await postData(`${BASE_URL}/auth/login`, {
    emailAddress: "test2@hotmail.com",
    password: "testPassword123@",
  });

  return res.data;
}

async function startSignalRConnection() {
  const connection = new signalR.HubConnectionBuilder()
    .withAutomaticReconnect()
    .withUrl("http://localhost:5030/auctionHub", {
      accessTokenFactory: () => loginRes.accessToken,
    })
    .build();

  connection.on("ReceiveMessage", function (msg) {
    console.log("hit");
    const messages = document.getElementById("messages");
    const user = `<span style="font-weight: bold">${msg.userName}: </span>`;
    messages.innerHTML += `<p>${user}<span>${msg.content}</span></p>`;
  });

  connection.on("AuctionStarted", function (msg) {
    console.log("hit");
    const messages = document.getElementById("messages");
    const user = `<span style="font-weight: bold">Admin: </span>`;
    messages.innerHTML += `<p>${user}<span>${msg}</span></p>`;

    const auctionStatus = document.getElementById("auctionStatusValue");
    auctionStatus.innerText = "In Progress";
  });

  connection.on("UserJoined", function (msg) {
    const messages = document.getElementById("messages");
    const user = `<span style="font-weight: bold">${msg} </span>`;
    messages.innerHTML += `<p style="color:grey">${user}has joined.</p>`;
  });

  connection.on("UserLeft", function (msg) {
    const messages = document.getElementById("messages");
    const user = `<span style="font-weight: bold">${msg} </span>`;
    messages.innerHTML += `<p style="color:grey">${user}has left.</p>`;
  });

  connection.on("BidPlaced", function (msg, amount) {
    const messages = document.getElementById("messages");
    const user = `<span style="font-weight: bold">${msg.userName}: </span>`;
    messages.innerHTML += `<p>${user}<span>${msg.content}</span></p>`;

    const highestBid = document.getElementById("highestPriceValue");
    highestBid.innerText = amount + " NGN";
  });

  try {
    // Start the SignalR connection
    await connection.start();
    console.log("SignalR connection started successfully.");
  } catch (error) {
    console.error("Error starting SignalR connection:", error);
  }
}

// The main function is called to kick off the initialization process
// main().catch(console.error);
