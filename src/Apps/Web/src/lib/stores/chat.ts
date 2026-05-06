import { browser } from '$app/environment';
import type { ChatMessage, HubMessage, HubUser } from '$lib/types';
import { get } from 'svelte/store';
import { writable } from 'svelte/store';
import { appendMessage } from './messages';
import { accessToken, chatApiUrl } from './session';

const maxImageBytes = 2 * 1024 * 1024;
let connection: import('@microsoft/signalr').HubConnection | null = null;

export const connectionState = writable<'disconnected' | 'connecting' | 'connected' | 'reconnecting'>('disconnected');
export const sendError = writable<string | null>(null);

export async function connectChat() {
  if (!browser || connection) return;

  const token = get(accessToken);
  const apiUrl = get(chatApiUrl);
  if (!token) return;

  const signalR = await import('@microsoft/signalr');
  connectionState.set('connecting');

  connection = new signalR.HubConnectionBuilder()
    .withUrl(`${apiUrl}/hub`, { accessTokenFactory: () => get(accessToken) ?? '' })
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();

  connection.on('OnMessage', (message: HubMessage) => appendMessage(normalizeMessage(message)));
  connection.onreconnecting(() => connectionState.set('reconnecting'));
  connection.onreconnected(() => connectionState.set('connected'));
  connection.onclose(() => {
    connection = null;
    connectionState.set('disconnected');
  });

  await connection.start();
  connectionState.set('connected');
}

export async function disconnectChat() {
  if (!connection) return;
  const active = connection;
  connection = null;
  await active.stop();
  connectionState.set('disconnected');
}

export async function sendTextMessage(channelId: string, text: string) {
  const trimmed = text.trim();
  if (!trimmed) return;
  await sendBytes(channelId, 'text/plain;charset=utf-8', new TextEncoder().encode(trimmed));
}

export async function sendImageMessage(channelId: string, file: File) {
  if (!file.type.startsWith('image/')) {
    sendError.set('Choose an image file.');
    return;
  }

  if (file.size > maxImageBytes) {
    sendError.set('Images must be 2 MiB or smaller.');
    return;
  }

  await sendBytes(channelId, file.type, new Uint8Array(await file.arrayBuffer()));
}

async function sendBytes(channelId: string, contentType: string, bytes: Uint8Array) {
  sendError.set(null);
  await connectChat();

  if (!connection) {
    sendError.set('Chat is not connected.');
    return;
  }

  await connection.invoke('SendMessage', channelId, contentType, bytesToBase64(bytes));
}

function normalizeMessage(message: HubMessage): ChatMessage {
  const bytes = contentToBytes(message.content);
  const contentType = message.contentType;
  const isImage = contentType.startsWith('image/');
  const author = normalizeUser(message.author);

  return {
    id: message.eventId,
    channelId: message.channel.channelId,
    author,
    contentType,
    text: isImage ? null : new TextDecoder().decode(bytes),
    imageUrl: isImage ? `data:${contentType};base64,${bytesToBase64(bytes)}` : null,
    timestamp: message.timestamp
  };
}

function normalizeUser(user: HubUser): HubUser {
  return {
    userId: user.userId,
    userName: user.userName,
    email: user.email,
    avatarUrl: user.avatarUrl
  };
}

function contentToBytes(content: unknown): Uint8Array {
  if (typeof content === 'string') return base64ToBytes(content);
  if (content instanceof ArrayBuffer) return new Uint8Array(content);
  if (ArrayBuffer.isView(content)) return new Uint8Array(content.buffer);
  if (Array.isArray(content)) return new Uint8Array(content);
  return new Uint8Array();
}

function bytesToBase64(bytes: Uint8Array) {
  let binary = '';
  const chunkSize = 0x8000;
  for (let index = 0; index < bytes.length; index += chunkSize) {
    binary += String.fromCharCode(...bytes.subarray(index, index + chunkSize));
  }

  return btoa(binary);
}

function base64ToBytes(value: string) {
  const binary = atob(value);
  const bytes = new Uint8Array(binary.length);
  for (let index = 0; index < binary.length; index += 1) {
    bytes[index] = binary.charCodeAt(index);
  }

  return bytes;
}
