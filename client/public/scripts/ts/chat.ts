// // todo: implement chat functionality
// // make navbar work
// // use express to bundle client or something sha so page navigation works.
// // test chat with different users

// // placeholder token generation
// // store token in local storage later. or use express jeje
// import type { ApiResponse, UserAuthResponse } from "./config.js";
// import {
//   SendMessageToRoom,
//   StartSignalRConnection,
//   LeaveRoom,
//   MakeBid,
// } from "./signalRConn.js";

// let loginRes: UserAuthResponse;
// let TOKEN = "";
// const storedData = JSON.parse(
//   localStorage.getItem("loginRes")!
// ) as UserAuthResponse;

// loginRes = storedData;
// console.log(loginRes);
// TOKEN = loginRes.accessToken;

// function SetChatEventListeners() {
//   document
//     .getElementById("sendMessageButton")!
//     .addEventListener("click", SendMessageToRoomInternal);

//   document
//     .getElementById("bidButton")!
//     .addEventListener("click", MakeBidInternal);

//   document
//     .getElementById("leaveRoomButton")!
//     .addEventListener("click", LeaveRoomInternal);
// }

// function SendMessageToRoomInternal() {
//   const messageInput = document.getElementById(
//     "messageInput"
//   ) as HTMLInputElement;
//   const roomId = document.getElementById("roomId")!.innerText;

//   if (messageInput.value && roomId) {
//     SendMessageToRoom(messageInput.value, roomId);
//     messageInput.value = "";
//   } else {
//     console.error("Type in a message before sending!");
//     messageInput.focus();
//   }
// }

// async function MakeBidInternal() {
//   const bidInput = document.getElementById("bidInput") as HTMLInputElement;
//   const roomId = (document.getElementById("roomId") as HTMLInputElement)
//     .innerText;

//   if (bidInput.value && roomId) {
//     const bidValue = parseFloat(bidInput.value);
//     await MakeBid(roomId, bidValue, TOKEN);
//     bidInput.value = "";
//   } else {
//     console.error("Input a value before sending a bid!");
//   }
// }

// async function LeaveRoomInternal() {
//   let userId = loginRes!.id;
//   const roomIdInput = document.getElementById("roomId");

//   if (!roomIdInput) {
//     console.error("Something has gone horribly wrong. There's no room ID!");
//     return;
//   }

//   const roomId = roomIdInput.innerText;
//   let success = await LeaveRoom(roomId, TOKEN);
//   if (!success) {
//     console.log(`${userId} failed to leave the room`);
//     return;
//   }

//   console.log(`${userId} left ${roomId}`);
// }

// SetChatEventListeners();
// // connect to chatRoom
