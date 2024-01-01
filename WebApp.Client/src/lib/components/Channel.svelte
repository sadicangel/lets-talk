<script lang="ts">
    import { hub } from '$lib/hub';
    import { channelList$ } from '$lib/stores/channelList';
    import { messageList$ } from '$lib/stores/messageList';
    import { userProfile$ } from '$lib/stores/userProfile';
    import { onMount } from 'svelte';
    import Message from './Message.svelte';
    import { Avatar } from '@skeletonlabs/skeleton';
    import { get } from 'svelte/store';
    import { goto } from '$app/navigation';

    export let channelId: string;

    $: channel = $channelList$.channels.find((c) => c.channelId === channelId)!;

    let textContent: string = '';

    async function send() {
        await hub.send(channel.channelId, 'text/plain', textContent);
        textContent = '';
    }

    onMount(() => {
        if (!get(userProfile$)) goto('/login');
    });
</script>

<!-- TODO: fix max height for scrolling -->
{#if channel}
    <div class="bg-surface-50 relative">
        <header class="p-3 variant-filled-surface text-2xl font-semibold uppercase">
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
                    <Message {message} justifyStart={message.senderId !== $userProfile$?.userId} />
                {/each}
            {/if}
        </div>
        <!-- TODO: fix element position -->
        <footer class="p-4 absolute w-full items-center bottom-0 variant-filled-surface">
            <form on:submit|preventDefault>
                <div class="flex items-center">
                    <input
                        type="text"
                        placeholder="Type a message..."
                        class="w-full p-2 rounded-md border focus:outline-none focus:border-secondary-700 text-black placeholder:text-surface-400"
                        bind:value={textContent}
                    />
                    <button
                        type="submit"
                        class="px-4 py-2 rounded-md ml-2 border bg-surface-700 hover:bg-surface-800 transition-colors hover:cursor-pointer disabled:bg-surface-400 disabled:hover:bg-surface-400 disabled:hover:cursor-default"
                        disabled={!textContent}
                        on:click={send}>Send</button
                    >
                </div>
            </form>
        </footer>
    </div>
{/if}
