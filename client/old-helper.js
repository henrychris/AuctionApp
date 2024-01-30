export async function joinRoom(roomId, connectionId, token) {
  const res = await postDataAuth(
    `${BASE_URL}/rooms/${roomId}/join`,
    {
      connectionId: connectionId,
    },
    token
  );

  if (res.status === 204) {
    console.log("joined");
    return true;
  }

  return false;
}

export async function leaveRoom(roomId, connectionId, token) {
  const res = await postDataAuth(
    `${BASE_URL}/rooms/${roomId}/leave`,
    {
      connectionId: connectionId,
    },
    token
  );

  if (res.status === 204) {
    console.log("left");
    return true;
  }

  return false;
}

export async function makeBid(roomId, connectionId, bid, token) {
  const res = await postDataAuth(
    `${BASE_URL}/rooms/${roomId}/bid`,
    {
      connectionId: connectionId,
      bidAmountInNaira: bid,
    },
    token
  );

  if (res.status === 200) {
    console.log("bid");
    return true;
  }

  console.log(res.json());
  return false;
}
