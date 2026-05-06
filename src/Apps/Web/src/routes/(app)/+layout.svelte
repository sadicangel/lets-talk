<script lang="ts">
  import LogOut from '@lucide/svelte/icons/log-out';
  import { disconnectChat } from '$lib/stores/chat';
  import { Button } from '$lib/components/ui/button';
  import { accessToken, chatApiUrl, profile } from '$lib/stores/session';

  let { data, children } = $props();

  $effect(() => {
    profile.set(data.profile);
    accessToken.set(data.accessToken);
    chatApiUrl.set(data.chatApiUrl);
  });
</script>

<div class="min-h-screen bg-background">
  <header class="flex min-h-16 items-center justify-between gap-5 border-b bg-card px-6 max-sm:flex-col max-sm:items-start max-sm:p-4">
    <a class="text-xl font-black tracking-normal text-foreground no-underline" href="/channels">Lets Talk</a>
    <nav class="flex items-center gap-2 max-sm:w-full max-sm:justify-between">
      <a class="rounded-md px-3 py-2 text-sm font-semibold text-muted-foreground transition-colors hover:bg-accent hover:text-accent-foreground" href="/channels">Channels</a>
      <a class="rounded-md px-3 py-2 text-sm font-semibold text-muted-foreground transition-colors hover:bg-accent hover:text-accent-foreground" href="/profile">Profile</a>
      <form method="POST" action="/logout" onsubmit={() => void disconnectChat()}>
        <Button variant="outline" type="submit">
          <LogOut />
          Log out
        </Button>
      </form>
    </nav>
  </header>
  {@render children()}
</div>
