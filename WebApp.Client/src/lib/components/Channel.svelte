<script lang="ts">
    import { channelList$ } from '$lib/stores/channelList';
    import { hubConnection$ } from '$lib/stores/hubConnection';
    import { messageList$ } from '$lib/stores/messageList';
    import { userProfile$ } from '$lib/stores/userProfiel';
    import { encodeText } from '$lib/utf8';
    import Message from './Message.svelte';
    import { Avatar } from '@skeletonlabs/skeleton';

    export let channelId: string;

    $: channel = $channelList$.channels[channelId];

    let textContent: string = '';

    async function send() {
        await $hubConnection$.send(
            'SendMessage',
            channel.channelId,
            'text/plain',
            encodeText(textContent)
        );
        textContent = '';
    }
</script>

<div class="flex-1 bg-surface-50">
    <header
        class="p-3 variant-filled-surface text-2xl font-semibold uppercase flex items-center justify-start"
    >
        <div class="mr-2">
            <Avatar
                src={channel.channelIcon}
                alt="{channel.channelName} Icon"
                rounded="rounded-xl"
                width="w-10"
            />
        </div>
        <div class="">
            <p>{channel.channelName}</p>
        </div>
    </header>
    <div class="overflow-y-auto p-4">
        {#if Array.isArray($messageList$[channel.channelId])}
            {#each $messageList$[channel.channelId] as message}
                <Message {message} justifyStart={message.senderId !== $userProfile$.userId} />
            {/each}
        {/if}
    </div>
    <!-- TODO: fix element position -->
    <footer class="p-4 absolute w-4/6 items-center bottom-3 variant-filled-surface">
        <div class="flex items-center">
            <input
                type="text"
                placeholder="Type a message..."
                class="w-full p-2 rounded-md border focus:outline-none focus:border-secondary-700 text-black placeholder:text-surface-400"
                bind:value={textContent}
            />
            <button
                class="px-4 py-2 rounded-md ml-2 border bg-surface-700 hover:bg-surface-800 transition-colors hover:cursor-pointer disabled:bg-surface-400 disabled:hover:bg-surface-400 disabled:hover:cursor-default"
                disabled={!textContent}
                on:click={send}>Send</button
            >
        </div>
    </footer>
</div>
