// See https://kit.svelte.dev/docs/types#app

import type { UserProfileResponse } from "$lib/api";

// for information about these interfaces
declare global {
    namespace App {
        // interface Error {}
        interface Locals {
            user: {
                isAuthenticated?: boolean,
                profile?: UserProfileResponse
            }
        }
        // interface PageData {}
        // interface Platform {}
    }
}

export { };
