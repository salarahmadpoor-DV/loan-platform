const SESSION_KEY = "matchi.accessToken";

export function readAccessToken(): string | null {
  try {
    const fromLocal = localStorage.getItem(SESSION_KEY);
    if (fromLocal) {
      return fromLocal;
    }
    const fromSession = sessionStorage.getItem(SESSION_KEY);
    if (fromSession) {
      localStorage.setItem(SESSION_KEY, fromSession);
      sessionStorage.removeItem(SESSION_KEY);
      return fromSession;
    }
    return null;
  } catch {
    return null;
  }
}

export function writeAccessToken(token: string | null): void {
  try {
    if (token) {
      localStorage.setItem(SESSION_KEY, token);
    } else {
      localStorage.removeItem(SESSION_KEY);
    }
  } catch {
    // localStorage may be unavailable; in-memory store still holds the token.
  }
}
