export const BASE_URL = "http://localhost:5030/api";

export async function postData(url = "", data = {}) {
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

export async function joinRoom(roomId, connectionId, token) {
  const res = await postDataAuth(
    `${BASE_URL}/rooms/${roomId}/join`,
    {
      connectionId: connectionId,
    },
    token
  );
}

export async function postDataAuth(url = "", data = {}, token) {
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
