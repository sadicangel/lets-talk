export type ContentType = 'text/plain';

export type ChannelDto = {
    id: string;
    displayName: string;
    iconUrl?: string;
    adminId: string;
}

export type UserDto = {
    id: string;
    userName: string;
    avatarUrl?: string;
}

export type EventBase = {
    eventId: string,
    eventName: 'MessageEvent' | 'NotificationEvent' | 'UserConnectedEvent' | 'UserDisconnectedEvent' | 'ChannelMemberJoinedEvent' | 'ChannelMemberLeftEvent',
    timestamp: string,
}

export type MessageEvent = EventBase & {
    eventName: 'MessageEvent',
    channel: ChannelDto,
    author: UserDto,
    contentType: ContentType,
    content: string,
}

export type NotificationEvent = EventBase & {
    eventName: 'NotificationEvent',
    contentType: ContentType,
    content: string,
}

export type UserConnectedEvent = EventBase & {
    eventName: 'UserConnectedEvent',
    connectingUser: UserDto;
    users: UserDto[];
}

export type UserDisconnectedEvent = EventBase & {
    eventName: 'UserDisconnectedEvent',
    disconnectingUser: UserDto;
    users: UserDto[];
}

export type ChannelMemberJoinedEvent = EventBase & {
    eventName: 'ChannelMemberJoinedEvent',
    channel: ChannelDto;
    joiningMember: UserDto;
    members: UserDto[];
}

export type ChannelMemberLeftEvent = EventBase & {
    eventName: 'ChannelMemberLeftEvent',
    channel: ChannelDto;
    leavingMember: UserDto;
    members: UserDto[];
}