<script lang="ts">
    import { AppBar, AppShell, Avatar } from '@skeletonlabs/skeleton';
    import '../app.pcss';
    import { onMount } from 'svelte';
    import { userProfile$ } from '$lib/stores/userProfiel';
    import { hubConnection$ } from '$lib/stores/hubConnection';
    import { channelList$ } from '$lib/stores/channelList';
    import ChannelList from '$lib/components/ChannelList.svelte';
    import { hubConnectionStatus$ } from '$lib/stores/hubConnectionStatus';
    import { encodeText } from '$lib/utf8';
    import { messageList$ } from '$lib/stores/messageList';

    onMount(async () => {
        await userProfile$.fetch();
        await hubConnection$.connect();
        await channelList$.fetch();

        // TEST CODE:
        const firstChannel = Object.values($channelList$.channels)[0];
        messageList$.push({
            senderId: 'cb33be51-2342-488d-8882-54028d40d91c',
            senderAvatar:
                'https://api.dicebear.com/7.x/fun-emoji/svg?seed=cb33be51-2342-488d-8882-54028d40d91c',
            senderName: 'other@lt.com',
            channelId: firstChannel.channelId,
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
            firstChannel.channelId,
            'text/plain',
            encodeText('Lorem ipsum dolor sit amet consectetur adipisicing elit.')
        );
    });

    //onDestroy(async () => hubConnection$.disconnect());
</script>

<AppShell
    class="card h-full p-1"
    slotSidebarLeft="grid grid-cols-1 w-1/6"
    slotSidebarRight="grid grid-cols-1 w-1/6"
    slotPageContent="grid grid-cols-1"
>
    <svelte:fragment slot="header">
        <AppBar class="w-full text-primary-100" background="bg-secondary-900">
            <svelte:fragment slot="trail">
                <div class="flex flex-row gap-2 items-end absolute right-0 mr-4">
                    {#if $userProfile$?.userId}
                        <div>{$userProfile$.userName}</div>
                        <div>
                            <Avatar
                                src={$userProfile$.userAvatar}
                                alt="{$userProfile$.userName} Icon"
                                rounded="rounded-full"
                                width="w-10"
                            />
                        </div>
                    {/if}
                </div>
            </svelte:fragment>
            <span class="text-xl text-primary-50 uppercase">Let's Talk</span>
        </AppBar>
    </svelte:fragment>
    <svelte:fragment slot="sidebarLeft">
        <div class="flex flex-col">
            <div class="flex-1">
                <ChannelList />
            </div>
            <div class="flex-none text-center variant-filled-primary">
                Channel settings placeholder
            </div>
        </div>
    </svelte:fragment>
    <svelte:fragment slot="sidebarRight">
        <div class="flex flex-row m-2 max-h-8 gap-2 items-center">
            {#if $hubConnectionStatus$.isConnected}
                <span>status: ✅</span>
                <button
                    class="flex-1 button-base-styles variant-filled rounded-lg"
                    on:click={hubConnection$.disconnect}>Disconnect from chat</button
                >
            {:else}
                <span>status: ❌</span>
                <button
                    class="basis-2/3 button-base-styles variant-filled rounded-lg"
                    on:click={hubConnection$.connect}>Connect to chat</button
                >
            {/if}
        </div>
    </svelte:fragment>
    <!-- (pageHeader) -->
    <!-- Router Slot -->
    <slot />
    <!-- ---- / ---- -->
    <!-- (pageFooter) -->
    <!-- <svelte:fragment slot="footer">Footer</svelte:fragment> -->
</AppShell>
