<script lang="ts">
  import { enhance } from '$app/forms';
  import { Button } from '$lib/components/ui/button';
  import { Input } from '$lib/components/ui/input';
  import { Label } from '$lib/components/ui/label';

  let { form } = $props();
</script>

<form class="grid gap-4" method="POST" use:enhance>
  <div class="grid gap-2">
    <Label for="userName">Username</Label>
    <Input id="userName" name="userName" autocomplete="username" value={form?.userName ?? ''} required />
  </div>
  <div class="grid gap-2">
    <Label for="password">Password</Label>
    <Input id="password" name="password" type="password" autocomplete="current-password" required />
  </div>
  {#if form?.message}
    <p class="text-sm font-medium text-destructive">{form.message}</p>
  {/if}
  <Button class="w-full" type="submit">Log in</Button>
  <p class="text-center text-sm text-muted-foreground">
    New here? <a class="font-semibold text-primary underline-offset-4 hover:underline" href="/register">Create an account</a>
  </p>
</form>
