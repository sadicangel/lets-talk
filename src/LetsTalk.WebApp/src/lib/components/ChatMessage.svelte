<script lang="ts">
	import { decodeContent } from '$lib/encoding';
	import type { MessageEvent } from '$lib/events';

	let { message, alignment = 'start' }: { message: MessageEvent; alignment: 'start' | 'end' } =
		$props();

	function formatTimestamp(timestamp: string): string {
		return new Date(message.timestamp).toLocaleString();
	}

	function formatTime(timestamp: string): string {
		return new Date(timestamp).toLocaleTimeString([], {
			hour: '2-digit',
			minute: '2-digit'
		});
	}

	const defaultAvatarUrl = `https://ui-avatars.com/api/?name=${message.author.userName.replace(/[\s\.\_]/g, '+')}&background=random&rounded=true&size=256`;
</script>

<div class="chat chat-{alignment}">
	<div class="chat-image avatar">
		<div class="w-10 rounded-full">
			<img
				alt="{message.author.userName} Avatar"
				src={message.author.avatarUrl ?? defaultAvatarUrl}
			/>
		</div>
	</div>
	<div class="chat-header">
		{message.author.userName}
		<div class="tooltip" data-tip={formatTimestamp(message.timestamp)}>
			<time class="text-xs opacity-50">{formatTime(message.timestamp)}</time>
		</div>
	</div>
	<div class="chat-bubble">{decodeContent(message.contentType, message.content)}</div>
	<!-- <div class="chat-footer opacity-50">Delivered</div> -->
</div>
