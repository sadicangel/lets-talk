import api, { type UserChannelListResponse } from '$lib/api.js';
import { channelList$ } from '$lib/stores/channelList.js';

export async function load({ fetch, parent }) {
    // if (data.user.isAuthenticated && !get(userProfile$)) {
    //     data.user.profile = await api.account.profile().send(fetch);
    // }
    // userProfile$.set(data.user.profile);

    const data = await parent();

    let channelList: UserChannelListResponse = { channels: [] };
    if (data.user.profile) {
        channelList = await api.account.channels().send(fetch);
    }
    channelList$.set(channelList);

    return {
        ...data,
        channelList,
    };
}