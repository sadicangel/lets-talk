import { getProfile, getPublicChatApiUrl } from '$lib/server/api';
import { accessTokenCookie, clearAccessTokenCookie } from '$lib/server/cookies';
import { redirect } from '@sveltejs/kit';
import type { LayoutServerLoad } from './$types';

export const load: LayoutServerLoad = async ({ cookies }) => {
  const token = cookies.get(accessTokenCookie);
  if (!token) redirect(303, '/login');

  try {
    const profile = await getProfile(token);
    return {
      profile,
      accessToken: token,
      chatApiUrl: getPublicChatApiUrl()
    };
  } catch {
    clearAccessTokenCookie(cookies);
    redirect(303, '/login');
  }
};
