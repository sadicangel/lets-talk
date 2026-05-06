<script lang="ts">
  import { enhance } from '$app/forms';

  let { data, form } = $props();
  const current = $derived(form?.profile ?? data.profile);
</script>

<main class="profile-page">
  <section>
    <div>
      <h1>Profile</h1>
      <p>{current.userName}</p>
    </div>
    {#if current.avatarUrl}
      <img src={current.avatarUrl} alt="" />
    {/if}
  </section>

  <form method="POST" use:enhance>
    <label>
      Email
      <input name="email" type="email" autocomplete="email" value={form?.email ?? current.email} required />
    </label>
    <label>
      Avatar URL
      <input name="avatarUrl" type="url" value={form?.avatarUrl ?? current.avatarUrl ?? ''} />
    </label>
    {#if form?.message}
      <p class="success">{form.message}</p>
    {/if}
    <button type="submit">Save profile</button>
  </form>
</main>

<style>
  .profile-page {
    display: grid;
    gap: 24px;
    width: min(100%, 720px);
    margin: 0 auto;
    padding: 32px 20px;
  }

  section {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 20px;
  }

  h1,
  p {
    margin: 0;
  }

  h1 {
    font-size: 2rem;
  }

  p {
    margin-top: 4px;
    color: #60706a;
    font-weight: 700;
  }

  img {
    width: 72px;
    height: 72px;
    border-radius: 8px;
    object-fit: cover;
  }

  form {
    display: grid;
    gap: 16px;
    padding: 24px;
    border: 1px solid #dbe1dc;
    border-radius: 8px;
    background: #ffffff;
  }

  label {
    display: grid;
    gap: 7px;
    color: #40504a;
    font-weight: 800;
  }

  input {
    min-height: 44px;
    border: 1px solid #cdd7d1;
    border-radius: 7px;
    padding: 0 12px;
  }

  input:focus {
    border-color: #1a6d5c;
    outline: 3px solid rgba(26, 109, 92, 0.14);
  }

  button {
    width: fit-content;
    min-height: 42px;
    border: 0;
    border-radius: 7px;
    padding: 0 16px;
    background: #1a6d5c;
    color: #ffffff;
    font-weight: 900;
  }

  .success {
    color: #1a6d5c;
  }
</style>
