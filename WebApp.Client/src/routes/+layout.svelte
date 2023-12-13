<script lang="ts">
    import { AppBar, AppShell, Avatar } from '@skeletonlabs/skeleton';
    import '../app.pcss';
    import { onMount } from 'svelte';
    import { userProfile$ } from '$lib/stores/userProfiel';

    onMount(async () => {
        await userProfile$.fetch();
    });

    //onDestroy(async () => hubConnection$.disconnect());
</script>

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

    <!-- Router Slot -->
    <slot />

    <!-- Footer -->
    <svelte:fragment slot="footer">
        <div class="text-xl w-full items-center text-center">Footer Placeholder</div>
    </svelte:fragment>
</AppShell>
