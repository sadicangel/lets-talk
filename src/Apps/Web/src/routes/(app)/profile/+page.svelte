<script lang="ts">
  import { enhance } from '$app/forms';
  import * as Avatar from '$lib/components/ui/avatar';
  import { Button } from '$lib/components/ui/button';
  import * as Card from '$lib/components/ui/card';
  import { Input } from '$lib/components/ui/input';
  import { Label } from '$lib/components/ui/label';

  let { data, form } = $props();
  const current = $derived(form?.profile ?? data.profile);
</script>

<main class="mx-auto grid w-full max-w-3xl gap-6 px-5 py-8">
  <section class="flex items-center justify-between gap-5">
    <div>
      <h1 class="text-3xl font-semibold tracking-normal">Profile</h1>
      <p class="mt-1 text-sm font-medium text-muted-foreground">{current.userName}</p>
    </div>
    <Avatar.Root class="size-18">
      {#if current.avatarUrl}
        <Avatar.Image src={current.avatarUrl} alt="" />
      {:else}
        <Avatar.Fallback>{current.userName.slice(0, 1).toUpperCase()}</Avatar.Fallback>
      {/if}
    </Avatar.Root>
  </section>

  <Card.Card>
    <Card.CardHeader>
      <Card.CardTitle>Account details</Card.CardTitle>
      <Card.CardDescription>Update how you appear in channels.</Card.CardDescription>
    </Card.CardHeader>
    <Card.CardContent>
      <form class="grid gap-4" method="POST" use:enhance>
        <div class="grid gap-2">
          <Label for="email">Email</Label>
          <Input id="email" name="email" type="email" autocomplete="email" value={form?.email ?? current.email} required />
        </div>
        <div class="grid gap-2">
          <Label for="avatarUrl">Avatar URL</Label>
          <Input id="avatarUrl" name="avatarUrl" type="url" value={form?.avatarUrl ?? current.avatarUrl ?? ''} />
        </div>
        {#if form?.message}
          <p class="text-sm font-medium text-primary">{form.message}</p>
        {/if}
        <Button class="w-fit" type="submit">Save profile</Button>
      </form>
    </Card.CardContent>
  </Card.Card>
</main>
