import type { UserChannelListResponse } from "$lib/api";
import { writable } from "svelte/store";

export const channelList$ = function create() {
    const { subscribe, set, update } = writable<UserChannelListResponse>({ channels: [] });
    return {
        subscribe,
        set,
        update
    };
}();