import { clearAccessTokenCookie } from '$lib/server/cookies';
import { redirect, type RequestHandler } from '@sveltejs/kit';

export const POST: RequestHandler = async ({ cookies }) => {
  clearAccessTokenCookie(cookies);
  redirect(303, '/login');
};

export const GET: RequestHandler = async ({ cookies }) => {
  clearAccessTokenCookie(cookies);
  redirect(303, '/login');
};
