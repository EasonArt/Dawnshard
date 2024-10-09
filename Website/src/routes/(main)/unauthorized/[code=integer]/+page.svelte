<script lang="ts">
  import { page } from '$app/stores';
</script>

<div class="center">
  <h1 class="mb-2 text-3xl">您游戏内还没有绑定关联账户</h1>
  <p class="text-lg">Error code: {$page.params.code}</p>
  {#if $page.params.code === '404'}
    <p class="mt-4">
      您游戏内还没有绑定账户哦，刚打开游戏会弹窗提示是否绑定账户，选择绑定，进行注册或登录已注册账户进行关联，绑定后亲再登录本网站即可正常使用。
      如果您游戏内绑定弹窗点了关闭，也不要紧，未绑定用户再进游戏会重复提醒绑定的，可能短时间内不会重复提醒，等1小时或数小时重进游戏会再次提醒您绑定账户的。
    </p>
  {/if}
</div>

<style>
  .center {
    text-align: center;
    margin-top: 5%;
  }
</style>
