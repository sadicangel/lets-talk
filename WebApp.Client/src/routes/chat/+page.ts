import type { UserChannelListChannel } from '$lib/stores/channelList.js';
import { userProfile$ } from '$lib/stores/userProfile.js';
import { get } from 'svelte/store';

export async function load({ fetch }) {
    if (get(userProfile$)) {
        const channelListResponse = await fetch('/api/account/channels');
        if (!channelListResponse.ok) {
            console.error(channelListResponse);
            return;
        }
        const channels: UserChannelListChannel[] = (await channelListResponse.json()).channels;
        return { channels };
    }
}