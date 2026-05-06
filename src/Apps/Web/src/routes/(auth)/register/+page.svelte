<script lang="ts">
  import { enhance } from '$app/forms';

  let { form } = $props();
</script>

<form method="POST" use:enhance>
  <label>
    Username
    <input name="userName" autocomplete="username" value={form?.userName ?? ''} required />
  </label>
  <label>
    Email
    <input name="email" type="email" autocomplete="email" value={form?.email ?? ''} required />
  </label>
  <label>
    Avatar URL
    <input name="avatarUrl" type="url" value={form?.avatarUrl ?? ''} />
  </label>
  <label>
    Password
    <input name="password" type="password" autocomplete="new-password" required />
  </label>
  {#if form?.message}
    <p class="error">{form.message}</p>
  {/if}
  {#if form?.details?.length}
    <ul class="details">
      {#each form.details as detail}
        <li>{detail}</li>
      {/each}
    </ul>
  {/if}
  <button type="submit">Create account</button>
  <p class="switch">Already registered? <a href="/login">Log in</a></p>
</form>

<style>
  form {
    display: grid;
    gap: 16px;
  }

  label {
    display: grid;
    gap: 7px;
    color: #40504a;
    font-weight: 700;
  }

  input {
    min-height: 44px;
    border: 1px solid #cdd7d1;
    border-radius: 7px;
    padding: 0 12px;
    color: #172026;
    background: #fbfcfb;
  }

  input:focus {
    border-color: #1a6d5c;
    outline: 3px solid rgba(26, 109, 92, 0.14);
  }

  button {
    min-height: 44px;
    border: 0;
    border-radius: 7px;
    background: #1a6d5c;
    color: #ffffff;
    font-weight: 800;
  }

  .error {
    margin: 0;
    color: #a23b2a;
    font-weight: 700;
  }

  .details {
    display: grid;
    gap: 4px;
    margin: 0;
    padding-left: 18px;
    color: #a23b2a;
  }

  .switch {
    margin: 0;
    color: #60706a;
    text-align: center;
  }

  a {
    color: #1a6d5c;
    font-weight: 800;
  }
</style>
