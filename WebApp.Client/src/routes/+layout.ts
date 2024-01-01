import api from '$lib/api.js';
import { userProfile$ } from '$lib/stores/userProfile.js';
import { get } from 'svelte/store';

export async function load({ data, fetch }) {
    if (data.user.isAuthenticated && !get(userProfile$)) {
        data.user.profile = await api.account.profile().send(fetch);
    }
    userProfile$.set(data.user.profile);

    return data;
}