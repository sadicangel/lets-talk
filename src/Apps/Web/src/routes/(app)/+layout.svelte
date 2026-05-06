<script lang="ts">
  import { disconnectChat } from '$lib/stores/chat';
  import { accessToken, chatApiUrl, profile } from '$lib/stores/session';

  let { data, children } = $props();

  $effect(() => {
    profile.set(data.profile);
    accessToken.set(data.accessToken);
    chatApiUrl.set(data.chatApiUrl);
  });
</script>

<div class="app-shell">
  <header>
    <a class="brand" href="/channels">Lets Talk</a>
    <nav>
      <a href="/channels">Channels</a>
      <a href="/profile">Profile</a>
      <form method="POST" action="/logout" onsubmit={() => void disconnectChat()}>
        <button type="submit">Log out</button>
      </form>
    </nav>
  </header>
  {@render children()}
</div>

<style>
  .app-shell {
    min-height: 100vh;
    background: #f5f7f4;
  }

  header {
    display: flex;
    min-height: 64px;
    align-items: center;
    justify-content: space-between;
    gap: 20px;
    padding: 0 24px;
    border-bottom: 1px solid #dbe1dc;
    background: #ffffff;
  }

  .brand {
    color: #172026;
    font-size: 1.2rem;
    font-weight: 900;
    text-decoration: none;
  }

  nav {
    display: flex;
    align-items: center;
    gap: 16px;
  }

  nav a {
    color: #40504a;
    font-weight: 800;
    text-decoration: none;
  }

  form {
    margin: 0;
  }

  button {
    min-height: 36px;
    border: 1px solid #cdd7d1;
    border-radius: 7px;
    padding: 0 13px;
    background: #ffffff;
    color: #40504a;
    font-weight: 800;
  }

  @media (max-width: 640px) {
    header {
      align-items: flex-start;
      flex-direction: column;
      padding: 16px;
    }

    nav {
      width: 100%;
      justify-content: space-between;
    }
  }
</style>
