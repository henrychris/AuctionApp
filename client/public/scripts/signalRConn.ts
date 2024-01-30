import { PostDataWithToken, PostDataWithTokenNoRes } from "./helper.js";
import { BASE_URL } from "./config.js";
import * as signalR from "@microsoft/signalr";

let connection: signalR.HubConnection;

export async function StartSignalRConnection(token: string) {
  connection = new signalR.HubConnectionBuilder()
    .withAutomaticReconnect()
    .withUrl("http://localhost:5030/auctionHub", {
      accessTokenFactory: () => token,
    })
    .build();

  SetSignalRMessageReceivers(connection);

  try {
    await connection.start();
    console.log("SignalR connection started successfully.");
  } catch (error) {
    console.error("Error starting SignalR connection:", error);
  }
}

function SetSignalRMessageReceivers(connection: signalR.HubConnection) {
  connection.on("ReceiveMessage", function (msg) {
    console.log("hit");
    const messages = document.getElementById("messages")!;
    const user = `<span style="font-weight: bold">${msg.userName}: </span>`;
    messages.innerHTML += `<p>${user}<span>${msg.content}</span></p>`;
  });

  connection.on("AuctionStarted", function (msg) {
    console.log("hit");
    const messages = document.getElementById("messages");
    const user = `<span style="font-weight: bold">Admin: </span>`;
    messages!.innerHTML += `<p>${user}<span>${msg}</span></p>`;

    const auctionStatus = document.getElementById("auctionStatusValue")!;
    auctionStatus.innerText = "In Progress";
  });

  connection.on("UserJoined", function (msg) {
    const messages = document.getElementById("messages")!;
    const user = `<span style="font-weight: bold">${msg} </span>`;
    messages.innerHTML += `<p style="color:grey">${user}has joined.</p>`;
  });

  connection.on("UserLeft", function (msg) {
    const messages = document.getElementById("messages")!;
    const user = `<span style="font-weight: bold">${msg} </span>`;
    messages.innerHTML += `<p style="color:grey">${user}has left.</p>`;
  });

  connection.on("BidPlaced", function (msg, amount) {
    const messages = document.getElementById("messages")!;
    const user = `<span style="font-weight: bold">${msg.userName}: </span>`;
    messages.innerHTML += `<p>${user}<span>${msg.content}</span></p>`;
  });
}

export function SendMessageToRoom(message: string, roomId: string) {
  connection.invoke("SendMessageToRoom", roomId, message);
}

export async function MakeBid(roomId: string, bid: number, token: string) {
  const res = await PostDataWithToken(
    `${BASE_URL}/rooms/${roomId}/bid`,
    {
      connectionId: connection.connectionId,
      bidAmountInNaira: bid,
    },
    token
  );

  if (res.status === 200) {
    console.log("bid successful");
    return true;
  }

  console.error(res.json());
  return false;
}

export async function LeaveRoom(roomId: string, token: string) {
  const res = await PostDataWithTokenNoRes(
    `${BASE_URL}/rooms/${roomId}/leave`,
    {
      connectionId: connection.connectionId,
    },
    token
  );

  if (res.status === 204) {
    console.log("left");
    return true;
  }

  console.error(res);
  return false;
}