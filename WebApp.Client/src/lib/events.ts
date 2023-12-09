export interface EventBase {
  id: string;
  type: string;
  time: string;
}

export interface UserConnected extends EventBase {
  userId: string;
  userName: string;
  userAvatar: string;
}

export interface UserDisconnected extends EventBase {
  userId: string;
  userName: string;
  userAvatar: string;
}

export interface ChannelCreated extends EventBase {
  channelId: string;
  channelName: string;
  channelIcon: string;
}

export interface ChannelDeleted extends EventBase {
  channelId: string;
  channelName: string;
  channelIcon: string;
}

export interface UserJoined extends EventBase {
  userId: string;
  userName: string;
  userAvatar: string;
  channelId: string;
  channelName: string;
  channelIcon: string;
}

export interface UserLeft extends EventBase {
  userId: string;
  userName: string;
  userAvatar: string;
  channelId: string;
  channelName: string;
  channelIcon: string;
}

export interface MessageBroadcast extends EventBase {
  senderId: string;
  senderName: string;
  senderAvatar: string;
  channelId: string;
  channelName: string;
  channelIcon: string;
  contentType: string;
  content: string;
  timestamp: string;
}

export interface NotificationBroadcast extends EventBase {
  contentType: string;
  content: string;
  timestamp: string;
}