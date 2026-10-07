import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { map, Observable } from 'rxjs';
import { UseCaseIds } from './models';

export interface CurrentUser {
  id: number;
  identity: string;
  email: string;
  avatar: string | null;
  useCaseIds: number[];
  expiresAt: number;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  phone: string;
  address: string;
  city: string;
}

interface TokenPair {
  token: string;
  refreshToken: string;
}

const TOKEN_KEY = 'flex-travel.token';
const REFRESH_KEY = 'flex-travel.refresh';

const REFRESH_EARLY_MS = 60_000;

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);

  readonly token = signal<string | null>(null);
  private refreshToken: string | null = null;
  private remember = false;
  private refreshing: Promise<string | null> | null = null;
  private timer: ReturnType<typeof setTimeout> | null = null;

  readonly user = computed(() => decodeToken(this.token()));
  readonly userId = computed(() => this.user()?.id ?? null);
  readonly isLoggedIn = computed(() => this.user() !== null);

  readonly isAdmin = computed(() => this.can(UseCaseIds.GetAdminStats));
  readonly canSeeAuditLog = computed(() => this.can(UseCaseIds.GetAuditLogs));
  readonly canBook = computed(() => this.can(UseCaseIds.CreateReservation));
  readonly isAgency = computed(() => this.can(UseCaseIds.GetAgencyTrips));
  readonly hasNotifications = computed(() => this.can(UseCaseIds.GetMyNotifications));
  readonly canFavorite = computed(() => this.can(UseCaseIds.SetFavorite));
  readonly canReview = computed(() => this.can(UseCaseIds.CreateReview));
  readonly canUseWaitlist = computed(() => this.can(UseCaseIds.JoinWaitlist));
  readonly canModerate = computed(() => this.can(UseCaseIds.ModerateReview));
  readonly canManageCatalog = computed(() => this.can(UseCaseIds.SaveDestination));
  readonly hasProfile = computed(() => this.can(UseCaseIds.GetMyProfile));
  readonly canSetPriceAlerts = computed(() => this.can(UseCaseIds.SetPriceAlert));
  readonly canChangeAvatar = computed(() => this.can(UseCaseIds.ChangeMyAvatar));
  readonly canRemoveAvatars = computed(() => this.can(UseCaseIds.RemoveUserAvatar));
  readonly canEditUsers = computed(() => this.can(UseCaseIds.UpdateUser));

  readonly homeUrl = computed(() =>
    this.isAgency() ? '/agency' : this.isAdmin() ? '/admin' : '/trips',
  );

  constructor() {
    const stored = readStored();
    if (stored) {
      this.useSession(stored.token, stored.refreshToken, stored.remember, false);
    }

    window.addEventListener('storage', (event) => {
      if (event.key !== TOKEN_KEY && event.key !== REFRESH_KEY) return;
      const next = readStored();
      if (!next) {
        this.clear(false);
      } else if (next.token !== this.token()) {
        this.useSession(next.token, next.refreshToken, next.remember, false);
      }
    });

    document.addEventListener('visibilitychange', () => {
      if (document.visibilityState === 'visible' && this.refreshToken && this.expiresSoon()) {
        void this.refresh();
      }
    });
  }

  login(email: string, password: string, rememberMe = false): Observable<void> {
    return this.http
      .post<TokenPair>('/api/token', { email, password, rememberMe })
      .pipe(map((pair) => this.useSession(pair.token, pair.refreshToken, rememberMe, true)));
  }

  register(account: RegisterRequest): Observable<void> {
    return this.http
      .post<TokenPair>('/api/register', account)
      .pipe(map((pair) => this.useSession(pair.token, pair.refreshToken, false, true)));
  }

  replaceToken(token: string): void {
    this.useSession(token, this.refreshToken, this.remember, true);
  }

  currentRefreshToken(): string | null {
    return this.refreshToken;
  }

  logout(): void {
    const refreshToken = this.refreshToken;
    if (refreshToken) {
      fetch('/api/token/revoke', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ refreshToken }),
      }).catch(() => undefined);
    }
    this.clear(true);
  }

  validToken(): Promise<string | null> {
    const token = this.token();
    if (!token) return Promise.resolve(null);
    if (!this.expiresSoon() || !this.refreshToken) return Promise.resolve(token);
    return this.refresh();
  }

  refresh(): Promise<string | null> {
    this.refreshing ??= this.refreshShared().finally(() => (this.refreshing = null));
    return this.refreshing;
  }

  private async refreshShared(): Promise<string | null> {
    const run = async (): Promise<string | null> => {
      const stored = readStored();
      if (stored && stored.refreshToken !== this.refreshToken && !isExpiringSoon(stored.token)) {
        this.useSession(stored.token, stored.refreshToken, stored.remember, false);
        return stored.token;
      }
      return this.callRefresh();
    };

    const locks = (navigator as Navigator & { locks?: LockManager }).locks;
    return locks ? locks.request('flex-travel.refresh', run) : run();
  }

  private async callRefresh(): Promise<string | null> {
    const refreshToken = this.refreshToken;
    if (!refreshToken) return null;

    try {
      const response = await fetch('/api/token/refresh', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ refreshToken }),
      });

      if (response.status === 401) {
        this.clear(true);
        return null;
      }

      if (!response.ok) return this.token();

      const pair = (await response.json()) as TokenPair;
      this.useSession(pair.token, pair.refreshToken, this.remember, true);
      return pair.token;
    } catch {
      return this.token();
    }
  }

  private can(useCaseId: number): boolean {
    return this.user()?.useCaseIds.includes(useCaseId) ?? false;
  }

  private expiresSoon(): boolean {
    const token = this.token();
    return token ? isExpiringSoon(token) : false;
  }

  private useSession(
    token: string,
    refreshToken: string | null,
    remember: boolean,
    store: boolean,
  ): void {
    this.refreshToken = refreshToken;
    this.remember = remember;
    this.token.set(token);

    if (store) {
      write(remember ? localStorage : sessionStorage, token, refreshToken);

      clearStorage(remember ? sessionStorage : localStorage);
    }

    this.schedule();
  }

  private clear(store: boolean): void {
    this.refreshToken = null;
    this.token.set(null);
    if (this.timer) clearTimeout(this.timer);
    if (store) {
      clearStorage(localStorage);
      clearStorage(sessionStorage);
    }
  }

  private schedule(): void {
    if (this.timer) clearTimeout(this.timer);
    const user = this.user();
    if (!user) return;

    if (!this.refreshToken) {
      this.timer = setTimeout(() => this.clear(true), Math.max(0, user.expiresAt - Date.now()));
      return;
    }

    const delay = Math.max(0, user.expiresAt - REFRESH_EARLY_MS - Date.now());
    this.timer = setTimeout(() => void this.refresh(), delay);
  }
}

