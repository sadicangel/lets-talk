import type { Cookies } from '@sveltejs/kit';

export const accessTokenCookie = 'lt_access_token';

export function setAccessTokenCookie(cookies: Cookies, token: string, expiresIn: number, secure: boolean) {
  cookies.set(accessTokenCookie, token, {
    path: '/',
    httpOnly: false,
    sameSite: 'lax',
    secure,
    maxAge: expiresIn
  });
}

export function clearAccessTokenCookie(cookies: Cookies) {
  cookies.delete(accessTokenCookie, { path: '/' });
}
