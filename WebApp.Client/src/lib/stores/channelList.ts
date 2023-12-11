import type { UserChannelListResponse } from "$lib/responses";
import { writable } from "svelte/store";

export const channelList$ = function create() {
    const { subscribe, set, update } = writable<ChannelListStore>({ channels: [] });
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

            set(await channelListResponse.json());
        }
    };
}();

export interface ChannelListStore extends UserChannelListResponse {
    channels: ChannelStore[]
}

export interface UserChannelListChannel {
    channelId: string;
    channelName: string;
    channelIcon: string;
};

export interface ChannelStore {
    channelId: string;
    channelName: string;
    channelIcon: string;
};