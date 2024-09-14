<script lang="ts">
  import Ceris from '$lib/assets/acknowledgement/ceris.webp';
  import FatesTimelines1337 from '$lib/assets/acknowledgement/fatestimelines1337.webp';
  import Lati from '$lib/assets/acknowledgement/lati.webp';
  import LukeFZ from '$lib/assets/acknowledgement/lukefz.webp';
  import Nano from '$lib/assets/acknowledgement/nano.webp';
  import Nightmerp from '$lib/assets/acknowledgement/nightmerp.webp';
  import Skazord from '$lib/assets/acknowledgement/skazord.webp';
  import Sockperson from '$lib/assets/acknowledgement/sockperson.webp';
  import Typography from '$lib/components/typography.svelte';
  import * as Card from '$shadcn/components/ui/card/index';

  import Acknowledgement from './acknowledgement.svelte';
  import BuyMeACoffee from './icons/buyMeACoffee.svelte';
  import Discord from './icons/discord.svelte';
  import GitHub from './icons/github.svelte';
  import Patreon from './icons/patreon.svelte';
  import LinkButton from './linkButton.svelte';
</script>

<div id="banner">
  <Card.Root class="w-full max-w-[55rem]">
    <Card.Header>
      <Card.Title tag="h2" class="text-3xl">欢迎来到失落的龙约</Card.Title>
    </Card.Header>
    <Card.Content>
      <p class="text-lg font-semibold" style:margin-bottom="0.5rem">
        本服务是一个服务器模拟器项目，可以让失落的龙约能够继续游玩。
      </p>
      <p>
        从23年服务开放至今，修复了游戏很多问题，并将会持续更新，为龙约爱好者带来更好的服务和游玩体验。
      </p>
    </Card.Content>

  </Card.Root>
</div>

<div class="flex flex-col gap-5 p-5 md:w-[75%]">


  <div class="flex flex-col gap-3">

    <div>
      <Typography typography="h3">如何使用GM工具?</Typography>
      <p>
         请先在右上角点击登录，完成登录后本站左侧会多出账户一栏，在账户栏目下点击GM工具，进入GM工具页面，选择道具类别，
         道具名称，数量，加入到要发送列表，点击保存修改，即可完成发送，发送成功可在游戏邮箱中收到您刚选择的道具。
      </p>
    </div>
    <div>
      <Typography typography="h3">感觉还行</Typography>
      <p>目前已经实现的功能</p>
      <ul class="list-inside list-disc px-4">
        <li>任务系统，活动系统</li>
        <li>建筑、召唤、龙好感度</li>
        <li>132个限定池，主池10连7金</li>
        <li>IOS安卓PC三端数据互通</li>
      </ul>
    </div>
    <div>
      <Typography typography="h3">值得期待</Typography>
      <p>新功能还在路上</p>
      <ul class="list-inside list-disc px-4">
        <li>朋友和联盟</li>
        <li>多人同斗</li>
        <li>努力：有些是可用的，正在慢慢增加</li>
      </ul>
    </div>
  </div>
  <div>
    <Typography typography="h2">打赏名单</Typography>
    <p>
      特别感谢以下朋友，提供了帮助和支持
    </p>
    <ul class="ml-3 mt-1">
      <Acknowledgement name="a321344416" avatarSrc={Nightmerp}>
        感谢您的打赏，这是一种鼓舞，这是一种支持，这是一种动力，这是一种榜样！
      </Acknowledgement>
      <Acknowledgement name="tb356341176" avatarSrc={Sockperson}>
        真诚的买家，并提出了很多宝贵的建议。
      </Acknowledgement>
    </ul>
  </div>
</div>

<style>
  #banner {
    padding: 3rem;
    object-position: 100% 50%;
    background-size: cover;
    background-repeat: no-repeat;
    border-bottom: 1px solid;
    border-color: var(--divider);
  }

  @media (min-width: 1080px) {
    #banner {
      background-position: 10% 20%;
      background-image: url('/src/lib/assets/bannerLight-wide.webp');
    }

    :global(.dark #banner) {
      background-image: url('/src/lib/assets/bannerDark-wide.webp');
    }
  }

  @media (max-width: 1080px) {
    #banner {
      background-position: 10% 10%;
      background-image: url('/src/lib/assets/bannerLight-narrow.webp');
    }

    :global(.dark #banner) {
      background-image: url('/src/lib/assets/bannerDark-narrow.webp');
    }
  }

  li {
    margin: 5px 0;
  }
</style>
