import { postData, joinRoom, postDataAuth, BASE_URL } from "./helper.js";

let TOKEN = "";

async function main() {
  try {
    // Wait for the login to complete and get the token
    TOKEN = await login();

    // Start the SignalR connection after obtaining the token
    await startSignalRConnection();

    // Add any other initialization steps here if needed
  } catch (error) {
    console.error("Error during initialization:", error);
  }
}

async function login() {
  const res = await postData(`${BASE_URL}/auth/login`, {
    emailAddress: "test@email.com",
    password: "testPassword123@",
  });

  return res.data.accessToken;
}

async function startSignalRConnection() {
  const connection = new signalR.HubConnectionBuilder()
    .withAutomaticReconnect()
    .withUrl("http://localhost:5030/auctionHub", {
      accessTokenFactory: () => TOKEN,
    })
    .build();

  connection.on("ReceiveMessage", function (msg) {
    console.log("hit");
    const messages = document.getElementById("messages");
    const user = `<span style="font-weight: bold">${msg.userName}: </span>`;
    messages.innerHTML += `<p>${user}<span>${msg.content}</span></p>`;
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

  try {
    // Start the SignalR connection
    await connection.start();
    console.log("SignalR connection started successfully.");
  } catch (error) {
    console.error("Error starting SignalR connection:", error);
  }

  // The rest of your code remains unchanged
  document
    .getElementById("joinRoom")
    .addEventListener("click", async function () {
      const roomId = document.getElementById("roomId").value;
      const userName = document.getElementById("userName").value;
      await joinRoom(roomId, connection.connectionId, TOKEN);
      console.log(`${userName} joined room`);

      document.getElementById("homeScreen").style.display = "none";
      document.getElementById("chatScreen").style.display = "block";
      document.getElementById("roomTitle").innerHTML = roomId;
      document.getElementById("messageInput").focus();
    });

  document
    .getElementById("messageInput")
    .addEventListener("keyup", function (event) {
      if (event.key === "Enter") {
        const message = document.getElementById("messageInput").value;
        console.log(`message: ${message}`);
        const roomId = document.getElementById("roomId").value;
        console.log(`roomId: ${roomId}`);
        if (message && roomId) {
          connection.invoke("SendMessageToRoom", roomId, message);
          document.getElementById("messageInput").value = "";
        }
      }
    });
}

// The main function is called to kick off the initialization process
main().catch(console.error);
