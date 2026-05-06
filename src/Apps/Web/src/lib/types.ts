export type AccessTokenResponse = {
  accessToken: string;
  refreshToken?: string;
  expiresIn: number;
};

export type UserProfile = {
  userId: string;
  userName: string;
  email: string;
  avatarUrl: string | null;
};

export type Channel = {
  channelId: string;
  channelName: string;
  description: string | null;
  members: string[];
};

export type ChannelListResponse = {
  channels: Channel[];
};

export type HubUser = {
  userId: string;
  userName: string;
  email: string;
  avatarUrl: string | null;
};

export type HubChannel = {
  channelId: string;
  channelName: string;
  description: string | null;
};

export type HubMessage = {
  eventId: string;
  timestamp: string;
  channel: HubChannel;
  author: HubUser;
  contentType: string;
  content: unknown;
};

export type ChatMessage = {
  id: string;
  channelId: string;
  author: HubUser;
  contentType: string;
  text: string | null;
  imageUrl: string | null;
  timestamp: string;
};
