import { login } from "./auth.js";
import { BASE_URL } from "./config.js";
import { GetDataWithToken } from "./helper.js";
import { JoinRoom, StartSignalRConnection } from "./signalRConn.js";
// placeholder token generation
// store token in local storage later. or use express jeje
const loginRes = await login("test@email.com", "testPassword123@");
const TOKEN = loginRes.accessToken;
function SetEventListeners() { }
async function GetRooms(token) {
    const queryParams = "?status=open&pageNumber=1&pageSize=10";
    const rooms = await GetDataWithToken(`${BASE_URL}/rooms/all${queryParams}`, token);
    return rooms.data;
}
async function ListRooms(rooms) {
    const roomTable = document.getElementById("roomTable");
    for (const room of rooms.items) {
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
        joinButton.addEventListener("click", () => joinRoomInternal(room.roomId));
    }
    return roomRow;
}
// Sample function to simulate joining a room
async function joinRoomInternal(roomId) {
    let userId = loginRes.id;
    if (!roomId) {
        console.error("Something has gone horribly wrong. There's no room ID!");
        return;
    }
    let success = await JoinRoom(roomId, TOKEN);
    if (!success) {
        console.log(`${userId} failed to join the room`);
        return;
    }
    window.location.href = "chatRoom.html";
    console.log(`${userId} joined ${roomId}`);
}
async function main() {
    const rooms = await GetRooms(TOKEN);
    await ListRooms(rooms);
}
StartSignalRConnection(TOKEN);
main();
