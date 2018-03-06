using System;
using System.Collections.Generic;

namespace com.fpnn.rtm
{
	public class RTMQuestCallback
	{
		public virtual void authCallback(bool success, bool isReconnect) { }
		public virtual void authException(RTMException e) { }

		public virtual void sendMessageCallback() { }
		public virtual void sendMessageException(RTMException e) { }

		public virtual void sendMessagesCallback() { }
		public virtual void sendMessagesException(RTMException e) { }

		public virtual void sendGroupMessageCallback() { }
		public virtual void sendGroupMessageException(RTMException e) { }

		public virtual void sendRoomMessageCallback() { }
		public virtual void sendRoomMessageException(RTMException e) { }

		public virtual void byeCallback() { }
		public virtual void byeException(RTMException e) { }

		public virtual void addVariablesCallback() { }
		public virtual void addVariablesException(RTMException e) { }

		public virtual void setPushNameCallback() { }
		public virtual void setPushNameException(RTMException e) { }

		public virtual void getPushNameCallback(string pushName) { }
		public virtual void getPushNameException(RTMException e) { }

		public virtual void setGeoCallback() { }
		public virtual void setGeoException(RTMException e) { }

		public virtual void getGeoCallback(double lat, double lng) { }
		public virtual void getGeoException(RTMException e) { }

		public virtual void getGeosCallback(List<List<string>> geos) { }
		public virtual void getGeosException(RTMException e) { }

		public virtual void addFriendsCallback() { }
		public virtual void addFriendsException(RTMException e) { }

		public virtual void deleteFriendsCallback() { }
		public virtual void deleteFriendsException(RTMException e) { }

		public virtual void getFriendsCallback(HashSet<long> uids) { }
		public virtual void getFriendsException(RTMException e) { }

		public virtual void addGroupMembersCallback() { }
		public virtual void addGroupMembersException(RTMException e) { }

		public virtual void delGroupMembersCallback() { }
		public virtual void delGroupMembersException(RTMException e) { }

		public virtual void getGroupMembersCallback(List<long> uids) { }
		public virtual void getGroupMembersException(RTMException e) { }

		public virtual void getUserGroupsCallback(List<long> gids) { }
		public virtual void getUserGroupsException(RTMException e) { }

		public virtual void enterRoomCallback() { }
		public virtual void enterRoomException(RTMException e) { }

		public virtual void leaveRoomCallback() { }
		public virtual void leaveRoomException(RTMException e) { }

		public virtual void getUserRoomsCallback(List<long> rooms) { }
		public virtual void getUserRoomsException(RTMException e) { }

		public virtual void getOnlineUsersCallback(List<long> uids) { }
		public virtual void getOnlineUsersException(RTMException e) { }

		public virtual void getGroupMessageCallback(short num, long maxid, List<GroupMsg> msgs) { }
		public virtual void getGroupMessageException(RTMException e) { }

		public virtual void getRoomMessageCallback(short num, long maxid, List<RoomMsg> msgs) { }
		public virtual void getRoomMessageException(RTMException e) { }

		public virtual void getBroadcastMessageCallback(short num, long maxid, List<BroadcastMsg> msgs) { }
		public virtual void getBroadcastMessageException(RTMException e) { }

		public virtual void getP2PMessageCallback(short num, long maxid, List<P2PMsg> msgs) { }
		public virtual void getP2PMessageException(RTMException e) { }

		public virtual void addDeviceCallback() { }
		public virtual void addDeviceException(RTMException e) { }

		public virtual void setLangCallback() { }
		public virtual void setLangException(RTMException e) { }

		public virtual void translateCallback(string stext, string src, string dtext, string dst) { }
		public virtual void translateException(RTMException e) { }

		public virtual void sendFileCallback() { }
		public virtual void sendFileException(RTMException e) { }

		public virtual void sendFilesCallback() { }
		public virtual void sendFilesException(RTMException e) { }

		public virtual void sendGroupFileCallback() { }
		public virtual void sendGroupFileException(RTMException e) { }

		public virtual void sendRoomFileCallback() { }
		public virtual void sendRoomFileException(RTMException e) { }

		public virtual void broadcastFileCallback() { }
		public virtual void broadcastFileException(RTMException e) { }
	}
}

