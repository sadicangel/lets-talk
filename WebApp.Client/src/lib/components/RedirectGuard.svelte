<script lang="ts">
    import { browser } from '$app/environment';
    import { goto } from '$app/navigation';
    import { onMount } from 'svelte';

    export let redirectUrl: string;
    export let canPassGuard = false;

    onMount(async () => {
        if (browser) {
            if (!canPassGuard) {
                goto(redirectUrl);
            }
        }
    });
</script>

{#if canPassGuard}
    <slot />
{/if}
