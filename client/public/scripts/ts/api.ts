import { BASE_URL } from "./config";
import { GetDataWithToken, PostDataWithToken } from "./helper";

interface getAuctionData {
  nameOfHighestBidder: string;
  highestBidAmountInNaira: number;
  auctionStatus: string;
}

export async function getAuctionData(
  roomId: string,
  TOKEN: string
): Promise<getAuctionData> {
  var res = await GetDataWithToken(BASE_URL + `/rooms/${roomId}/data`, TOKEN);
  return res.data as getAuctionData;
}
