import type { UserProfile } from '$lib/types';
import { writable } from 'svelte/store';

export const profile = writable<UserProfile | null>(null);
export const accessToken = writable<string | null>(null);
export const chatApiUrl = writable<string>('http://localhost:5099');
