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
    <Label for="email">Email</Label>
    <Input id="email" name="email" type="email" autocomplete="email" value={form?.email ?? ''} required />
  </div>
  <div class="grid gap-2">
    <Label for="avatarUrl">Avatar URL</Label>
    <Input id="avatarUrl" name="avatarUrl" type="url" value={form?.avatarUrl ?? ''} />
  </div>
  <div class="grid gap-2">
    <Label for="password">Password</Label>
    <Input id="password" name="password" type="password" autocomplete="new-password" required />
  </div>
  {#if form?.message}
    <p class="text-sm font-medium text-destructive">{form.message}</p>
  {/if}
  {#if form?.details?.length}
    <ul class="grid gap-1 pl-5 text-sm text-destructive">
      {#each form.details as detail}
        <li class="list-disc">{detail}</li>
      {/each}
    </ul>
  {/if}
  <Button class="w-full" type="submit">Create account</Button>
  <p class="text-center text-sm text-muted-foreground">
    Already registered? <a class="font-semibold text-primary underline-offset-4 hover:underline" href="/login">Log in</a>
  </p>
</form>
