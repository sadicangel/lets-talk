<script lang="ts">
  import type { Snippet } from 'svelte';
  import type { HTMLAttributes } from 'svelte/elements';
  import { cn } from '$lib/utils';

  let { class: className, children, ...restProps }: HTMLAttributes<HTMLSpanElement> & { children?: Snippet } = $props();
</script>

<span class={cn('bg-muted flex size-full items-center justify-center rounded-lg font-semibold', className)} {...restProps}>
  {@render children?.()}
</span>
