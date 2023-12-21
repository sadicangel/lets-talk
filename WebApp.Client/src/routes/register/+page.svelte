<script lang="ts">
    import RedirectGuard from '$lib/components/RedirectGuard.svelte';
    import FormButton from '$lib/components/forms/FormButton.svelte';
    import FormCheckbox from '$lib/components/forms/FormCheckbox.svelte';
    import FormInput from '$lib/components/forms/FormInput.svelte';
    import FormSection from '$lib/components/forms/FormSection.svelte';
    import { userProfile$ } from '$lib/stores/userProfile';

    let email: string;
    let username: string;
    let password: string;
    let confirmPassword: string;
    let acceptTerms: boolean;

    async function register() {
        console.log(email, username, password, confirmPassword);
    }
</script>

<RedirectGuard redirectUrl="/chat" canPassGuard={!$userProfile$}>
    <FormSection title="Create an account">
        <FormInput label="Email" value={email} placeholder="user@lt.com" />
        <FormInput label="Username" value={username} placeholder="username" />
        <FormInput label="Password" value={password} placeholder="**********" />
        <FormInput label="Confirm password" value={confirmPassword} placeholder="**********" />
        <FormCheckbox value={acceptTerms} required={true}
            >I accept the <a
                class="font-medium text-primary-600 hover:underline dark:text-primary-500"
                href="/">Terms and Conditions</a
            ></FormCheckbox
        >
        <FormButton value="Create account" click={register} />
        <p class="text-sm font-light text-gray-500 dark:text-gray-400">
            Already have an account? <a
                href="/login"
                class="font-medium text-primary-600 hover:underline dark:text-primary-500">Login</a
            >
        </p>
    </FormSection>
</RedirectGuard>
