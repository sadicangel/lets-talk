<script lang="ts">
    import { channelList$ } from '$lib/stores/channelList';
    import type { PageData } from '../$types';

    export let data: PageData;

    channelList$.update((s) => {
        for (const channel of data.channels!) s.channels[channel.channelId] = channel;
        return s;
    });
</script>

<div class="text-2xl text-center">Placeholder</div>
