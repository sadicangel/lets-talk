<script>
    import { onMount } from "svelte";

    let result = {};

onMount(async () => {
    const response = await fetch("/api/profile");
    if(response.ok) {
        result = JSON.stringify(await response.json());
    }
    else {
        result = await response.text();
    }
})

</script>

<h1>Welcome to SvelteKit</h1>
<pre>{result}</pre>
