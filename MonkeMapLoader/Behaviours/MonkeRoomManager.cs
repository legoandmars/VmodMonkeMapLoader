using System;
using System.Collections.Generic;
using System.Text;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Linq;
namespace VmodMonkeMapLoader.Behaviours
{
    public class MonkeRoomManager : MonoBehaviourPunCallbacks
    {
        public struct RoomRegionInfo {
            public RoomInfo RoomInfo;
            public string Region;

            public RoomRegionInfo(RoomInfo RoomInfo, string Region)
            {
                this.RoomInfo = RoomInfo;
                this.Region = Region;
            }
        }

        public static string[] checkedRegions;
        public static string forcedRegion;

        public static List<RoomRegionInfo> roomListCache = new List<RoomRegionInfo>();
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            string currentRegion = PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion;
            if (forcedRegion == null && checkedRegions == null)
            {
                checkedRegions = PhotonNetworkController.instance.serverRegions;
                forcedRegion = PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion;
                List<string> regionList = checkedRegions.ToList();
                if (regionList.ToList().Remove(forcedRegion)) checkedRegions = regionList.ToArray();
            }

            int validRoomsCount = 0;
            foreach (RoomInfo roomInfo in roomList)
            {
                if (roomListCache.Where(x => x.RoomInfo.Name == roomInfo.Name).Count() == 0)
                {
                    validRoomsCount++;
                    roomListCache.Add(new RoomRegionInfo(roomInfo, currentRegion));
                }
                else
                {
                    // replace some rooms, that's OK
                    roomListCache.Remove(roomListCache.Where(x => x.RoomInfo.Name == roomInfo.Name).First());
                    roomListCache.Add(new RoomRegionInfo(roomInfo, currentRegion));
                }
            }
            Debug.Log(validRoomsCount+ " Valid rooms in " + PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion);
            Debug.Log(roomListCache.Sum(x => x.RoomInfo.PlayerCount));

            // handle custom map stuff
            if(roomListCache.Count > 0)
            {
                var MapRooms = roomListCache.Where(x => x.RoomInfo.CustomProperties["gameMode"] != null && x.RoomInfo.CustomProperties["gameMode"].ToString().Contains("_"));
                Dictionary<string, int> playersOnMaps = new Dictionary<string, int>();
                foreach(RoomRegionInfo regionInfo in MapRooms)
                {
                    RoomInfo info = regionInfo.RoomInfo;

                    string get = info.CustomProperties["gameMode"].ToString().Split(new string[]{"MOD_"}, StringSplitOptions.None)[1].Split(new string[] { "DEFAULT" }, StringSplitOptions.None)[0];
                    Debug.Log(get);
                    Debug.Log(regionInfo.Region);
                    if (playersOnMaps.ContainsKey(get)) playersOnMaps[get] += info.PlayerCount;
                    else playersOnMaps.Add(get, info.PlayerCount);
                }
                Debug.Log("MAP ROOM COUNT: " + MapRooms.Count());

                foreach (var customMap in playersOnMaps)
                {
                    Debug.Log(customMap.Key + " CURRENTLY HAS " + customMap.Value + " PLAYERS");
                }
            }
            if (roomList.Count > 0)
            {
                // switch server
                if(checkedRegions.Length > 0)
                {
                    string newRegion = checkedRegions[0];

                    List<string> checkedRegionList = checkedRegions.ToList();
                    checkedRegionList.RemoveAt(0);
                    checkedRegions = checkedRegionList.ToArray();
                    Debug.Log("Connecting with new region... attempting "+ newRegion);
                    Patches.ForceRegionPatch.forcedRegion = newRegion;
                    PhotonNetwork.Disconnect();
                }
                else if(checkedRegions.Length == 0 && forcedRegion != null)
                {
                    Debug.Log("Connecting back to the best region " + forcedRegion);
                    Patches.ForceRegionPatch.forcedRegion = forcedRegion;
                    PhotonNetwork.Disconnect();
                    forcedRegion = null;
                }
            }
        }

        public override void OnConnectedToMaster()
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }
            PhotonNetwork.JoinLobby();
        }

        public static int PlayersOnMap(string mapName)
        {
            var MapRooms = roomListCache.Where(x => x.RoomInfo.CustomProperties["gameMode"] != null && x.RoomInfo.CustomProperties["gameMode"].ToString().Contains("_"));
            Dictionary<string, int> playersOnMaps = new Dictionary<string, int>();
            foreach (RoomRegionInfo regionInfo in MapRooms)
            {
                RoomInfo info = regionInfo.RoomInfo;

                string get = info.CustomProperties["gameMode"].ToString().Split(new string[] { "MOD_" }, StringSplitOptions.None)[1].Split(new string[] { "DEFAULT" }, StringSplitOptions.None)[0];
                if (playersOnMaps.ContainsKey(get)) playersOnMaps[get] += info.PlayerCount;
                else playersOnMaps.Add(get, info.PlayerCount);
            }

            if (playersOnMaps.ContainsKey(mapName))
            {
                return playersOnMaps[mapName];
            }
            else return 0;
        }
    }
}
