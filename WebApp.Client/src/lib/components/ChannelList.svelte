<script lang="ts">
    import { goto } from '$app/navigation';
    import { channelList$ } from '$lib/stores/channelList';
    import { Avatar, ListBox, ListBoxItem } from '@skeletonlabs/skeleton';

    let selectedChannelId: string = undefined as any;

    function selectChannel() {
        goto(`/chat/${selectedChannelId}`);
    }
</script>

<div class="w-full card p-4 text-token">
    <ListBox
        >{#each $channelList$.channels as channel}
            <ListBoxItem
                bind:group={selectedChannelId}
                name={channel.channelId}
                value={channel.channelId}
                on:change={selectChannel}
            >
                <svelte:fragment slot="lead"
                    ><Avatar src={channel.channelIcon} alt={channel.channelName} width="w-6"
                    ></Avatar></svelte:fragment
                >
                <span class="uppercase font-semibold">{channel.channelName}</span></ListBoxItem
            >
        {/each}
    </ListBox>
</div>
