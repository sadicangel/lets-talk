import { env } from '$env/dynamic/private';
import type { AccessTokenResponse, ChannelListResponse, Channel, UserProfile } from '$lib/types';
import { error } from '@sveltejs/kit';
import { existsSync, readFileSync } from 'node:fs';
import { Agent, fetch as undiciFetch, type Dispatcher } from 'undici';

const identityApiUrl = trimTrailingSlash(env.IDENTITY_API_URL ?? 'https://localhost:7116');
const chatApiUrl = trimTrailingSlash(env.CHAT_API_URL ?? 'https://localhost:7178');
const httpsDispatcher = createHttpsDispatcher();

export class ApiRequestError extends Error {
  constructor(
    public readonly status: number,
    message: string,
    public readonly details: string[] = []
  ) {
    super(message);
  }
}

function trimTrailingSlash(value: string) {
  return value.replace(/\/+$/, '');
}

function createHttpsDispatcher(): Dispatcher | undefined {
  const certPath = env.NODE_EXTRA_CA_CERTS;
  if (!certPath) return undefined;
  if (!existsSync(certPath)) {
    console.warn(`NODE_EXTRA_CA_CERTS points to a missing file: ${certPath}`);
    return undefined;
  }

  return new Agent({
    connect: {
      ca: readFileSync(certPath, 'utf8')
    }
  });
}

async function apiFetch(url: string, init: RequestInit = {}) {
  if (url.startsWith('https://') && httpsDispatcher) {
    return (await undiciFetch(
      url,
      { ...(init as object), dispatcher: httpsDispatcher } as Parameters<typeof undiciFetch>[1]
    )) as unknown as Response;
  }

  return fetch(url, init);
}

async function readError(response: Response, fallback: string) {
  const text = await response.text().catch(() => '');
  if (!text) return new ApiRequestError(response.status, fallback);

  try {
    const json = JSON.parse(text) as {
      title?: string;
      detail?: string;
      errors?: Record<string, string[]>;
    };
    const details = Object.values(json.errors ?? {}).flat();
    return new ApiRequestError(response.status, json.detail ?? json.title ?? fallback, details);
  } catch {
    return new ApiRequestError(response.status, text, [text]);
  }
}

async function readJson<T>(response: Response): Promise<T> {
  if (!response.ok) {
    const apiError = await readError(response, response.statusText);
    throw error(apiError.status, apiError.message);
  }

  return (await response.json()) as T;
}

function authHeaders(token: string) {
  return { Authorization: `Bearer ${token}` };
}

export async function registerUser(request: {
  userName: string;
  email: string;
  password: string;
  avatarUrl: string | null;
}) {
  console.info(`Registering user through IdentityService at ${identityApiUrl}/api/register`);
  const response = await apiFetch(`${identityApiUrl}/api/register`, {
    method: 'POST',
    headers: { 'content-type': 'application/json' },
    body: JSON.stringify(request)
  });

  if (!response.ok) {
    throw await readError(response, 'Registration failed.');
  }
}

export async function loginUser(request: { userName: string; password: string }) {
  const response = await apiFetch(`${identityApiUrl}/api/login`, {
    method: 'POST',
    headers: { 'content-type': 'application/json' },
    body: JSON.stringify(request)
  });

  return readJson<AccessTokenResponse>(response);
}

export async function getProfile(token: string) {
  const response = await apiFetch(`${identityApiUrl}/api/profile`, {
    headers: authHeaders(token)
  });

  return readJson<UserProfile>(response);
}

export async function updateProfile(token: string, request: { email: string; avatarUrl: string | null }) {
  const response = await apiFetch(`${identityApiUrl}/api/profile`, {
    method: 'PUT',
    headers: { ...authHeaders(token), 'content-type': 'application/json' },
    body: JSON.stringify(request)
  });

  return readJson<UserProfile>(response);
}

export async function getChannels(token: string) {
  const response = await apiFetch(`${chatApiUrl}/api/channels`, {
    headers: authHeaders(token)
  });

  return readJson<ChannelListResponse>(response);
}

export async function getChannel(token: string, channelId: string) {
  const response = await apiFetch(`${chatApiUrl}/api/channels/${channelId}`, {
    headers: authHeaders(token)
  });

  return readJson<Channel>(response);
}

export async function joinChannel(token: string, channelId: string) {
  const response = await apiFetch(`${chatApiUrl}/api/channels/${channelId}/join`, {
    method: 'PATCH',
    headers: authHeaders(token)
  });

  if (!response.ok) {
    const body = await response.text().catch(() => '');
    throw error(response.status, body || 'Could not join channel.');
  }
}

export function getPublicChatApiUrl() {
  return trimTrailingSlash(env.PUBLIC_CHAT_API_URL ?? env.CHAT_API_URL ?? 'https://localhost:7178');
}
