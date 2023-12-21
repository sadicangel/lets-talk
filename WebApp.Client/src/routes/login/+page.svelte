<script lang="ts">
    import { goto } from '$app/navigation';
    import RedirectGuard from '$lib/components/RedirectGuard.svelte';
    import FormButton from '$lib/components/forms/FormButton.svelte';
    import FormCheckbox from '$lib/components/forms/FormCheckbox.svelte';
    import FormInput from '$lib/components/forms/FormInput.svelte';
    import FormSection from '$lib/components/forms/FormSection.svelte';
    import { userProfile$ } from '$lib/stores/userProfile';
    import { getToastStore } from '@skeletonlabs/skeleton';

    const toastStore = getToastStore();

    let email: string;
    let password: string;
    let rememberMe = false;

    async function login() {
        const response = await fetch('/api/account/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ email, password })
        });

        if (!response.ok) {
            toastStore.trigger({
                message: await response.text(),
                timeout: 5000,
                background: 'variant-filled-error'
            });
        } else {
            await userProfile$.fetch();
            goto('/chat');
        }
    }
</script>

<RedirectGuard redirectUrl="/chat" canPassGuard={!$userProfile$}>
    <FormSection title="Welcome back!">
        <FormInput type="email" label="Email" placeholder="user@lt.com" bind:value={email} />
        <FormInput
            type="password"
            label="Password"
            placeholder="**********"
            bind:value={password}
        />
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
                class="font-medium text-primary-600 hover:underline dark:text-primary-500"
                >Register</a
            >
        </p>
    </FormSection>
</RedirectGuard>
