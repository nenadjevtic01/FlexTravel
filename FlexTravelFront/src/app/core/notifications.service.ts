import { HttpClient } from '@angular/common/http';
import { effect, inject, Injectable, signal, untracked } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { AuthService } from './auth.service';
import { AppNotification, NotificationList } from './models';

@Injectable({ providedIn: 'root' })
export class NotificationsService {
  private readonly http = inject(HttpClient);
  private readonly auth = inject(AuthService);
  private connection: HubConnection | null = null;

  readonly items = signal<AppNotification[]>([]);
  readonly unreadCount = signal(0);
  readonly live = signal(false);

  readonly latest = signal<AppNotification | null>(null);

  constructor() {
    effect(() => {
      const userId = this.auth.userId();
      const enabled = this.auth.hasNotifications();
      untracked(() => (userId !== null && enabled ? this.start() : this.stop()));
    });
  }

  load(): void {
    this.http.get<NotificationList>('/api/notifications').subscribe({
      next: (list) => {
        this.items.set(list.items);
        this.unreadCount.set(list.unreadCount);
      },
      error: () => {},
    });
  }

  markAllRead(): void {
    if (this.unreadCount() === 0) return;

    this.http.post('/api/notifications/read', {}).subscribe(() => {
      this.unreadCount.set(0);
      this.items.update((items) => items.map((n) => ({ ...n, isRead: true })));
    });
  }

  private start(): void {
    this.stop();
    this.load();

    const connection = new HubConnectionBuilder()

      .withUrl('/hubs/notifications', {
        accessTokenFactory: async () => (await this.auth.validToken()) ?? '',
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build();

    connection.on('notification', (notification: AppNotification) => {
      this.items.update((items) =>
        [notification, ...items.filter((n) => n.id !== notification.id)].slice(0, 20),
      );
      this.unreadCount.update((count) => count + 1);
      this.latest.set(notification);
    });

    connection.onreconnecting(() => this.live.set(false));
    connection.onreconnected(() => {
      this.live.set(true);

      this.load();
    });
    connection.onclose(() => this.live.set(false));

    connection
      .start()
      .then(() => this.live.set(connection === this.connection))
      .catch(() => this.live.set(false));

    this.connection = connection;
  }

  private stop(): void {
    const connection = this.connection;
    this.connection = null;
    connection?.stop();
    this.items.set([]);
    this.unreadCount.set(0);
    this.latest.set(null);
    this.live.set(false);
  }
}
