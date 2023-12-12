<script lang="ts">
    import type { MessageBroadcast } from '$lib/events';
    import { decodeText } from '$lib/utf8';
    import { Avatar } from '@skeletonlabs/skeleton';

    export let message: MessageBroadcast;
    export let justifyStart: boolean;

    // const timestamp = new Date(message.timestamp);

    // function getDate(date: Date): string {
    //     const today = new Date();
    //     if (today.getFullYear() === date.getFullYear()) {
    //         if (today.getMonth() == date.getMonth() && today.getDate() == date.getDate())
    //             return 'today';
    //         today.setDate(today.getDate() - 1);
    //         if (today.getMonth() == date.getMonth() && today.getDate() == date.getDate())
    //             return 'yesterday';
    //     }
    //     return timestamp.toLocaleDateString();
    // }
</script>

<div class="message {justifyStart ? 'justify-start' : 'justify-end'}">
    {#if justifyStart}
        <div class="mr-2">
            <Avatar src={message.senderAvatar} alt="{message.senderName} Avatar" width="w-8" />
        </div>
    {/if}
    <div
        class="message-text max-w-96 {justifyStart
            ? 'variant-filled-secondary'
            : 'variant-filled-primary'}"
    >
        {#if message.contentType === 'text/plain'}
            <p>{decodeText(message.content)}</p>
        {:else}
            <p>{decodeText(message.content)}</p>
        {/if}
    </div>
    {#if !justifyStart}
        <div class="ml-2">
            <Avatar src={message.senderAvatar} alt="{message.senderName} Avatar" width="w-8" />
        </div>
    {/if}
</div>

<style lang="postcss">
    .message {
        @apply mb-4 flex cursor-pointer items-center;
    }

    .message-text {
        @apply flex gap-3 rounded-lg p-3;
    }
</style>
