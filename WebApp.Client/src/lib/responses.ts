export interface UserProfile {
    userId: string;
    userName: string;
    userAvatar: string;
    channels: ChannelProfile[];
}

export interface ChannelProfile {
    channelId: string;
    channelName: string;
    channelIcon: string;
}
