import { HttpClient, httpResource } from '@angular/common/http';
import { Component, effect, inject, input, signal, untracked } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import {
  errorMessage,
  formatDate,
  formatDateRange,
  formatMoney,
  imageUrl,
  formatTravelers,
} from '../../core/format';
import { PagedResponse, Reservation, WaitlistEntry } from '../../core/models';
import { NotificationsService } from '../../core/notifications.service';
import { PaymentBadge } from '../../shared/booking-bits';
import { Pagination } from '../../shared/pagination';
import { Stars } from '../../shared/stars';
import { StatusBadge } from '../../shared/status-badge';

@Component({
  selector: 'app-my-trips',
  imports: [RouterLink, Pagination, StatusBadge, Stars, PaymentBadge],
  templateUrl: './my-trips.html',
  styleUrl: './my-trips.scss',
})
export class MyTrips {
  private readonly http = inject(HttpClient);
  protected readonly auth = inject(AuthService);

  readonly booked = input<string>();

  protected readonly statuses = ['', 'Pending', 'Confirmed', 'Cancelled', 'Rejected', 'Expired'];
  protected readonly status = signal('');
  protected readonly page = signal(1);

  protected readonly reservations = httpResource<PagedResponse<Reservation>>(() => {
    if (!this.auth.canBook()) return undefined;
    const params: Record<string, string | number> = { page: this.page(), perPage: 10 };
    if (this.status()) params['status'] = this.status();
    return { url: '/api/reservations/mine', params };
  });

  protected readonly waitlist = httpResource<WaitlistEntry[]>(() =>
    this.auth.canUseWaitlist() ? '/api/waitlist/mine' : undefined,
  );

  protected readonly busyId = signal<number | null>(null);
  protected readonly actionError = signal<string | null>(null);

  constructor() {
    const notifications = inject(NotificationsService);
    effect(() => {
      if (!notifications.latest()) return;
      untracked(() => {
        this.reservations.reload();
        this.waitlist.reload();
      });
    });
  }

  protected readonly formatMoney = formatMoney;
  protected readonly formatTravelers = formatTravelers;
  protected readonly formatDate = formatDate;
  protected readonly formatDateRange = formatDateRange;
  protected readonly imageUrl = imageUrl;

  protected setStatus(status: string): void {
    this.status.set(status);
    this.page.set(1);
  }

  protected cancel(reservation: Reservation): void {
    if (!confirm(`Cancel reservation ${reservation.code} for "${reservation.tripName}"?`)) {
      return;
    }

    this.busyId.set(reservation.id);
    this.actionError.set(null);

    this.http.post(`/api/reservations/${reservation.id}/cancel`, null).subscribe({
      next: () => {
        this.busyId.set(null);
        this.reservations.reload();
      },
      error: (error) => {
        this.busyId.set(null);
        this.actionError.set(errorMessage(error));
      },
    });
  }

  protected leaveWaitlist(entry: WaitlistEntry): void {
    this.http.delete(`/api/waitlist/${entry.id}`).subscribe({
      next: () => this.waitlist.reload(),
      error: (error) => this.actionError.set(errorMessage(error)),
    });
  }

  protected hasEnded(reservation: Reservation): boolean {
    return reservation.endDate.slice(0, 10) <= new Date().toISOString().slice(0, 10);
  }

  protected errorText(error: unknown): string {
    return errorMessage(error);
  }
}
