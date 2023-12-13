<script lang="ts">
    import Channel from '$lib/components/Channel.svelte';
    import type { PageData } from './$types';

    export let data: PageData;
</script>

<Channel channelId={data.channelId} />
