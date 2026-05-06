import { ApiRequestError, loginUser, registerUser } from '$lib/server/api';
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
    const email = String(form.get('email') ?? '').trim();
    const password = String(form.get('password') ?? '');
    const avatarUrl = String(form.get('avatarUrl') ?? '').trim() || null;

    if (!userName || !email || !password) {
      return fail(400, {
        message: 'Username, email, and password are required.',
        details: [],
        userName,
        email,
        avatarUrl
      });
    }

    try {
      await registerUser({ userName, email, password, avatarUrl });
      const token = await loginUser({ userName, password });
      setAccessTokenCookie(cookies, token.accessToken, token.expiresIn, url.protocol === 'https:');
    } catch (error) {
      if (error instanceof ApiRequestError) {
        return fail(error.status, {
          message: error.message,
          details: error.details,
          userName,
          email,
          avatarUrl
        });
      }

      return fail(400, {
        message: 'Could not create that account.',
        details: [describeError(error)],
        userName,
        email,
        avatarUrl
      });
    }

    redirect(303, '/channels');
  }
};

function describeError(error: unknown) {
  if (!(error instanceof Error)) return 'Unknown error.';

  const cause = error.cause;
  if (cause && typeof cause === 'object' && 'message' in cause) {
    return `${error.message}: ${String(cause.message)}`;
  }

  return error.message;
}
