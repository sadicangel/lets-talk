declare global {
  namespace App {
    interface PageData {
      profile?: import('$lib/types').UserProfile;
      accessToken?: string;
      chatApiUrl?: string;
    }
  }
}

export {};
