<script lang="ts">
	import ChatMessage from '$lib/components/ChatMessage.svelte';
	import { decodeContent, encodeContent } from '$lib/encoding';
	import type { MessageEvent } from '$lib/events';
	import { HubClient } from '$lib/HubClient';
	import { mount, onMount } from 'svelte';

	let isConnected = $state(false);

	let username = $state('');

	const hub = new HubClient();
	hub
		.on('OnMessage', (event) => {
			mount(ChatMessage, {
				target: document.getElementById('chat')!,
				props: { message: event, alignment: 'start' }
			});
		})
		.on('OnNotification', (event) => {
			const notification = document.createElement('div');
			notification.textContent = decodeContent(event.contentType, event.content);
			notification.classList.add('text-blue-600');
			document.getElementById('chat')!.appendChild(notification);
		})
		.on('OnUserConnected', (event) => {
			const notification = document.createElement('div');
			notification.textContent = `${event.connectingUser.userName} has joined the chat.`;
			notification.classList.add('text-blue-600');
			document.getElementById('chat')!.appendChild(notification);
		})
		.on('OnUserDisconnected', (event) => {
			const notification = document.createElement('div');
			notification.textContent = `${event.disconnectingUser.userName} has left the chat.`;
			notification.classList.add('text-blue-600');
			document.getElementById('chat')!.appendChild(notification);
		})
		.on('OnChannelMemberJoined', (event) => {
			console.log(event);
		})
		.on('OnChannelMemberLeft', (event) => {
			console.log(event);
		});

	onMount(() => {
		fetch('/api/auth/users')
			.then((response) => {
				if (!response.ok) {
					throw new Error(response.statusText);
				}
				return response.json();
			})
			.then((users: string[]) => {
				username = users[0];
			});
	});

	// dummy message event
	const message: MessageEvent = {
		eventId: '',
		eventName: 'MessageEvent',
		timestamp: new Date().toISOString(),
		channel: {
			id: '019341b2-ff36-7798-ad0b-cb7ecc9fb128',
			displayName: 'General',
			adminId: ''
		},
		author: {
			id: '',
			userName: 'Obi-Wan_Kenobi'
		},
		contentType: 'text/plain',
		content: encodeContent('text/plain', 'You were the Chosen One!')
	};
</script>

<h1>Chat - {username}</h1>

{#if isConnected}
	<h1>Let's Talks Chat Room - Connected</h1>
	<div id="chat"></div>
	<form
		id="inputForm"
		onsubmit={async (event) => {
			event.preventDefault();
			const messageInput = document.getElementById('messageInput')! as HTMLInputElement;
			const channelId = '019341b2-ff36-7798-ad0b-cb7ecc9fb128';
			const content = encodeContent('text/plain', messageInput.value);
			hub
				.invoke('SendMessage', channelId, 'text/plain', content)
				.catch((err) => console.error(err.toString()));
			messageInput.value = '';
		}}
	>
		<input type="text" id="messageInput" placeholder="Message" required />
		<button type="submit">Send</button>
	</form>
{:else}
	<h1 class="text-red-500">Let's Talks Chat Room - Not connected</h1>
	<button
		disabled={!username}
		onclick={async () => {
			await fetch('/api/auth/login', {
				method: 'POST',
				headers: {
					'Content-Type': 'application/x-www-form-urlencoded'
				},
				body: new URLSearchParams({
					username: username,
					password: 'password'
				})
			})
				.then((loginResponse) => {
					if (!loginResponse.ok) {
						throw new Error(loginResponse.statusText);
					}
					return hub.start();
				})
				.then(() => {
					isConnected = true;
				})
				.catch((err) => {
					console.error(err.toString());
				});
		}}>Login</button
	>
{/if}
