import { joinRoom, makeBid } from "./helper.js";
import { login } from "./auth.js";

export function showChatScreen(connection) {
  document.getElementById("loginScreen").style.display = "none";
  document.getElementById("roomListScreen").style.display = "none";
  document.getElementById("chatScreen").style.display = "block";

  document
    .getElementById("sendMessageButton")
    .addEventListener("click", SendMessageToRoom(connection));
  document
    .getElementById("bidButton")
    .addEventListener("click", bid(connection));
  document
    .getElementById("leaveRoom")
    .addEventListener("click", leaveRoomInternal);
}

// Function to show room list screen
export function showRoomListScreen(username, connection) {
  document.getElementById("loginScreen").style.display = "none";
  document.getElementById("roomListScreen").style.display = "block";
  document.getElementById("chatScreen").style.display = "none";

  // Display the logged-in username
  document.getElementById("loggedInUsername").textContent = username;
  document
    .getElementById("joinRoom")
    .addEventListener("click", joinRoomInternal(connection));
}

async function joinRoomInternal(connection) {
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
}

function SendMessageToRoom(connection) {
  const message = document.getElementById("messageInput").value;
  const roomId = document.getElementById("roomId").value;
  if (message && roomId) {
    connection.invoke("SendMessageToRoom", roomId, message);
    document.getElementById("messageInput").value = "";
    document.getElementById("messageInput").focus();
  }
}

async function bid(connection) {
  const bid = document.getElementById("bidInput").value;
  const roomId = document.getElementById("roomId").value;
  console.log(`bid: ${bid}`);
  console.log(`roomId: ${roomId}`);
  if (bid && roomId) {
    await makeBid(roomId, connection.connectionId, bid, loginRes.accessToken);
    document.getElementById("bidInput").value = "";
  }
}

async function leaveRoomInternal() {
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
}
