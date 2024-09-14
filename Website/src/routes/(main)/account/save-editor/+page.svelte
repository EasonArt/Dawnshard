<svelte:options immutable={true} />

<script lang="ts">
  import Page from '$lib/components/page.svelte';
  import Typography from '$lib/components/typography.svelte';
  import { Separator } from '$shadcn/components/ui/separator';

  import type { PageData } from './$types';
  import PresentWidget from './present/presentWidget.svelte';
  import Staged from './staged.svelte';

  export let data: PageData;
</script>

<Page title="GM工具">
  <div id="about" class="max-w-7xl">
    <Typography typography="h2">关于</Typography>
    <p>
      在此页面上，您可以发送各种道具到您的游戏邮箱

    </p>
    <p>
      该功能目前是基本的，将来可能会扩展。
    </p>
  </div>
  <br />
  <div class="flex flex-col gap-2 md:h-[67.5vh] md:flex-row">
    <div class="w-full">
      <PresentWidget widgetData={data.presentWidgetData} />
    </div>
    <Separator class="mx-3 hidden md:block" orientation="vertical" />
    <Separator class="visible my-2 px-4 md:hidden" orientation="horizontal" />
    <div class="h-full w-full overflow-hidden md:w-[35rem]">
      <Staged />
    </div>
  </div>
</Page>
