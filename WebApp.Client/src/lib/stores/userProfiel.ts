import type { UserProfileResponse } from "$lib/responses";
import { writable } from "svelte/store";

export const userProfile$ = function create() {
    const { subscribe, set, update } = writable<UserProfileStore>({
        userId: '',
        userName: '',
        userAvatar: ''
    });

    return {
        subscribe,
        set,
        update,
        fetch: async () => {
            const userProfileResponse = await fetch('/api/account/profile');
            if (!userProfileResponse.ok) {
                console.error(userProfileResponse);
                return;
            }
            set(await userProfileResponse.json());
        }
    }
}();

export interface UserProfileStore extends UserProfileResponse {
    userId: string;
    userName: string;
    userAvatar: string;
}