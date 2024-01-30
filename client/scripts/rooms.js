// placeholder token generation
// store token in local storage later. or use express jeje
import { login } from "./auth.js";
import { BASE_URL } from "./config.js";
import { GetDataWithToken } from "./helper.js";

const loginRes = await login("test@email.com", "testPassword123@");

const TOKEN = loginRes.accessToken;

function SetEventListeners() {}

async function GetRooms(TOKEN) {
  const queryParams = "?status=open&pageNumber=1&pageSize=10";
  const rooms = await GetDataWithToken(
    `${BASE_URL}/rooms/all${queryParams}`,
    TOKEN
  );

  return rooms.data.items;
}

async function ListRooms(rooms) {
  const roomTable = document.getElementById("roomTable");

  for (const room of rooms) {
    const roomRow = createRoomRow(room);
    roomTable.appendChild(roomRow);
  }
}

function createRoomRow(room) {
  const roomRow = document.createElement("tr");
  roomRow.classList.add("room-row");

  roomRow.innerHTML = `
    <td>${room.roomId}</td>
    <td>${room.auctionId}</td>
    <td>${room.status}</td>
    <td><button class="joinRoomButton" data-roomid="${room.roomId}">Join</button></td>
  `;

  // Attach an event listener to the "Join" button
  if (room.status === "Open" || room.status === "open") {
    const joinButton = roomRow.querySelector(".joinRoomButton");
    joinButton.addEventListener("click", () => joinRoom(room.roomId));
  }

  return roomRow;
}

// Sample function to simulate joining a room
function joinRoom(roomId) {
  alert(`Joining Room ${roomId}`);
}

async function main() {
  const rooms = await GetRooms(TOKEN);
  await ListRooms(rooms);
}

main();