function isExpiringSoon(token: string): boolean {
  const user = decodeToken(token);
  return !user || user.expiresAt - REFRESH_EARLY_MS <= Date.now();
}

function write(storage: Storage, token: string, refreshToken: string | null): void {
  try {
    storage.setItem(TOKEN_KEY, token);
    if (refreshToken) {
      storage.setItem(REFRESH_KEY, refreshToken);
    } else {
      storage.removeItem(REFRESH_KEY);
    }
  } catch {}
}

function clearStorage(storage: Storage): void {
  try {
    storage.removeItem(TOKEN_KEY);
    storage.removeItem(REFRESH_KEY);
  } catch {}
}

function readStored(): { token: string; refreshToken: string | null; remember: boolean } | null {
  for (const [storage, remember] of [
    [localStorage, true],
    [sessionStorage, false],
  ] as const) {
    try {
      const token = storage.getItem(TOKEN_KEY);
      const refreshToken = storage.getItem(REFRESH_KEY);
      const user = decodeToken(token);
      if (token && user && (refreshToken || user.expiresAt > Date.now())) {
        return { token, refreshToken, remember };
      }
    } catch {}
  }
  return null;
}

function decodeToken(token: string | null): CurrentUser | null {
  if (!token) {
    return null;
  }

  try {
    const payload = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
    const json = decodeURIComponent(
      atob(payload)
        .split('')
        .map((c) => '%' + c.charCodeAt(0).toString(16).padStart(2, '0'))
        .join(''),
    );
    const claims = JSON.parse(json);
    const expiresAt = Number(claims.exp) * 1000;

    if (!expiresAt) {
      return null;
    }

    return {
      id: Number(claims.UserId),
      identity: claims.Identity ?? claims.Email,
      email: claims.Email,
      avatar: claims.Avatar ?? null,
      useCaseIds: JSON.parse(claims.UseCases ?? '[]'),
      expiresAt,
    };
  } catch {
    return null;
  }
}
