<script lang="ts">
	import { HubClient } from '$lib/HubClient';

	let isConnected = $state(false);

	const hub = new HubClient();
	hub
		.on('OnMessage', (event) => {
			const msg = document.createElement('div');
			msg.textContent = `${event.author.userName}: ${atob(event.content)}`;
			document.getElementById('chat')!.appendChild(msg);
		})
		.on('OnNotification', (event) => {
			const notification = document.createElement('div');
			notification.textContent = atob(event.content);
			notification.classList.add('text-blue-600');
			document.getElementById('chat')!.appendChild(notification);
		})
		.on('OnUserConnected', (event) => {
			const notification = document.createElement('div');
			notification.textContent = `${event.connectingUser.userName} has joined the chat.`;
			notification.classList.add('notification');
			document.getElementById('chat')!.appendChild(notification);
		})
		.on('OnUserDisconnected', (event) => {
			const notification = document.createElement('div');
			notification.textContent = `${event.disconnectingUser.userName} has left the chat.`;
			notification.classList.add('notification');
			document.getElementById('chat')!.appendChild(notification);
		})
		.on('OnChannelMemberJoined', (event) => {
			console.log(event);
		})
		.on('OnChannelMemberLeft', (event) => {
			console.log(event);
		});
</script>

<h1>Chat</h1>

<button
	onclick={async () => {
		const usersResponse = await fetch('/api/auth/users');
		if (!usersResponse.ok) {
			console.error(usersResponse.statusText);
			return;
		}

		const users: string[] = await usersResponse.json();
		const loginResponse = await fetch('/api/auth/login', {
			method: 'POST',
			headers: {
				'Content-Type': 'application/x-www-form-urlencoded'
			},
			body: new URLSearchParams({
				username: users[0],
				password: 'password'
			})
		});
		if (!loginResponse.ok) {
			console.error(loginResponse.statusText);
			return;
		}
	}}>Login</button
>

<button
	onclick={async () => {
		await hub.start();
		isConnected = true;
	}}>Click</button
>

{#if isConnected}
	<h1>Let's Talks Chat Room - Connected</h1>
	<div id="chat"></div>
	<form
		id="inputForm"
		onsubmit={async (event) => {
			event.preventDefault();
			const messageInput = document.getElementById('messageInput')! as HTMLInputElement;
			const channelId = '019341b2-ff36-7798-ad0b-cb7ecc9fb128';
			const content = messageInput.value;
			const utf8Array = new TextEncoder().encode(content);
			const base64String = btoa(String.fromCharCode(...utf8Array));
			hub
				.invoke('SendMessage', channelId, 'text/plain', base64String)
				.catch((err) => console.error(err.toString()));
			messageInput.value = '';
		}}
	>
		<input type="text" id="messageInput" placeholder="Message" required />
		<button type="submit">Send</button>
	</form>
{:else}
	<h1 class="text-red-500">Let's Talks Chat Room - Not connected</h1>
{/if}
