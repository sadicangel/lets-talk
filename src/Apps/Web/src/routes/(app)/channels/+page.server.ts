import { getChannels } from '$lib/server/api';
import { accessTokenCookie } from '$lib/server/cookies';
import { redirect } from '@sveltejs/kit';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ cookies }) => {
  const token = cookies.get(accessTokenCookie);
  if (!token) redirect(303, '/login');

  const response = await getChannels(token);
  const firstChannel = response.channels[0];
  if (firstChannel) redirect(303, `/channels/${firstChannel.channelId}`);

  return { channels: [] };
};
