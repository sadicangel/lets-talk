<script lang="ts">
  import * as Card from '$lib/components/ui/card';

  let { children } = $props();
</script>

<main class="grid min-h-screen place-items-center bg-background p-6">
  <Card.Card class="w-full max-w-[430px] shadow-xl">
    <Card.CardHeader class="pb-2">
      <div class="flex items-center gap-3">
        <span class="grid size-12 place-items-center rounded-lg bg-primary text-sm font-black text-primary-foreground">LT</span>
        <div>
          <Card.CardTitle class="text-2xl">Lets Talk</Card.CardTitle>
          <Card.CardDescription>Realtime channels for small teams.</Card.CardDescription>
        </div>
      </div>
    </Card.CardHeader>
    <Card.CardContent>
      {@render children()}
    </Card.CardContent>
  </Card.Card>
</main>
