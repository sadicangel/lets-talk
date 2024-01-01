<script lang="ts">
    import { goto } from '$app/navigation';
</script>

<div class="text-4xl self-center text-center">Welcome to Let's Talk</div>
<button class="variant-filled-primary" on:click={() => goto('/chat')}>Go to chat</button>
