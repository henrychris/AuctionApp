const BASE_URL = "http://localhost:5030/api";
let TOKEN = "";
async function main() {
  TOKEN = await login();
}

main().catch(console.error);

async function postData(url = "", data = {}) {
  // Default options are marked with *
  const response = await fetch(url, {
    method: "POST",
    mode: "cors",
    cache: "no-cache",
    // credentials: "same-origin",
    headers: {
      "Content-Type": "application/json",
    },
    redirect: "follow",
    referrerPolicy: "no-referrer",
    body: JSON.stringify(data), // body data type must match "Content-Type" header
  });
  return response.json(); // parses JSON response into native JavaScript objects
}

async function postDataAuth(url = "", data = {}, token) {
  // Default options are marked with *
  const response = await fetch(url, {
    method: "POST",
    mode: "cors",
    cache: "no-cache",
    // credentials: "same-origin",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    redirect: "follow",
    referrerPolicy: "no-referrer",
    body: JSON.stringify(data), // body data type must match "Content-Type" header
  });
}

async function login() {
  const res = await postData(`${BASE_URL}/auth/login`, {
    emailAddress: "test@email.com",
    password: "testPassword123@",
  });

  return res.data.accessToken;
}

async function joinRoom(roomId, connectionId, token) {
  const res = await postDataAuth(
    `${BASE_URL}/rooms/${roomId}/join`,
    {
      connectionId: connectionId,
    },
    token
  );
}

console.log(TOKEN);
const connection = new signalR.HubConnectionBuilder()
  .withAutomaticReconnect()
  .withUrl("http://localhost:5030/auctionHub", {
    accessTokenFactory: async () => await login(),
  })
  .build();

connection.start().catch(function (err) {
  return console.error(err.toString());
});

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
