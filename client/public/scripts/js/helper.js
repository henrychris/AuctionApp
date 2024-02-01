var __require = (id) => {
  return import.meta.require(id);
};

// public/scripts/ts/helper.ts
async function postData(url = "", data = {}) {
  const response = await fetch(url, {
    method: "POST",
    mode: "cors",
    cache: "no-cache",
    headers: {
      "Content-Type": "application/json"
    },
    redirect: "follow",
    referrerPolicy: "no-referrer",
    body: JSON.stringify(data)
  });
  return response.json();
}
async function PostDataWithToken(url = "", data = {}, token) {
  const response = await fetch(url, {
    method: "POST",
    mode: "cors",
    cache: "no-cache",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`
    },
    redirect: "follow",
    referrerPolicy: "no-referrer",
    body: JSON.stringify(data)
  });
  return response.json();
}
async function PostDataWithTokenNoRes(url = "", data = {}, token) {
  const response = await fetch(url, {
    method: "POST",
    mode: "cors",
    cache: "no-cache",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`
    },
    redirect: "follow",
    referrerPolicy: "no-referrer",
    body: JSON.stringify(data)
  });
  return response;
}
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
export {
  postData,
  PostDataWithTokenNoRes,
  PostDataWithToken,
  GetDataWithToken
};
