import api from '$lib/api.js';
import { userProfile$ } from '$lib/stores/userProfile.js'
import { get } from 'svelte/store'

export async function load({ data, fetch }) {
    data.user.profile = get(userProfile$);
    if (data.user.isAuthenticated && !data.user.profile) {
        data.user.profile = await api.account.profile().send(fetch);
        if (!data.user.profile) {
            data.user.isAuthenticated = false;
            data.user.profile = undefined;
        }
    }
    return data;
}