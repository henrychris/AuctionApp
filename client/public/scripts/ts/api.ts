import { BASE_URL } from "./config";
import { GetDataWithToken, PostDataWithToken } from "./helper";

interface getAuctionData {
  nameOfHighestBidder: string;
  highestBidAmountInNaira: number;
  auctionStatus: string;
}

interface GetUserResponse {
  firstName: string;
}

export async function getAuctionData(
  roomId: string,
  TOKEN: string
): Promise<getAuctionData> {
  var res = await GetDataWithToken(BASE_URL + `/rooms/${roomId}/data`, TOKEN);
  return res.data as getAuctionData;
}

export async function GetUserDetails(userId: string, TOKEN: string) {
  const res = await GetDataWithToken(`${BASE_URL}/user/${userId}`, TOKEN);
  return res.data as GetUserResponse;
}
