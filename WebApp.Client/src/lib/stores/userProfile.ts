import type { UserProfileResponse } from "$lib/api";
import { writable } from "svelte/store";

export const userProfile$ = function create() {
    const { subscribe, set, update } = writable<UserProfileResponse | undefined>();

    return {
        subscribe,
        set,
        update
    }
}();