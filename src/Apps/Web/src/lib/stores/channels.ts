import type { Channel } from '$lib/types';
import { writable } from 'svelte/store';

export const channels = writable<Channel[]>([]);
export const activeChannelId = writable<string | null>(null);
