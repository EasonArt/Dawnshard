<script lang="ts">
  import NotebookPen from 'lucide-svelte/icons/notebook-pen';
  import { toast } from 'svelte-sonner';

  import type { SaveChangesRequest } from '$main/account/save-editor/present/presentTypes';
  import { Button } from '$shadcn/components/ui/button';
  import * as Card from '$shadcn/components/ui/card';

  import StagedPresent from './present/stagedPresent.svelte';
  import { changesCount, presents } from './stores';

  $: anyModifications = $changesCount > 0;

  let loading = false;

  const onClickReset = () => {
    presents.set([]);
  };

  const onClickSave = () => {
    if (!anyModifications) return;
    const requestBody = {
      presents: $presents
    };

    saveChanges(requestBody);
  };

  const saveChanges = async (requestBody: SaveChangesRequest) => {
    loading = true;

    const request = new Request('/api/savefile/edit', {
      method: 'POST',
      body: JSON.stringify(requestBody),
      headers: {
        'Content-Type': 'application/json'
      }
    });

    const response = await fetch(request);

    loading = false;

    if (response.ok) {
      toast.success('发送物品成功');
      onClickReset();
    } else {
      toast.error('发送失败，可能您没有权限，限定版可用，如需升级请联系客服！');
      // eslint-disable-next-line no-console
      console.error('Savefile edit request failed with status', response.status);
    }
  };
</script>

<Card.Root class="h-full w-full overflow-hidden">
  <Card.Header>
    <Card.Title>
      <div class="flex flex-row items-center justify-items-start gap-2">
        <NotebookPen aria-hidden={true} size={25} />
        <h2 id="staged-changes-title" class="m-0 text-xl font-bold">发送清单</h2>
        {#if $changesCount > 90}
          <div class="flex-grow" />
          <p class="text-sm font-normal text-muted-foreground">{$changesCount} / 100</p>
        {/if}
      </div>
    </Card.Title>
  </Card.Header>
  <Card.Content class={anyModifications ? 'h-full' : 'pb-0'}>
    <div class="flex gap-3">
      <Button disabled={!anyModifications} variant="outline" on:click={onClickReset}>重置</Button>
      <Button disabled={!anyModifications || $changesCount > 100} {loading} on:click={onClickSave}>
        发送物品
      </Button>
    </div>
    <br />
    <ul class="flex h-[75%] flex-col gap-2 overflow-y-auto" aria-labelledby="staged-changes-title">
      {#if anyModifications}
        {#each $presents as present}
          <StagedPresent {present} />
        {/each}
      {/if}
    </ul>
  </Card.Content>
</Card.Root>
