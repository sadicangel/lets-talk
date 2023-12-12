import { writable } from "svelte/store";

export const channelList$ = function create() {
    const { subscribe, set, update } = writable<ChannelListStore>({ channels: {} });
    return {
        subscribe,
        set,
        update,
        fetch: async () => {
            const channelListResponse = await fetch('/api/account/channels');
            if (!channelListResponse.ok) {
                console.error(channelListResponse);
                return;
            }
            const channels: Array<UserChannelListChannel> = (await channelListResponse.json()).channels;
            update(s => {
                for (const channel of channels)
                    s.channels[channel.channelId] = channel;
                return s;
            });
        }
    };
}();

export interface ChannelListStore {
    channels: Record<string, UserChannelListChannel>
}

export interface UserChannelListChannel {
    channelId: string;
    channelName: string;
    channelIcon: string;
};