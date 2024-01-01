<script lang="ts">
    import { goto } from '$app/navigation';
    import api from '$lib/api';
    import FormButton from '$lib/components/forms/FormButton.svelte';
    import FormCheckbox from '$lib/components/forms/FormCheckbox.svelte';
    import FormInput from '$lib/components/forms/FormInput.svelte';
    import FormSection from '$lib/components/forms/FormSection.svelte';
    import { userProfile$ } from '$lib/stores/userProfile';
    import { getToastStore } from '@skeletonlabs/skeleton';
    import type { PageData } from './$types';
    import { onMount } from 'svelte';

    const toastStore = getToastStore();

    export let data: PageData;

    let email: string;
    let password: string;
    let rememberMe = false;

    async function login() {
        try {
            await api.account.login(email, password, rememberMe).send();
            userProfile$.set(await api.account.profile().send());
            goto('/chat');
        } catch (e) {
            const response = e as Response;
            toastStore.trigger({
                message: await response.text(),
                timeout: 5000,
                background: 'variant-filled-error'
            });
        }
    }

    onMount(async () => {
        if (data.user.profile) {
            goto('/chat');
        }
        userProfile$.set(data.user.profile);
    });
</script>

<FormSection title="Welcome back!">
    <FormInput type="email" label="Email" placeholder="user@lt.com" bind:value={email} />
    <FormInput type="password" label="Password" placeholder="**********" bind:value={password} />
    <div class="flex items-center justify-between">
        <FormCheckbox bind:value={rememberMe}>Remember me</FormCheckbox>
        <a
            href="/forgot"
            class="text-sm font-medium text-primary-600 hover:underline dark:text-primary-500"
            >Forgot password?</a
        >
    </div>
    <FormButton value="Log in" click={login} />
    <p class="text-sm font-light text-gray-500 dark:text-gray-400">
        Don’t have an account yet? <a
            href="/register"
            class="font-medium text-primary-600 hover:underline dark:text-primary-500">Register</a
        >
    </p>
</FormSection>
