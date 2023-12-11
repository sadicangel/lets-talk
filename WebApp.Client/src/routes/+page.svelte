<script lang="ts">
    import { onDestroy, onMount } from 'svelte';
    import { encodeText } from '$lib/utf8';
    import { userProfile$ } from '$lib/stores/userProfiel';
    import { channelList$ } from '$lib/stores/channelList';
    import { messageList$ } from '$lib/stores/messageList';
    import { hubConnection$ } from '$lib/stores/hubConnection';
    import { hubConnectionStatus$ } from '$lib/stores/hubConnectionStatus';
    import api from '$lib/api';
    import Channel from '$lib/components/Channel.svelte';

    onMount(async () => {
        await userProfile$.fetch();

        await hubConnection$.connect();

        await channelList$.fetch();

        // TEST CODE:
        messageList$.push({
            senderId: 'cb33be51-2342-488d-8882-54028d40d91c',
            senderAvatar:
                'https://api.dicebear.com/7.x/fun-emoji/svg?seed=cb33be51-2342-488d-8882-54028d40d91c',
            senderName: 'other@lt.com',
            channelId: $channelList$.channels[0].channelId,
            channelName: 'admins',
            channelIcon:
                'https://api.dicebear.com/7.x/shapes/svg?seed=6e5312d6-57e0-4362-925d-a3881c1e5df7',
            content: encodeText('Odio harum omnis quo labore laborum. Sequi?'),
            contentType: 'text/plain',
            eventId: '',
            eventTimestamp: '',
            eventType: '',
            timestamp: new Date().toISOString()
        });

        await $hubConnection$.send(
            'SendMessage',
            $channelList$.channels[0].channelId,
            'text/plain',
            encodeText('Lorem ipsum dolor sit amet consectetur adipisicing elit.')
        );
    });

    onDestroy(async () => hubConnection$.disconnect());

    let createChannelName = undefined as unknown as string;

    async function createChannel() {
        console.log(await api.channel.create(createChannelName));
    }

    let updateChannelId = undefined as unknown as string;
    let updateChannelName = undefined as unknown as string;
    let updateChannelIcon = undefined as unknown as string;

    async function updateChannel() {
        console.log(
            await api.channel.update(updateChannelId, updateChannelName, updateChannelIcon)
        );
    }

    let deleteChannelId = undefined as unknown as string;

    async function deleteChannel() {
        console.log(await api.channel.delete(deleteChannelId));
    }

    let joinChannelId = undefined as unknown as string;

    async function joinChannel() {
        await $hubConnection$.send('JoinChannel', joinChannelId);
    }

    let leaveChannelId = undefined as unknown as string;

    async function leaveChannel() {
        await $hubConnection$.send('LeaveChannel', leaveChannelId);
    }
</script>

<h1>Let's Talk</h1>
{#if $userProfile$?.userId}
    <div>{$userProfile$.userName}</div>
    <img src={$userProfile$.userAvatar} alt="avatar" width="128" />
{/if}
<pre>{JSON.stringify($userProfile$, undefined, 2)}</pre>
<pre>{JSON.stringify($channelList$, undefined, 2)}</pre>

{#if $hubConnectionStatus$.isConnected}
    <div>
        <span>status: ✅</span>
        <button on:click={hubConnection$.disconnect}>Disconnect from chat</button>
    </div>
    <div>
        <input type="text" placeholder="channel name" bind:value={createChannelName} />
        <button disabled={!createChannelName} on:click={createChannel}>Create Channel</button>
    </div>
    <div>
        <input type="text" placeholder="channel id" bind:value={updateChannelId} />
        <input type="text" placeholder="channel name" bind:value={updateChannelName} />
        <input type="text" placeholder="channel icon" bind:value={updateChannelIcon} />
        <button disabled={!updateChannelName} on:click={updateChannel}>Update Channel</button>
    </div>
    <div>
        <input type="text" placeholder="channel id" bind:value={deleteChannelId} />
        <button disabled={!deleteChannelId} on:click={deleteChannel}>Delete Channel</button>
    </div>
    <div>
        <input type="text" placeholder="channel id" bind:value={joinChannelId} />
        <button disabled={!joinChannelId} on:click={joinChannel}>Join Channel</button>
    </div>
    <div>
        <input type="text" placeholder="channel id" bind:value={leaveChannelId} />
        <button disabled={!leaveChannelId} on:click={leaveChannel}>Leave Channel</button>
    </div>
    <div>
        {#each Object.keys($channelList$.channels) as channelId}
            <Channel {channelId} />
        {/each}
    </div>
{:else}
    <div>
        <span>status: ❌</span>
        <button on:click={hubConnection$.connect}>Connect to chat</button>
    </div>
{/if}
