<script lang="ts">
    // let createChannelName = undefined as unknown as string;

    // async function createChannel() {
    //     console.log(await api.channel.create(createChannelName));
    // }

    // let updateChannelId = undefined as unknown as string;
    // let updateChannelName = undefined as unknown as string;
    // let updateChannelIcon = undefined as unknown as string;

    // async function updateChannel() {
    //     console.log(
    //         await api.channel.update(updateChannelId, updateChannelName, updateChannelIcon)
    //     );
    // }

    // let deleteChannelId = undefined as unknown as string;

    // async function deleteChannel() {
    //     console.log(await api.channel.delete(deleteChannelId));
    // }

    // let joinChannelId = undefined as unknown as string;

    // async function joinChannel() {
    //     await $hubConnection$.send('JoinChannel', joinChannelId);
    // }

    // let leaveChannelId = undefined as unknown as string;

    // async function leaveChannel() {
    //     await $hubConnection$.send('LeaveChannel', leaveChannelId);
    // }
</script>

<div class="text-4xl">Welcome to Let's Talk</div>

<!-- {#if $hubConnectionStatus$.isConnected}
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
        {#if selectedChannel}
            <Channel channelId={selectedChannel.channelId} />
        {/if}
    </div>
{/if} -->
