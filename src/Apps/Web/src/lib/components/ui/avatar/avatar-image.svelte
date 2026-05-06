<script lang="ts">
  import type { HTMLImgAttributes } from 'svelte/elements';
  import { cn } from '$lib/utils';

  let { class: className, ...restProps }: HTMLImgAttributes = $props();
</script>

<img class={cn('aspect-square size-full object-cover', className)} {...restProps} />
