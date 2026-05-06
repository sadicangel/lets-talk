import { loginUser } from '$lib/server/api';
import { setAccessTokenCookie } from '$lib/server/cookies';
import { fail, redirect } from '@sveltejs/kit';
import type { Actions, PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ cookies }) => {
  if (cookies.get('lt_access_token')) redirect(303, '/channels');
};

export const actions: Actions = {
  default: async ({ request, cookies, url }) => {
    const form = await request.formData();
    const userName = String(form.get('userName') ?? '').trim();
    const password = String(form.get('password') ?? '');

    if (!userName || !password) {
      return fail(400, { message: 'Enter your username and password.', userName });
    }

    try {
      const token = await loginUser({ userName, password });
      setAccessTokenCookie(cookies, token.accessToken, token.expiresIn, url.protocol === 'https:');
    } catch {
      return fail(401, { message: 'The username or password was not accepted.', userName });
    }

    redirect(303, '/channels');
  }
};
