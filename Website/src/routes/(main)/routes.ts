import { type Icon } from 'lucide-svelte';
import ChartBarIncreasing from 'lucide-svelte/icons/chart-bar-increasing';
import House from 'lucide-svelte/icons/house';
import Newspaper from 'lucide-svelte/icons/newspaper';
import Pencil from 'lucide-svelte/icons/pencil';
import User from 'lucide-svelte/icons/user';
import type { ComponentType } from 'svelte';

export type RouteGroup = {
  title: string;
  routes: Route[];
  requireAuth?: boolean;
};

export type Route = {
  title: string;
  href: string;
  icon: ComponentType<Icon>;
};

export const routeGroups: RouteGroup[] = [
  {
    title: '咨询',
    routes: [
      { title: '主页', href: '/', icon: House },
      { title: '新闻', href: '/news/1', icon: Newspaper }
    ]
  },

  {
    title: '事件',
    routes: [
      {
        title: '速通排行',
        href: '/events/time-attack/rankings/227010104',
        icon: ChartBarIncreasing
      }
    ]
  },
  {
    title: '账户',
    requireAuth: true,
    routes: [
      { title: '简介', href: '/account/profile', icon: User },
      {
        title: 'GM工具',
        href: '/account/save-editor',
        icon: Pencil
      }
    ]
  }
];
