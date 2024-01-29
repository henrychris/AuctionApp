import {
  postData,
  joinRoom,
  leaveRoom,
  makeBid,
  postDataAuth,
  BASE_URL,
} from "./helper.js";

let loginRes = { accessToken: "", role: "", id: "" };

async function main() {
  try {
    // Wait for the login to complete and get the token
    loginRes = await login();

    // Start the SignalR connection after obtaining the token
    await startSignalRConnection();

    // Add any other initialization steps here if needed
  } catch (error) {
    console.error("Error during initialization:", error);
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

  // The rest of your code remains unchanged
  document
    .getElementById("joinRoom")
    .addEventListener("click", async function () {
      const roomId = document.getElementById("roomId").value;
      const userName = document.getElementById("userName").value;
      let success = await joinRoom(
        roomId,
        connection.connectionId,
        loginRes.accessToken
      );

      if (!success) {
        console.log("failed to join");
        return;
      }
      console.log(`${userName} joined room`);

      document.getElementById("homeScreen").style.display = "none";
      document.getElementById("chatScreen").style.display = "block";
      document.getElementById("roomTitle").innerHTML = roomId;
      document.getElementById("messageInput").focus();
    });

  document
    .getElementById("sendMessageButton")
    .addEventListener("click", function (event) {
      // if (event.key === "Enter") {
      const message = document.getElementById("messageInput").value;
      console.log(`message: ${message}`);
      const roomId = document.getElementById("roomId").value;
      console.log(`roomId: ${roomId}`);
      if (message && roomId) {
        connection.invoke("SendMessageToRoom", roomId, message);
        document.getElementById("messageInput").value = "";
        document.getElementById("messageInput").focus();
      }
      // }
    });

  document
    .getElementById("leaveRoom")
    .addEventListener("click", async function () {
      let userId = loginRes.id;
      const roomId = document.getElementById("roomId").value;
      let success = await leaveRoom(
        roomId,
        connection.connectionId,
        loginRes.accessToken
      );

      if (!success) {
        console.log("failed to leave");
        return;
      }

      console.log(`${userId} left room`);

      document.getElementById("homeScreen").style.display = "flex";
      document.getElementById("chatScreen").style.display = "none";
      document.getElementById("roomId").value = "";
      document.getElementById("userName").value = "";

      // connection.stop();
    });

  document
    .getElementById("bidButton")
    .addEventListener("click", async function () {
      const bid = document.getElementById("bidInput").value;
      const roomId = document.getElementById("roomId").value;
      console.log(`bid: ${bid}`);
      console.log(`roomId: ${roomId}`);
      if (bid && roomId) {
        let success = await makeBid(
          roomId,
          connection.connectionId,
          bid,
          loginRes.accessToken
        );
        document.getElementById("bidInput").value = "";
      }
    });
}

// The main function is called to kick off the initialization process
main().catch(console.error);
