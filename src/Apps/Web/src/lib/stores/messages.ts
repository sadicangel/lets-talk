import type { ChatMessage } from '$lib/types';
import { derived, writable } from 'svelte/store';
import { activeChannelId } from './channels';

export const messagesByChannel = writable<Map<string, ChatMessage[]>>(new Map());

export const activeMessages = derived(
  [messagesByChannel, activeChannelId],
  ([$messagesByChannel, $activeChannelId]) =>
    $activeChannelId ? ($messagesByChannel.get($activeChannelId) ?? []) : []
);

export function appendMessage(message: ChatMessage) {
  messagesByChannel.update((current) => {
    const next = new Map(current);
    next.set(message.channelId, [...(next.get(message.channelId) ?? []), message]);
    return next;
  });
}
