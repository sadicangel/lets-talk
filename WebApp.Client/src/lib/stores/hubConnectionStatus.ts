import { HubConnectionState } from "@microsoft/signalr";
import { writable } from "svelte/store";

export const hubConnectionStatus$ = writable<HubConnectionStatus>({
    isConnected: false,
    state: HubConnectionState.Disconnected
});

export interface HubConnectionStatus {
    isConnected: boolean;
    state: HubConnectionState;
    error?: Error;
}