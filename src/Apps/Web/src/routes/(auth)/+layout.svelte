<script lang="ts">
  let { children } = $props();
</script>

<main class="auth-shell">
  <section class="auth-panel">
    <div class="brand">
      <span class="mark">LT</span>
      <div>
        <h1>Lets Talk</h1>
        <p>Realtime channels for small teams.</p>
      </div>
    </div>
    {@render children()}
  </section>
</main>

<style>
  .auth-shell {
    display: grid;
    min-height: 100vh;
    place-items: center;
    padding: 24px;
    background:
      linear-gradient(135deg, rgba(26, 109, 92, 0.12), transparent 42%),
      linear-gradient(315deg, rgba(196, 85, 50, 0.12), transparent 38%),
      #f5f7f4;
  }

  .auth-panel {
    width: min(100%, 430px);
    padding: 32px;
    border: 1px solid #dbe1dc;
    border-radius: 8px;
    background: #ffffff;
    box-shadow: 0 24px 70px rgba(23, 32, 38, 0.12);
  }

  .brand {
    display: flex;
    gap: 14px;
    align-items: center;
    margin-bottom: 28px;
  }

  .mark {
    display: grid;
    width: 48px;
    height: 48px;
    place-items: center;
    border-radius: 8px;
    background: #1a6d5c;
    color: #ffffff;
    font-weight: 800;
  }

  h1,
  p {
    margin: 0;
  }

  h1 {
    font-size: 1.45rem;
  }

  p {
    margin-top: 3px;
    color: #60706a;
  }
</style>
