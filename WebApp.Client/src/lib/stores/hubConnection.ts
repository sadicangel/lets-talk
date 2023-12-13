import { browser } from "$app/environment";
import type { ChannelCreated, ChannelDeleted, ChannelUpdated, MessageBroadcast, NotificationBroadcast, UserConnected, UserDisconnected, UserJoined, UserLeft } from "$lib/events";
import { HubConnectionBuilder, type HubConnection, HttpTransportType, HubConnectionState } from "@microsoft/signalr";
import { writable } from "svelte/store";
import { hubConnectionStatus$ } from "./hubConnectionStatus";
import { hubNotification$ } from "./hubNotification";
import { messageList$ } from "./messageList";

export const hubConnection$ = function create() {
    let connection: HubConnection;
    const { subscribe } = writable<HubConnection>({} as any, (set) => {
        if (browser) {
            connection = new HubConnectionBuilder()
                .withUrl('/api/letstalk', {
                    skipNegotiation: true,
                    transport: HttpTransportType.WebSockets
                })
                .build();

            connection.onreconnecting((error) => {
                hubConnectionStatus$.set({
                    isConnected: connection.state === HubConnectionState.Connected,
                    state: connection.state,
                    error
                });
            })

            connection.onreconnected(() => {
                hubConnectionStatus$.set({
                    isConnected: connection.state === HubConnectionState.Connected,
                    state: connection.state
                });
            });

            connection.onclose((error) => {
                hubConnectionStatus$.set({
                    isConnected: connection.state === HubConnectionState.Connected,
                    state: connection.state,
                    error
                });
            });

            connection.on('OnUserConnected', (event: UserConnected) => {
                hubNotification$.set(event);
            });

            connection.on('OnUserDisconnected', (event: UserDisconnected) => {
                hubNotification$.set(event);
            });

            connection.on('OnChannelCreated', (event: ChannelCreated) => {
                hubNotification$.set(event);
            });

            connection.on('OnChannelUpdated', (event: ChannelUpdated) => {
                hubNotification$.set(event);
            });

            connection.on('OnChannelDeleted', (event: ChannelDeleted) => {
                hubNotification$.set(event);
            });

            connection.on('OnUserJoined', (event: UserJoined) => {
                hubNotification$.set(event);
            });

            connection.on('OnUserLeft', (event: UserLeft) => {
                hubNotification$.set(event);
            });

            connection.on('OnMessageBroadcast', (event: MessageBroadcast) => {
                messageList$.push(event);
            });

            connection.on('OnNotificationBroadcast', (event: NotificationBroadcast) => {
                hubNotification$.set(event);
            });

            set(connection);
        }
    });

    return {
        subscribe,
        connect: async () => {
            if (!connection) return;
            await connection.start();
            hubConnectionStatus$.set({
                isConnected: connection.state === HubConnectionState.Connected,
                state: connection.state
            });
        },
        disconnect: async () => {
            if (!connection) return;
            await connection.stop();
        }
    };
}();