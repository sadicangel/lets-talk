import { updateProfile } from '$lib/server/api';
import { accessTokenCookie, clearAccessTokenCookie } from '$lib/server/cookies';
import { fail, redirect } from '@sveltejs/kit';
import type { Actions } from './$types';

export const actions: Actions = {
  default: async ({ request, cookies }) => {
    const token = cookies.get(accessTokenCookie);
    if (!token) redirect(303, '/login');

    const form = await request.formData();
    const email = String(form.get('email') ?? '').trim();
    const avatarUrl = String(form.get('avatarUrl') ?? '').trim() || null;

    if (!email) {
      return fail(400, { message: 'Email is required.', email, avatarUrl });
    }

    try {
      return {
        profile: await updateProfile(token, { email, avatarUrl }),
        message: 'Profile saved.'
      };
    } catch {
      clearAccessTokenCookie(cookies);
      redirect(303, '/login');
    }
  }
};
