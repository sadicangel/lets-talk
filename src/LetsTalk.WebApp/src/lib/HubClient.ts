import { HubConnectionBuilder, HubConnection, HubConnectionState, LogLevel, type IHttpConnectionOptions, HttpTransportType } from '@microsoft/signalr';
import type { ChannelMemberJoinedEvent, ChannelMemberLeftEvent, MessageEvent, NotificationEvent, UserConnectedEvent, UserDisconnectedEvent } from './events';

export type EventMap = {
    OnMessage: (event: MessageEvent) => void;
    OnNotification: (event: NotificationEvent) => void;
    OnUserConnected: (event: UserConnectedEvent) => void;
    OnUserDisconnected: (event: UserDisconnectedEvent) => void;
    OnChannelMemberJoined: (event: ChannelMemberJoinedEvent) => void;
    OnChannelMemberLeft: (event: ChannelMemberLeftEvent) => void;
}

export type InvokeMap = {
    SendMessage: (channelId: string, contentType: string, content: string) => void;
}

export class HubClient {
    private deffered: Map<keyof EventMap, Array<EventMap[keyof EventMap]>>;
    private options: IHttpConnectionOptions;
    private _connection?: HubConnection;
    public get connection(): HubConnection {
        if (!this._connection) {
            this._connection = new HubConnectionBuilder()
                .withUrl('/hub', this.options)
                .withAutomaticReconnect()
                .build();

            for (const [event, handlers] of this.deffered) {
                for (const handler of handlers) {
                    this._connection.on(event, handler);
                }
            }
            this.deffered.clear();
        }
        return this._connection;
    }

    constructor(options?: IHttpConnectionOptions) {
        this.deffered = new Map();
        this.options = Object.assign<IHttpConnectionOptions, IHttpConnectionOptions | undefined>({
            logger: LogLevel.Information,
            skipNegotiation: true,
            transport: HttpTransportType.WebSockets,
        },
            options);

    }

    async start() {
        if (this.connection.state === HubConnectionState.Disconnected) {
            await this.connection.start();
        }
    }

    async stop() {
        if (this.connection.state === HubConnectionState.Connected) {
            await this.connection.stop();
        }
    }

    on<T extends keyof EventMap>(event: T, handler: EventMap[T]): this {
        if (this._connection) {
            this.connection.on(event, handler);
        } else {
            let handlers = this.deffered.get(event);
            if (!handlers) {
                handlers = [];
                this.deffered.set(event, handlers);
            }
            handlers.push(handler);
        }
        return this;
    }

    off<T extends keyof EventMap>(event: T, handler: EventMap[T]): this {
        if (this._connection) {
            this.connection.off(event, handler);
        } else {
            const handlers = this.deffered.get(event);
            if (handlers) {
                const index = handlers.indexOf(handler);
                if (index !== -1) {
                    handlers.splice(index, 1);
                }
            }
        }
        return this;
    }

    async invoke<T extends keyof InvokeMap>(method: T, ...args: Parameters<InvokeMap[T]>): Promise<ReturnType<InvokeMap[T]>> {
        return await this.connection.invoke<ReturnType<InvokeMap[T]>>(method, ...args);
    }
}