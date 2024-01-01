export const { encodeText, decodeText } = (function () {
    const encoder = new TextEncoder();
    return {
        encodeText: (content: string) => Buffer.from(encoder.encode(content)).toString('base64'),
        decodeText: (content: string) => Buffer.from(content, 'base64').toString('utf-8')
    };
})();
