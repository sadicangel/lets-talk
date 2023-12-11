export interface UserProfileResponse {
    userId: string;
    userName: string;
    userAvatar: string;
}

export interface UserChannelListChannel {
    channelId: string;
    channelName: string;
    channelIcon: string;
};

export interface UserChannelListResponse {

    channels: UserChannelListChannel[]
}
