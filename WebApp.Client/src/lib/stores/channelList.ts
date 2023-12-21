import { writable } from "svelte/store";

export const channelList$ = function create() {
    const { subscribe, set, update } = writable<ChannelListStore>({ channels: {} });
    return {
        subscribe,
        set,
        update
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