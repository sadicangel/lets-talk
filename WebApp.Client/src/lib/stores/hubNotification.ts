import type {
    ChannelCreated,
    ChannelDeleted,
    ChannelUpdated,
    NotificationBroadcast,
    UserConnected,
    UserDisconnected,
    UserJoined,
    UserLeft
} from '$lib/events';
import { writable } from 'svelte/store';

export const hubNotification$ = writable<HubNotification>();

export type HubNotification = UserConnected | UserDisconnected | ChannelCreated | ChannelUpdated | ChannelDeleted | UserJoined | UserLeft | NotificationBroadcast;