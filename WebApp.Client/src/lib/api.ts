export const channel = {
    getList: (after?: string, count?: number): Promise<ChannelListResponse> => {
        const uri = new URL('api/channels');
        if (after) {
            uri.searchParams.append('after', after);
        }
        if (count) {
            uri.searchParams.append('count', count.toString());
        }
        return unwrap(fetch(uri));
    },
    getById: (channelId: string): Promise<ChannelListChannel> => {
        return unwrap(fetch(`/api/channels/${channelId}`))
    },
    create: (channelName: string, channelIcon?: string): Promise<ChannelListChannel> => {
        return unwrap(fetch('/api/channels', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                channelName,
                channelIcon
            })
        }));
    },
    update: (channelId: string, channelName: string, channelIcon: string): Promise<void> => {
        return unwrap(fetch(`/api/channels/${channelId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                channelName,
                channelIcon
            })
        }));
    },
    delete: (channelId: string) => {
        return unwrap(fetch(`/api/channels/${channelId}`, { method: 'DELETE' }));
    }
};

export interface ChannelListResponse {
    channels: []
    after?: string;
}

export interface ChannelListChannel {
    channelId: string;
    channelName: string;
    channelIcon: string;
}

async function unwrap<T>(promise: Promise<Response>): Promise<T> {
    const response = await promise;
    if (response.ok) {
        if (response.headers.get('Content-Type')?.includes('application/json')) {
            return response.json();
        } else {
            return response.text() as any;
        }
    } else {
        throw response;
    }
}

export default {
    channel
};