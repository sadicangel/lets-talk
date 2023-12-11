<script lang="ts">
    import { channelList$ } from '$lib/stores/channelList';
    import { messageList$ } from '$lib/stores/messageList';
    import { userProfile$ } from '$lib/stores/userProfiel';
    import Message from './Message.svelte';

    export let channelId: string;

    $: channel = $channelList$.channels[channelId];
</script>

<div class="flex-1 bg-surface-50">
    <header class="p-4 variant-filled-surface text-2xl font-semibold">
        {channel.channelName}
    </header>
    <div class="overflow-y-auto p-4">
        {#if Array.isArray($messageList$[channel.channelId])}
            {#each $messageList$[channel.channelId] as message}
                <Message {message} justifyStart={message.senderId !== $userProfile$.userId} />
            {/each}
        {/if}
    </div>
</div>
