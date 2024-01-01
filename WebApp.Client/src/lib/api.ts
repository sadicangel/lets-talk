interface IRequest<T> {
    send(fetcher?: typeof fetch): Promise<T>;
}

export interface ProblemDetails {
    type?: string | undefined;
    title?: string | undefined;
    status?: number | undefined;
    detail?: string | undefined;
    instance?: string | undefined;
    extensions?: { [key: string]: Object; } | undefined;
}

const account = {
    register: (email: string, userName: string, password: string): IRequest<void> => {
        return {
            send: (fetcher = fetch) => unwrapOrThrow(fetcher('/api/account/register', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ email, userName, password })
            }))
        };
    },
    login: (email: string, password: string, rememberMe: boolean): IRequest<void> => {
        return {
            send: (fetcher = fetch) =>
                unwrapOrThrow(fetcher('/api/account/login', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ email, password, rememberMe })
                }))
        };
    },
    logout: (): IRequest<void> => {
        return { send: (fetcher = fetch) => unwrapOrCatch(fetcher('/api/account/logout')) };
    },
    profile: (): IRequest<UserProfileResponse | undefined> => {
        return { send: (fetcher = fetch) => unwrapOrCatch(fetcher('/api/account/profile')) };
    },
    channels: (): IRequest<UserChannelListResponse> => {
        return { send: (fetcher = fetch) => unwrapOrCatch(fetcher('/api/account/channels'), { channels: [] }) };
    }
}

export interface UserProfileResponse {
    userId: string;
    userName: string;
    userAvatar: string;
}

export interface UserChannelListResponse {
    channels: UserChannelListChannel[]
}

interface UserChannelListChannel {
    channelId: string;
    channelName: string;
    channelIcon: string;
}

const channel = {
    getList: (after?: string, count?: number): IRequest<ChannelListResponse> => {
        const uri = new URL('api/channels');
        if (after) {
            uri.searchParams.append('after', after);
        }
        if (count) {
            uri.searchParams.append('count', count.toString());
        }
        return { send: (fetcher = fetch) => unwrapOrThrow(fetcher(uri)) };
    },
    getById: (channelId: string): IRequest<ChannelListChannel> => {
        return { send: (fetcher = fetch) => unwrapOrThrow(fetcher(`/api/channels/${channelId}`)) };
    },
    create: (channelName: string, channelIcon?: string): IRequest<ChannelListChannel> => {
        return {
            send: (fetcher = fetch) => unwrapOrThrow(fetcher('/api/channels', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    channelName,
                    channelIcon
                })
            }))
        };
    },
    update: (channelId: string, channelName: string, channelIcon: string): IRequest<void> => {
        return {
            send: (fetcher = fetch) => unwrapOrThrow(fetcher(`/api/channels/${channelId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    channelName,
                    channelIcon
                })
            }))
        };
    },
    delete: (channelId: string): IRequest<void> => {
        return { send: (fetcher = fetch) => unwrapOrThrow(fetcher(`/api/channels/${channelId}`, { method: 'DELETE' })) };
    }
};

interface ChannelListResponse {
    channels: []
    after?: string;
}

interface ChannelListChannel {
    channelId: string;
    channelName: string;
    channelIcon: string;
}

async function unwrapOrThrow<T>(promise: Promise<Response>): Promise<T> {
    const response = await promise;
    if (response.ok) {
        if (response.headers.get('Content-Type')?.includes('application/json')) {
            return response.json();
        } else {
            return response.text() as any;
        }
    } else {
        let problemDetails = response;
        try {

        }
        catch {
            problemDetails = response;
        }
        throw response;
    }
}
async function unwrapOrCatch<T>(promise: Promise<Response>): Promise<T | undefined>
async function unwrapOrCatch<T>(promise: Promise<Response>, defaultValue: T): Promise<T>
async function unwrapOrCatch<T>(promise: Promise<Response>, defaultValue?: T): Promise<T | undefined> {
    try {
        return await unwrapOrThrow(promise);
    }
    catch (e) {
        console.error(await (e as Response).text());
        return defaultValue;
    }
}

const api = {
    account,
    channel
};

type Api = typeof api;

export default api as Api;