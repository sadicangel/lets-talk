type EncodeMap = {
    'text/plain': string;
}

const textEncoder = new TextEncoder();

// TODO: Replace atob and btoa with Buffer.from and Buffer.toString('base64').

export function encodeContent<T extends keyof EncodeMap>(contentType: T, content: string): EncodeMap[T] {
    switch (contentType) {
        case 'text/plain':
            return btoa(String.fromCharCode(...textEncoder.encode(content)));
        default:
            throw new Error(`Unsupported content type: ${contentType}`);
    }
}

export function decodeContent<T extends keyof EncodeMap>(contentType: T, content: EncodeMap[T]): string {
    switch (contentType) {
        case 'text/plain':
            return atob(content);
        default:
            throw new Error(`Unsupported content type: ${contentType}`);
    }
}