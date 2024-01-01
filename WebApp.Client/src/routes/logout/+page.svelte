<script lang="ts">
    import { userProfile$ } from '$lib/stores/userProfile';
    import { ProgressRadial } from '@skeletonlabs/skeleton';
    import { onMount } from 'svelte';
    import api from '$lib/api';
    import { goto } from '$app/navigation';
    import FormSection from '$lib/components/forms/FormSection.svelte';

    export let data;

    onMount(async () => {
        if (data.user.isAuthenticated) {
            await api.account.logout().send();
            userProfile$.set(undefined);
        }
        goto('/login');
    });
</script>

<FormSection>
    <p class="text-center">Logging out...</p>
    <ProgressRadial
        meter="stroke-primary-500"
        track="stroke-primary-500/30"
        width="w-1/2"
        class="m-auto"
    /></FormSection
>
