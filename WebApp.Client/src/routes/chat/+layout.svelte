<script lang="ts">
    import RedirectGuard from '$lib/components/RedirectGuard.svelte';
    import ChannelList from '$lib/components/ChannelList.svelte';
    import { channelList$ } from '$lib/stores/channelList';
    import { hubConnection$ } from '$lib/stores/hubConnection';
    import { hubConnectionStatus$ } from '$lib/stores/hubConnectionStatus';
    import { messageList$ } from '$lib/stores/messageList';
    import { userProfile$ } from '$lib/stores/userProfile';
    import { encodeText } from '$lib/utf8';
    import { AppShell } from '@skeletonlabs/skeleton';
    import { onMount } from 'svelte';

    onMount(async () => {
        await hubConnection$.connect();

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
            eventType: 'MessageBroadcast',
            timestamp: new Date().toISOString()
        });

        await $hubConnection$.send(
            'SendMessage',
            firstChannel.channelId,
            'text/plain',
            encodeText('Lorem ipsum dolor sit amet consectetur adipisicing elit.')
        );
    });

    async function triggerServerNotification() {
        await fetch(`/api/test/notification?message=Server Message @${new Date().toISOString()}`);
    }
</script>

<RedirectGuard redirectUrl="/login" canPassGuard={!!$userProfile$}>
    <AppShell
        class="card h-full p-1"
        slotSidebarLeft="grid grid-cols-1 w-1/6"
        slotSidebarRight="grid grid-cols-1 w-1/6"
        slotPageContent="grid grid-cols-1"
    >
        <!-- Left SideBar-->
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

        <!-- Router Slot -->
        <slot />

        <!-- Right SideBar-->
        <svelte:fragment slot="sidebarRight">
            <div class="flex flex-col m-2 max-h-8 gap-2 items-center">
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
                <button
                    class="basis-2/3 button-base-styles variant-filled rounded-lg"
                    on:click={triggerServerNotification}>Trigger server notification</button
                >
            </div>
        </svelte:fragment>
    </AppShell>
</RedirectGuard>
