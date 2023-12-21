import { writable } from "svelte/store";

export const userProfile$ = function create() {
    const { subscribe, set, update } = writable<UserProfileStore>();

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
            const profile: UserProfileStore = await userProfileResponse.json();
            set(profile);
            return profile;
        }
    }
}();

export interface UserProfileStore {
    userId: string;
    userName: string;
    userAvatar: string;
}