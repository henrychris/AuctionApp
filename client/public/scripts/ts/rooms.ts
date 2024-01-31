import { BASE_URL, USER_EMAIL, USER_PASSWORD } from "./config.js";
import { GetDataWithToken } from "./helper.js";
import {
  JoinRoom,
  LeaveRoom,
  MakeBid,
  SendMessageToRoom,
  StartSignalRConnection,
} from "./signalRConn.js";
import type { ApiResponse, UserAuthResponse } from "./config.js";
import { getAuctionData } from "./api.js";
// placeholder token generation
// store token in local storage later. or use express jeje

let loginRes: UserAuthResponse;
let TOKEN = "";
const storedData = JSON.parse(
  localStorage.getItem("loginRes")!
) as UserAuthResponse;

loginRes = storedData;
if (loginRes) {
  console.log("retrieved access token");
}
TOKEN = loginRes.accessToken;

interface Rooms {
  items: Room[];
}

interface Room {
  status: string;
  auctionId: string;
  roomId: string;
}

function SetEventListeners() {
  document
    .getElementById("sendMessageButton")!
    .addEventListener("click", SendMessageToRoomInternal);

  document
    .getElementById("bidButton")!
    .addEventListener("click", MakeBidInternal);

  document
    .getElementById("messageInput")!
    .addEventListener("keyup", function (event) {
      if (event.key === "Enter") {
        event.preventDefault();
        SendMessageToRoomInternal();
      }
    });

  document
    .getElementById("bidInput")!
    .addEventListener("keyup", function (event) {
      if (event.key === "Enter") {
        event.preventDefault();
        MakeBidInternal();
      }
    });

  document
    .getElementById("leaveRoomButton")!
    .addEventListener("click", LeaveRoomInternal);
}

async function GetRooms(token: string) {
  const queryParams = "?status=open&pageNumber=1&pageSize=10";
  const rooms = await GetDataWithToken(
    `${BASE_URL}/rooms/all${queryParams}`,
    token
  );

  return rooms.data as Rooms;
}

async function ListRooms(rooms: Rooms) {
  const roomTable = document.getElementById("roomTable")!;

  for (const room of rooms.items) {
    const roomRow = createRoomRow(room);
    roomTable.appendChild(roomRow);
  }
}

function createRoomRow(room: Room) {
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
    const joinButton = roomRow.querySelector(".joinRoomButton")!;
    joinButton.addEventListener("click", () => joinRoomInternal(room.roomId));
  }

  return roomRow;
}

// Sample function to simulate joining a room
async function joinRoomInternal(roomId: string) {
  let userId = loginRes!.id;

  if (!roomId) {
    console.error("Something has gone horribly wrong. There's no room ID!");
    return;
  }

  let success = await JoinRoom(roomId, TOKEN);
  if (!success) {
    console.log(`${userId} failed to join the room`);
    return;
  }

  const chatScreen = document.getElementById("chatScreen")!;
  const roomScreen = document.getElementById("roomListScreen")!;
  chatScreen.style.display = "block";
  roomScreen.style.display = "none";

  localStorage.setItem("currentRoomId", roomId);
  console.log(`${userId} joined ${roomId}`);
}

async function main() {
  const rooms = await GetRooms(TOKEN);
  await ListRooms(rooms);
}

function SendMessageToRoomInternal() {
  const messageInput = document.getElementById(
    "messageInput"
  ) as HTMLInputElement;
  const roomId = document.getElementById("roomId")!.innerText;

  if (messageInput.value && roomId) {
    SendMessageToRoom(messageInput.value, roomId);
    messageInput.value = "";
  } else {
    console.error("Type in a message before sending!");
    messageInput.focus();
  }
}

async function MakeBidInternal() {
  const bidInput = document.getElementById("bidInput") as HTMLInputElement;
  const roomId = (document.getElementById("roomId") as HTMLInputElement)
    .innerText;

  if (bidInput.value && roomId) {
    const bidValue = parseFloat(bidInput.value);
    await MakeBid(roomId, bidValue, TOKEN);
    bidInput.value = "";
  } else {
    console.error("Input a value before sending a bid!");
  }
}

async function LeaveRoomInternal() {
  let userId = loginRes!.id;
  const roomIdInput = document.getElementById("roomId");

  if (!roomIdInput) {
    console.error("Something has gone horribly wrong. There's no room ID!");
    return;
  }

  const roomId = roomIdInput.innerText;
  let success = await LeaveRoom(roomId, TOKEN);
  if (!success) {
    console.log(`${userId} failed to leave the room`);
    return;
  }

  console.log(`${userId} left ${roomId}`);
  window.location.href = "./rooms.html";
}

function addMutationObserver() {
  const section = document.getElementById("chatScreen")!;

  // Create an observer instance
  const observer = new MutationObserver(function (mutations) {
    mutations.forEach(function (mutation) {
      // Check if the section is visible
      if (section.style.display !== "none") {
        // Update data
        loadAuctionData();
      }
    });
  });

  const config = { attributes: true, attributeFilter: ["style"] };
  observer.observe(section, config);
}

async function loadAuctionData() {
  console.log("Loading auction data.");
  
  const roomId = localStorage.getItem("currentRoomId");
  if (!roomId) {
    throw new Error("current roomId not set!");
  }

  const auctionData = await getAuctionData(roomId, TOKEN);

  const roomIdElement = document.getElementById("roomId")!;
  const highestBidElement = document.getElementById("highestPriceValue")!;
  const statusElement = document.getElementById("auctionStatusValue")!;

  roomIdElement.innerText = roomId;
  highestBidElement.innerText = auctionData.highestBidAmountInNaira + " NGN";
  statusElement.innerText = auctionData.auctionStatus;
}

// get connectionId and store it here.
SetEventListeners();
addMutationObserver();
StartSignalRConnection(TOKEN);
main();
