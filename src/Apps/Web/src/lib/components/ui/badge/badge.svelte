<script lang="ts">
  import type { Snippet } from 'svelte';
  import type { HTMLAttributes } from 'svelte/elements';
  import { type VariantProps, tv } from 'tailwind-variants';
  import { cn } from '$lib/utils';

  export const badgeVariants = tv({
    base: 'inline-flex w-fit shrink-0 items-center justify-center gap-1 overflow-hidden rounded-md border px-2 py-0.5 text-xs font-medium whitespace-nowrap transition-colors [&>svg]:size-3',
    variants: {
      variant: {
        default: 'border-transparent bg-primary text-primary-foreground',
        secondary: 'border-transparent bg-secondary text-secondary-foreground',
        destructive: 'border-transparent bg-destructive text-white',
        outline: 'text-foreground'
      }
    },
    defaultVariants: {
      variant: 'default'
    }
  });

  type BadgeVariant = VariantProps<typeof badgeVariants>['variant'];

  let {
    class: className,
    variant = 'default',
    children,
    ...restProps
  }: HTMLAttributes<HTMLSpanElement> & {
    variant?: BadgeVariant;
    children?: Snippet;
  } = $props();
</script>

<span class={cn(badgeVariants({ variant }), className)} {...restProps}>
  {@render children?.()}
</span>
