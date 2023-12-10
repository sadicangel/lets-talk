<script lang="ts">
    import { onDestroy, onMount } from 'svelte';
    import {
        HttpTransportType,
        HubConnection,
        HubConnectionBuilder,
        HubConnectionState
    } from '@microsoft/signalr';
    import type {
        ChannelCreated,
        ChannelDeleted,
        ChannelUpdated,
        MessageBroadcast,
        NotificationBroadcast,
        UserConnected,
        UserDisconnected,
        UserJoined,
        UserLeft
    } from '$lib/events';
    import type { UserProfile } from '$lib/responses';
    import { encodeText } from '$lib/utf8';
    import Message from '$lib/components/Message.svelte';

    let profile: UserProfile = {
        userId: '',
        userName: '',
        userAvatar: '',
        channels: []
    };

    let connection: HubConnection;
    let isConnnected = false;

    let messages: Record<string, MessageBroadcast[]> = {};

    onMount(async () => {
        const response = await fetch('/api/account/profile');
        if (response.ok) {
            profile = await response.json();
            connection = new HubConnectionBuilder()
                .withUrl('/api/letstalk', {
                    skipNegotiation: true,
                    transport: HttpTransportType.WebSockets
                })
                .build();

            connection.onreconnected(() => {
                isConnnected = true;
            });

            connection.onclose((error) => {
                if (error) console.error(error);
                isConnnected = false;
            });

            connection.on('OnUserConnected', (event: UserConnected) => {
                console.log('UserConnected', event);
            });

            connection.on('OnUserDisconnected', (event: UserDisconnected) => {
                console.log('UserDisconnected', event);
            });

            connection.on('OnChannelCreated', (event: ChannelCreated) => {
                console.log('ChannelCreated', event);
            });

            connection.on('OnChannelUpdated', (event: ChannelUpdated) => {
                console.log('ChannelUpdated', event);
            });

            connection.on('OnChannelDeleted', (event: ChannelDeleted) => {
                console.log('ChannelDeleted', event);
            });

            connection.on('OnUserJoined', (event: UserJoined) => {
                console.log('UserJoined', event);
            });

            connection.on('OnUserLeft', (event: UserLeft) => {
                console.log('UserLeft', event);
            });

            connection.on('OnMessageBroadcast', (event: MessageBroadcast) => {
                console.log('MessageBroadcast', event);
                const list = (messages[event.channelId] = messages[event.channelId] || []);
                list.push(event);
                messages = messages;
            });

            connection.on('OnNotificationBroadcast', (event: NotificationBroadcast) => {
                console.log('NotificationBroadcast', event);
            });

            await connectToHub();

            const list = (messages[profile.channels[0].channelId] =
                messages[profile.channels[0].channelId] || []);
            list.push({
                senderId: 'cb33be51-2342-488d-8882-54028d40d91c',
                senderAvatar:
                    'https://api.dicebear.com/7.x/fun-emoji/svg?seed=cb33be51-2342-488d-8882-54028d40d91c',
                senderName: 'other@lt.com',
                channelId: '6e5312d6-57e0-4362-925d-a3881c1e5df7',
                channelName: 'admins',
                channelIcon:
                    'https://api.dicebear.com/7.x/shapes/svg?seed=6e5312d6-57e0-4362-925d-a3881c1e5df7',
                content: encodeText('Odio harum omnis quo labore laborum. Sequi?'),
                contentType: 'text/plain',
                eventId: '',
                eventTimestamp: '',
                eventType: '',
                timestamp: new Date().toISOString()
            });
            messages = messages;

            await connection.send(
                'SendMessage',
                profile.channels[0].channelId,
                'text/plain',
                encodeText('Lorem ipsum dolor sit amet consectetur adipisicing elit.')
            );
        } else {
            console.error(await response.text());
        }
    });

    onDestroy(async () => await disconnectFromHub());

    async function connectToHub() {
        await connection.start();
        isConnnected = connection.state == HubConnectionState.Connected;
    }

    async function disconnectFromHub() {
        if (connection) {
            await connection.stop();
        }
    }

    let createChannelName = undefined as unknown as string;

    async function createChannel() {
        const response = await fetch('/api/channels', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ channelName: createChannelName })
        });
        console.log(await response.json());
    }

    let updateChannelId = undefined as unknown as string;
    let updateChannelName = undefined as unknown as string;
    let updateChannelIcon = undefined as unknown as string;

    async function updateChannel() {
        const response = await fetch(`/api/channels/${updateChannelId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ channelName: updateChannelName, channelIcon: updateChannelIcon })
        });
        console.log(await response.json());
    }

    let deleteChannelId = undefined as unknown as string;

    async function deleteChannel() {
        const response = await fetch(`/api/channels/${deleteChannelId}`, {
            method: 'DELETE'
        });
        if (!response.ok) console.error(await response.text());
    }

    let joinChannelId = undefined as unknown as string;

    async function joinChannel() {
        await connection.send('JoinChannel', joinChannelId);
    }

    let leaveChannelId = undefined as unknown as string;

    async function leaveChannel() {
        await connection.send('LeaveChannel', leaveChannelId);
    }

    const sendContentTypeList = ['text/plain', 'application/json'];
    let sendChannelId = undefined as unknown as string;
    let sendContentType = 'text/plain';
    let sendContent = undefined as unknown as string;

    async function sendMessage() {
        await connection.send(
            'SendMessage',
            sendChannelId,
            sendContentType,
            encodeText(sendContent)
        );
    }
</script>

<h1>Let's Talk</h1>
{#if profile?.userId}
    <div>{profile.userName}</div>
    <img src={profile.userAvatar} alt="avatar" width="128" />
{/if}
<pre>{JSON.stringify(profile, undefined, 2)}</pre>

{#if isConnnected}
    <div>
        <span>status: ✅</span>
        <button on:click={disconnectFromHub}>Disconnect from chat</button>
    </div>
    <div>
        <input type="text" placeholder="channel name" bind:value={createChannelName} />
        <button disabled={!createChannelName} on:click={createChannel}>Create Channel</button>
    </div>
    <div>
        <input type="text" placeholder="channel id" bind:value={updateChannelId} />
        <input type="text" placeholder="channel name" bind:value={updateChannelName} />
        <input type="text" placeholder="channel icon" bind:value={updateChannelIcon} />
        <button disabled={!updateChannelName} on:click={updateChannel}>Update Channel</button>
    </div>
    <div>
        <input type="text" placeholder="channel id" bind:value={deleteChannelId} />
        <button disabled={!deleteChannelId} on:click={deleteChannel}>Delete Channel</button>
    </div>
    <div>
        <input type="text" placeholder="channel id" bind:value={joinChannelId} />
        <button disabled={!joinChannelId} on:click={joinChannel}>Join Channel</button>
    </div>
    <div>
        <input type="text" placeholder="channel id" bind:value={leaveChannelId} />
        <button disabled={!leaveChannelId} on:click={leaveChannel}>Leave Channel</button>
    </div>
    <div>
        <input type="text" placeholder="channel id" bind:value={sendChannelId} />
        <select bind:value={sendContentType}>
            {#each sendContentTypeList as contentType}
                <option value={contentType}>
                    {contentType}
                </option>
            {/each}
        </select>
        <input type="text" placeholder="content" bind:value={sendContent} />
        <button disabled={!sendContent || !sendChannelId} on:click={sendMessage}
            >Send Message</button
        >
    </div>
    <div>
        {#each profile.channels as channel}
            <div class="text-xl">{channel.channelName}</div>
            {#if Array.isArray(messages[channel.channelId])}
                <ul>
                    {#each messages[channel.channelId] as message}
                        <li>
                            <Message {message} justifyStart={message.senderId !== profile.userId} />
                        </li>
                    {/each}
                </ul>
            {/if}
        {/each}
    </div>
{:else}
    <div>
        <span>status: ❌</span>
        <button on:click={connectToHub}>Connect to chat</button>
    </div>
{/if}
