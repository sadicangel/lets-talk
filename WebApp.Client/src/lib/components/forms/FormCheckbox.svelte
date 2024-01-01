<script lang="ts">
    export let value: boolean | undefined;
    export let required: boolean = false;
</script>

<div class="flex items-start">
    <div class="flex items-center h-5">
        <input
            bind:value
            id="terms"
            aria-describedby="terms"
            type="checkbox"
            class="w-4 h-4 border border-gray-300 rounded bg-gray-50 focus:ring-3 focus:ring-primary-300 dark:bg-gray-700 dark:border-gray-600 dark:focus:ring-primary-600 dark:ring-offset-gray-800"
            {required}
        />
    </div>
    <div class="ml-3 text-sm">
        <label for="terms" class="font-light text-gray-500 dark:text-gray-300"><slot /></label>
    </div>
</div>
