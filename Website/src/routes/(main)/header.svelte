<script lang="ts">
  import Menu from 'lucide-svelte/icons/menu';
  import Close from 'lucide-svelte/icons/x';
  import { onMount } from 'svelte';

  import Routes from '$main/routes.svelte';
  import { Button, buttonVariants } from '$shadcn/components/ui/button';
  import * as Drawer from '$shadcn/components/ui/drawer';
  import { cn } from '$shadcn/utils.js.ts';

  import HeaderContents from './headerContents.svelte';

  let enhance = false;

  export let hasValidJwt: boolean;

  onMount(() => {
    enhance = true;
  });
</script>

{#if enhance}
  <Drawer.Root direction="left">
    <header id="header" class="top-0 z-50 gap-1 bg-background px-2 md:gap-2 md:px-3">
      <Drawer.Trigger
        class={cn(buttonVariants({ variant: 'ghost', size: 'sm', className: 'md:hidden' }))}
        aria-label="Open navigation">
        <Menu aria-hidden={true} class="size-6" />
      </Drawer.Trigger>
      <HeaderContents {hasValidJwt} />
      <Drawer.Portal class="md:hidden">
        <Drawer.Content
          id="drawer-content"
          class="fixed bottom-0 left-0 top-0 mt-0 w-[75%] bg-background pl-4 pr-2 pt-2">
          <div id="my-content">
            <Drawer.Close class="flex w-full flex-col pl-0">
              <Button variant="ghost" class="w-[7rem] self-end">
                Close <Close class="ml-2 mt-0.5 h-5 w-5" />
              </Button>
            </Drawer.Close>
            <Routes {hasValidJwt} drawer={true} />
          </div>
        </Drawer.Content>
      </Drawer.Portal>
    </header>
  </Drawer.Root>
{:else}
  <header id="header" class="z-50 gap-1 bg-background px-2 md:gap-2 md:px-3">
    <Button variant="ghost" class="md:hidden" href="/navigation">
      <Menu />
    </Button>
    <HeaderContents {hasValidJwt} />
  </header>
{/if}

<style>
  #header {
    display: flex;
    flex-grow: 1;
    justify-content: space-between;
    align-items: center;
    border-bottom: 1px solid;
    border-color: var(--divider);
    position: fixed;
    width: 100vw;
    height: var(--header-height);
  }

  /* Prevent draggable bar from rendering */
  :global(#drawer-content > :not(#my-content)) {
    display: none;
  }

  #my-content {
    width: 100%;
  }
</style>
