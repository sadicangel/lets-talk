<script lang="ts">
  import { channelList$, type UserChannelListChannel } from '$lib/stores/channelList';
  import { Avatar, ListBox, ListBoxItem } from '@skeletonlabs/skeleton';
  import { createEventDispatcher } from 'svelte';

  const dispatch = createEventDispatcher<{ channelSelected: UserChannelListChannel }>();

  let selectedChannelId: string = undefined as any;

  function selectChannel() {
    dispatch('channelSelected', $channelList$.channels[selectedChannelId]);
  }
</script>

<div class="w-full card p-4 text-token">
  <ListBox
    >{#each Object.values($channelList$.channels) as channel}
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
