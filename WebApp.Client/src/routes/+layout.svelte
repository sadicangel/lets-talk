<script lang="ts">
    import { Toast, getToastStore, initializeStores } from '@skeletonlabs/skeleton';
    import { AppBar, AppShell, Avatar } from '@skeletonlabs/skeleton';
    import '../app.pcss';
    import { hubNotification$ } from '$lib/stores/hubNotification';
    import type { LayoutData } from './$types';
    import { userProfile$ } from '$lib/stores/userProfile';
    import { goto } from '$app/navigation';
    import { onMount } from 'svelte';

    initializeStores();

    const toastStore = getToastStore();

    export let data: LayoutData;

    hubNotification$.subscribe((notification) => {
        if (!notification) {
            return;
        }
        switch (notification.eventType) {
            case 'UserConnected':
                toastStore.trigger({
                    message: `${notification.userName} is online`,
                    timeout: 5000,
                    background: 'variant-filled-primary'
                });
                return;
            case 'NotificationBroadcast':
                toastStore.trigger({
                    message: notification.content,
                    timeout: 5000,
                    background: 'variant-filled-warning'
                });
                return;
        }
    });

    onMount(() => {
        if (!data.user.isAuthenticated) {
            goto('/login');
        }
    });
</script>

<Toast position="br" />

<AppShell
    class="card h-full p-1"
    slotSidebarLeft="grid grid-cols-1 w-1/6"
    slotSidebarRight="grid grid-cols-1 w-1/6"
    slotPageContent="grid grid-cols-1"
>
    <!-- Header-->
    <svelte:fragment slot="header">
        <AppBar class="w-full text-primary-100" background="bg-secondary-900">
            <svelte:fragment slot="trail">
                <div class="flex flex-row gap-2 items-end absolute right-0 mr-4">
                    {#if $userProfile$}
                        <div><a href="/logout">Logout</a></div>
                        <div>{$userProfile$.userName}</div>
                        <div>
                            <Avatar
                                src={$userProfile$.userAvatar}
                                alt="{$userProfile$.userName} Icon"
                                rounded="rounded-full"
                                width="w-10"
                            />
                        </div>
                    {:else}
                        <div><a href="/login">Login</a></div>
                    {/if}
                </div>
            </svelte:fragment>
            <span class="text-xl text-primary-50 uppercase">Let's Talk</span>
        </AppBar>
    </svelte:fragment>

    <!-- Router Slot -->
    <slot />

    <!-- Footer -->
    <svelte:fragment slot="footer">
        <div class="text-base w-full items-center text-center">
            Copyright © {new Date().getUTCFullYear()} Let's Talk
        </div>
    </svelte:fragment>
</AppShell>
