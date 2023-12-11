import type { MessageBroadcast } from "$lib/events";
import { writable } from "svelte/store";

export const messageList$ = function createMessageListStore() {
    const { subscribe, set, update } = writable<Readonly<Record<string, MessageBroadcast[]>>>({});

    function push(message: MessageBroadcast) {
        update((messages: Record<string, MessageBroadcast[]>) => {

            const list = (messages[message.channelId] = messages[message.channelId] || []);
            list.push(message);
            return messages;
        });
    }

    return {
        subscribe,
        set,
        update,
        push
    }
}();