import type { Handle } from '@sveltejs/kit';

const authRoutes = ['/login', '/register'];

export const handle: Handle = async ({ event, resolve }) => {
  if (authRoutes.includes(event.url.pathname) && event.cookies.get('lt_access_token')) {
    return resolve(event);
  }

  return resolve(event);
};
