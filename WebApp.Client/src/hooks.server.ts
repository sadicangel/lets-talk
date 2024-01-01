export async function handle({ event, resolve }) {
    event.locals.user = {
        isAuthenticated: !!event.cookies.get('.AspNetCore.Identity.Application')
    };
    return await resolve(event, {
        filterSerializedResponseHeaders: (name) => name.startsWith('content-type')
    });
}