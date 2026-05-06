<script lang="ts">
  import { afterNavigate } from '$app/navigation';
  import { connectChat, connectionState, sendError, sendImageMessage, sendTextMessage } from '$lib/stores/chat';
  import { activeChannelId, channels } from '$lib/stores/channels';
  import { activeMessages } from '$lib/stores/messages';
  import { profile } from '$lib/stores/session';

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

<main class="chat-layout">
  <aside>
    <h2>Channels</h2>
    <nav aria-label="Channels">
      {#each $channels as channel}
        <a
          class:active={channel.channelId === data.activeChannel.channelId}
          href={`/channels/${channel.channelId}`}
        >
          <span>{channel.channelName}</span>
          <small>{channel.members.length} members</small>
        </a>
      {/each}
    </nav>
  </aside>

  <section class="chat-panel">
    <header>
      <div>
        <h1>{data.activeChannel.channelName}</h1>
        {#if data.activeChannel.description}
          <p>{data.activeChannel.description}</p>
        {/if}
      </div>
      <span class="status" data-state={$connectionState}>{$connectionState}</span>
    </header>

    <div class="messages" aria-live="polite">
      {#if $activeMessages.length === 0}
        <div class="empty">No messages in this tab yet.</div>
      {:else}
        {#each $activeMessages as message (message.id)}
          <article class:mine={message.author.userId === $profile?.userId}>
            <div class="avatar">
              {#if message.author.avatarUrl}
                <img src={message.author.avatarUrl} alt="" />
              {:else}
                <span>{message.author.userName.slice(0, 1).toUpperCase()}</span>
              {/if}
            </div>
            <div class="bubble">
              <div class="meta">
                <strong>{message.author.userName}</strong>
                <time datetime={message.timestamp}>{new Date(message.timestamp).toLocaleTimeString()}</time>
              </div>
              {#if message.imageUrl}
                <img class="message-image" src={message.imageUrl} alt="Uploaded message" />
              {:else}
                <p>{message.text}</p>
              {/if}
            </div>
          </article>
        {/each}
      {/if}
    </div>

    <form class="composer" onsubmit={(event) => { event.preventDefault(); void submitMessage(); }}>
      <input bind:value={text} placeholder="Message {data.activeChannel.channelName}" aria-label="Message" />
      <input
        bind:this={fileInput}
        class="visually-hidden"
        type="file"
        accept="image/*"
        onchange={chooseImage}
      />
      <button class="secondary" type="button" title="Attach image" onclick={() => fileInput.click()}>
        Image
      </button>
      <button type="submit">Send</button>
    </form>
    {#if $sendError}
      <p class="send-error">{$sendError}</p>
    {/if}
  </section>
</main>

<style>
  .chat-layout {
    display: grid;
    grid-template-columns: 280px minmax(0, 1fr);
    min-height: calc(100vh - 64px);
  }

  aside {
    border-right: 1px solid #dbe1dc;
    background: #ffffff;
    padding: 20px;
  }

  aside h2 {
    margin: 0 0 14px;
    color: #60706a;
    font-size: 0.82rem;
    letter-spacing: 0;
    text-transform: uppercase;
  }

  aside nav {
    display: grid;
    gap: 6px;
  }

  aside a {
    display: grid;
    gap: 3px;
    min-height: 56px;
    justify-content: center;
    border-radius: 7px;
    padding: 8px 12px;
    color: #40504a;
    text-decoration: none;
  }

  aside a.active {
    background: #e4f0eb;
    color: #174f44;
  }

  aside span {
    font-weight: 900;
  }

  aside small {
    color: #6d7c76;
  }

  .chat-panel {
    display: grid;
    grid-template-rows: auto 1fr auto auto;
    min-width: 0;
    min-height: calc(100vh - 64px);
  }

  .chat-panel > header {
    display: flex;
    min-height: 76px;
    align-items: center;
    justify-content: space-between;
    gap: 16px;
    padding: 16px 22px;
    border-bottom: 1px solid #dbe1dc;
    background: #fbfcfb;
  }

  h1,
  p {
    margin: 0;
  }

  h1 {
    font-size: 1.35rem;
  }

  header p {
    margin-top: 3px;
    color: #60706a;
  }

  .status {
    border-radius: 999px;
    padding: 6px 10px;
    background: #e7e2d4;
    color: #6a4a17;
    font-size: 0.8rem;
    font-weight: 900;
  }

  .status[data-state='connected'] {
    background: #dceee6;
    color: #176149;
  }

  .messages {
    display: flex;
    flex-direction: column;
    gap: 14px;
    min-height: 0;
    overflow-y: auto;
    padding: 22px;
  }

  .empty {
    margin: auto;
    color: #60706a;
    font-weight: 800;
  }

  article {
    display: grid;
    grid-template-columns: 40px minmax(0, 680px);
    gap: 10px;
    align-items: flex-start;
  }

  article.mine {
    grid-template-columns: minmax(0, 680px) 40px;
    justify-content: end;
  }

  article.mine .avatar {
    grid-column: 2;
  }

  article.mine .bubble {
    grid-column: 1;
    grid-row: 1;
    background: #1a6d5c;
    color: #ffffff;
  }

  article.mine .meta,
  article.mine time {
    color: rgba(255, 255, 255, 0.78);
  }

  .avatar {
    width: 40px;
    height: 40px;
    overflow: hidden;
    border-radius: 8px;
    background: #c45532;
    color: #ffffff;
  }

  .avatar img {
    width: 100%;
    height: 100%;
    object-fit: cover;
  }

  .avatar span {
    display: grid;
    height: 100%;
    place-items: center;
    font-weight: 900;
  }

  .bubble {
    min-width: 0;
    border-radius: 8px;
    padding: 11px 13px;
    background: #ffffff;
    box-shadow: 0 1px 0 rgba(23, 32, 38, 0.08);
  }

  .meta {
    display: flex;
    gap: 10px;
    align-items: baseline;
    margin-bottom: 6px;
    color: #40504a;
  }

  time {
    color: #60706a;
    font-size: 0.78rem;
    font-weight: 800;
  }

  .bubble p {
    white-space: pre-wrap;
    overflow-wrap: anywhere;
  }

  .message-image {
    display: block;
    max-width: min(420px, 100%);
    max-height: 360px;
    border-radius: 7px;
    object-fit: contain;
  }

  .composer {
    display: grid;
    grid-template-columns: minmax(0, 1fr) auto auto;
    gap: 10px;
    padding: 16px 22px;
    border-top: 1px solid #dbe1dc;
    background: #ffffff;
  }

  .composer input:not([type]) {
    min-height: 44px;
    border: 1px solid #cdd7d1;
    border-radius: 7px;
    padding: 0 13px;
  }

  .composer button {
    min-height: 44px;
    border: 0;
    border-radius: 7px;
    padding: 0 16px;
    background: #1a6d5c;
    color: #ffffff;
    font-weight: 900;
  }

  .composer .secondary {
    border: 1px solid #cdd7d1;
    background: #ffffff;
    color: #40504a;
  }

  .send-error {
    padding: 0 22px 16px;
    color: #a23b2a;
    font-weight: 800;
  }

  @media (max-width: 760px) {
    .chat-layout {
      grid-template-columns: 1fr;
    }

    aside {
      border-right: 0;
      border-bottom: 1px solid #dbe1dc;
    }

    aside nav {
      grid-auto-flow: column;
      grid-auto-columns: minmax(160px, 1fr);
      overflow-x: auto;
    }

    .composer {
      grid-template-columns: 1fr;
    }
  }
</style>
