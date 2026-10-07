import { Component, effect, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AppNotification } from '../core/models';
import { NotificationsService } from '../core/notifications.service';
import { notificationIcon } from './notification-icon';

@Component({
  selector: 'app-notification-toast',
  templateUrl: './notification-toast.html',
  styleUrl: './notification-toast.scss',
})
export class NotificationToast {
  private readonly notifications = inject(NotificationsService);
  private readonly router = inject(Router);
  private timer: ReturnType<typeof setTimeout> | undefined;

  protected readonly shown = signal<AppNotification | null>(null);
  protected readonly icon = notificationIcon;

  constructor() {
    effect(() => {
      const latest = this.notifications.latest();
      if (!latest) return;
      this.shown.set(latest);
      clearTimeout(this.timer);
      this.timer = setTimeout(() => this.shown.set(null), 7000);
    });
  }

  protected open(notification: AppNotification): void {
    this.shown.set(null);
    if (notification.link) {
      this.router.navigateByUrl(notification.link);
    }
  }
}
