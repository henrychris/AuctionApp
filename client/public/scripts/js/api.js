"use strict";
(() => {
  // public/scripts/ts/config.ts
  var BASE_URL = "http://localhost:5000/api";

  // public/scripts/ts/helper.ts
  async function GetDataWithToken(url = "", token) {
    const response = await fetch(url, {
      method: "GET",
      mode: "cors",
      cache: "no-cache",
      headers: {
        Authorization: `Bearer ${token}`
      },
      redirect: "follow",
      referrerPolicy: "no-referrer"
    });
    return response.json();
  }

  // public/scripts/ts/api.ts
  async function getAuctionData(roomId, TOKEN) {
    var res = await GetDataWithToken(BASE_URL + `/rooms/${roomId}/data`, TOKEN);
    return res.data;
  }
  async function GetUserDetails(userId, TOKEN) {
    const res = await GetDataWithToken(`${BASE_URL}/user/${userId}`, TOKEN);
    return res.data;
  }
})();
