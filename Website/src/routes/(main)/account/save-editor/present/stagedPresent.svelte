<script lang="ts">
  import Gift from 'lucide-svelte/icons/gift';

  import { t } from '$lib/translations';
  import { formatTypeKey } from '$main/account/save-editor/present/util.ts';

  import type { PresentFormSubmission } from './presentTypes.ts';
  export let present: PresentFormSubmission;

  $: presentLabel = $t(`entity.${formatTypeKey(present.type)}.item.${present.item}`);
</script>

<li class="present">
  <Gift aria-hidden={true} />
  <p>添加 {present.quantity}x <strong>{presentLabel}</strong> 至您的游戏邮箱</p>
</li>

<style>
  .present {
    background: hsl(var(--background-surface));
    border-radius: 10px;
    padding: 1rem;
    display: flex;
    gap: 1rem;
    align-items: center;
  }
</style>
