export interface EventBase {
  eventId: string;
  eventType: 'UserConnected' | 'UserDisconnected' | 'ChannelCreated' | 'ChannelUpdated' | 'ChannelDeleted' | 'UserJoined' | 'UserLeft' | 'MessageBroadcast' | 'NotificationBroadcast';
  eventTimestamp: string;
}

export interface UserConnected extends EventBase {
  eventType: 'UserConnected';
  userId: string;
  userName: string;
  userAvatar: string;
}

export interface UserDisconnected extends EventBase {
  eventType: 'UserDisconnected';
  userId: string;
  userName: string;
  userAvatar: string;
}

export interface ChannelCreated extends EventBase {
  eventType: 'ChannelCreated';
  channelId: string;
  channelName: string;
  channelIcon: string;
}

export interface ChannelUpdated extends EventBase {
  eventType: 'ChannelUpdated';
  channelId: string;
  channelName: string;
  channelIcon: string;
}

export interface ChannelDeleted extends EventBase {
  eventType: 'ChannelDeleted';
  channelId: string;
  channelName: string;
  channelIcon: string;
}

export interface UserJoined extends EventBase {
  eventType: 'UserJoined';
  userId: string;
  userName: string;
  userAvatar: string;
  channelId: string;
  channelName: string;
  channelIcon: string;
}

export interface UserLeft extends EventBase {
  eventType: 'UserLeft';
  userId: string;
  userName: string;
  userAvatar: string;
  channelId: string;
  channelName: string;
  channelIcon: string;
}

export type ContentType = 'text/plain';

export interface MessageBroadcast extends EventBase {
  eventType: 'MessageBroadcast';
  senderId: string;
  senderName: string;
  senderAvatar: string;
  channelId: string;
  channelName: string;
  channelIcon: string;
  contentType: ContentType;
  content: string;
  timestamp: string;
}

export interface NotificationBroadcast extends EventBase {
  eventType: 'NotificationBroadcast';
  content: string;
  timestamp: string;
}
