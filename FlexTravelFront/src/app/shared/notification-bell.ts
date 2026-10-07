import { Component, ElementRef, HostListener, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { timeAgo } from '../core/format';
import { AppNotification } from '../core/models';
import { NotificationsService } from '../core/notifications.service';
import { notificationIcon } from './notification-icon';

@Component({
  selector: 'app-notification-bell',
  templateUrl: './notification-bell.html',
  styleUrl: './notification-bell.scss',
})
export class NotificationBell {
  protected readonly notifications = inject(NotificationsService);
  private readonly router = inject(Router);
  private readonly host = inject(ElementRef<HTMLElement>);

  protected readonly open = signal(false);
  protected readonly ago = timeAgo;
  protected readonly icon = notificationIcon;

  protected toggle(event: Event): void {
    event.stopPropagation();
    const opening = !this.open();
    this.open.set(opening);
    if (opening) {
      this.notifications.markAllRead();
    }
  }

  protected openNotification(notification: AppNotification): void {
    this.open.set(false);
    if (notification.link) {
      this.router.navigateByUrl(notification.link);
    }
  }

  @HostListener('document:click', ['$event'])
  protected closeOnOutsideClick(event: Event): void {
    if (this.open() && !this.host.nativeElement.contains(event.target as Node)) {
      this.open.set(false);
    }
  }

  @HostListener('document:keydown.escape')
  protected closeOnEscape(): void {
    this.open.set(false);
  }
}
