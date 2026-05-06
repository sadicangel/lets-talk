import { getChannels, joinChannel } from '$lib/server/api';
import { accessTokenCookie } from '$lib/server/cookies';
import { redirect } from '@sveltejs/kit';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ cookies, params }) => {
  const token = cookies.get(accessTokenCookie);
  if (!token) redirect(303, '/login');

  const response = await getChannels(token);
  const activeChannel = response.channels.find((channel) => channel.channelId === params.channelId);
  if (!activeChannel) redirect(303, '/channels');

  await joinChannel(token, params.channelId);
  const refreshed = await getChannels(token);

  return {
    channels: refreshed.channels,
    activeChannel: refreshed.channels.find((channel) => channel.channelId === params.channelId) ?? activeChannel
  };
};
