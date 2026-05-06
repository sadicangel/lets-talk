<script lang="ts">
  import ImagePlus from '@lucide/svelte/icons/image-plus';
  import SendHorizontal from '@lucide/svelte/icons/send-horizontal';
  import { afterNavigate } from '$app/navigation';
  import * as Avatar from '$lib/components/ui/avatar';
  import { Badge } from '$lib/components/ui/badge';
  import { Button } from '$lib/components/ui/button';
  import { Input } from '$lib/components/ui/input';
  import { connectChat, connectionState, sendError, sendImageMessage, sendTextMessage } from '$lib/stores/chat';
  import { activeChannelId, channels } from '$lib/stores/channels';
  import { activeMessages } from '$lib/stores/messages';
  import { profile } from '$lib/stores/session';
  import { cn } from '$lib/utils';

  let { data } = $props();
  let text = $state('');
  let fileInput: HTMLInputElement;

  $effect(() => {
    channels.set(data.channels);
    activeChannelId.set(data.activeChannel.channelId);
    void connectChat();
  });

  afterNavigate(() => {
    channels.set(data.channels);
    activeChannelId.set(data.activeChannel.channelId);
  });

  async function submitMessage() {
    const channelId = data.activeChannel.channelId;
    await sendTextMessage(channelId, text);
    text = '';
  }

  async function chooseImage(event: Event) {
    const input = event.currentTarget as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    await sendImageMessage(data.activeChannel.channelId, file);
    input.value = '';
  }
</script>

<main class="grid min-h-[calc(100vh-4rem)] grid-cols-[280px_minmax(0,1fr)] max-md:grid-cols-1">
  <aside class="border-r bg-sidebar p-5 max-md:border-r-0 max-md:border-b">
    <h2 class="mb-3.5 text-xs font-semibold uppercase text-muted-foreground">Channels</h2>
    <nav class="grid gap-1.5 max-md:grid-flow-col max-md:auto-cols-[minmax(160px,1fr)] max-md:overflow-x-auto" aria-label="Channels">
      {#each $channels as channel}
        <a
          class={cn(
            'grid min-h-14 content-center gap-0.5 rounded-md px-3 py-2 text-sm text-sidebar-foreground no-underline transition-colors hover:bg-sidebar-accent hover:text-sidebar-accent-foreground',
            channel.channelId === data.activeChannel.channelId && 'bg-sidebar-accent text-sidebar-accent-foreground'
          )}
          href={`/channels/${channel.channelId}`}
        >
          <span class="font-semibold">{channel.channelName}</span>
          <small class="text-muted-foreground">{channel.members.length} members</small>
        </a>
      {/each}
    </nav>
  </aside>

  <section class="grid min-h-[calc(100vh-4rem)] min-w-0 grid-rows-[auto_1fr_auto_auto]">
    <header class="flex min-h-19 items-center justify-between gap-4 border-b bg-card px-5 py-4">
      <div>
        <h1 class="text-xl font-semibold tracking-normal">{data.activeChannel.channelName}</h1>
        {#if data.activeChannel.description}
          <p class="mt-1 text-sm text-muted-foreground">{data.activeChannel.description}</p>
        {/if}
      </div>
      <Badge variant={$connectionState === 'connected' ? 'default' : 'secondary'} class="capitalize">{$connectionState}</Badge>
    </header>

    <div class="flex min-h-0 flex-col gap-3.5 overflow-y-auto p-5" aria-live="polite">
      {#if $activeMessages.length === 0}
        <div class="m-auto text-sm font-semibold text-muted-foreground">No messages in this tab yet.</div>
      {:else}
        {#each $activeMessages as message (message.id)}
          {@const mine = message.author.userId === $profile?.userId}
          <article class={cn('flex items-start gap-2.5', mine && 'justify-end')}>
            <Avatar.Root class={cn(mine && 'order-2')}>
              {#if message.author.avatarUrl}
                <Avatar.Image src={message.author.avatarUrl} alt="" />
              {:else}
                <Avatar.Fallback class="bg-chart-1 text-white">{message.author.userName.slice(0, 1).toUpperCase()}</Avatar.Fallback>
              {/if}
            </Avatar.Root>
            <div class={cn('min-w-0 max-w-[680px] rounded-lg bg-card px-3.5 py-3 shadow-xs', mine && 'bg-primary text-primary-foreground')}>
              <div class={cn('mb-1.5 flex items-baseline gap-2.5 text-sm text-muted-foreground', mine && 'text-primary-foreground/80')}>
                <strong>{message.author.userName}</strong>
                <time class="text-xs font-semibold" datetime={message.timestamp}>{new Date(message.timestamp).toLocaleTimeString()}</time>
              </div>
              {#if message.imageUrl}
                <img class="block max-h-[360px] max-w-[min(420px,100%)] rounded-md object-contain" src={message.imageUrl} alt="Uploaded message" />
              {:else}
                <p class="whitespace-pre-wrap break-anywhere">{message.text}</p>
              {/if}
            </div>
          </article>
        {/each}
      {/if}
    </div>

    <form class="grid grid-cols-[minmax(0,1fr)_auto_auto] gap-2.5 border-t bg-card px-5 py-4 max-md:grid-cols-1" onsubmit={(event) => { event.preventDefault(); void submitMessage(); }}>
      <Input bind:value={text} class="h-11" placeholder="Message {data.activeChannel.channelName}" aria-label="Message" />
      <input
        bind:this={fileInput}
        class="sr-only"
        type="file"
        accept="image/*"
        onchange={chooseImage}
      />
      <Button variant="outline" type="button" title="Attach image" onclick={() => fileInput.click()}>
        <ImagePlus />
        Image
      </Button>
      <Button type="submit">
        <SendHorizontal />
        Send
      </Button>
    </form>
    {#if $sendError}
      <p class="bg-card px-5 pb-4 text-sm font-medium text-destructive">{$sendError}</p>
    {/if}
  </section>
</main>
