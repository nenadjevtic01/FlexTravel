import { HttpClient, httpResource } from '@angular/common/http';
import {
  afterNextRender,
  Component,
  effect,
  inject,
  Injector,
  input,
  signal,
  untracked,
} from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { CurrencyService } from '../../core/currency.service';
import {
  errorMessage,
  formatDate,
  formatDateRange,
  formatDateTime,
  formatEuro,
  formatMoney,
  formatTravelers,
  httpStatus,
  imageUrl,
  offerLabel,
} from '../../core/format';
import { ReservationDetail, ReservationStatus } from '../../core/models';
import { NotificationsService } from '../../core/notifications.service';
import { CancellationPolicy } from '../../shared/booking-bits';
import { StarInput } from '../../shared/rating';
import {
  ReservationMessages,
  ReservationPayments,
  ReservationTravelers,
} from './reservation-parts';
import { Stars } from '../../shared/stars';
import { StatusBadge } from '../../shared/status-badge';

@Component({
  selector: 'app-reservation-detail',
  imports: [
    RouterLink,
    StatusBadge,
    Stars,
    StarInput,
    CancellationPolicy,
    ReservationTravelers,
    ReservationPayments,
    ReservationMessages,
  ],
  templateUrl: './reservation-detail.html',
  styleUrl: './reservation-detail.scss',
})
export class ReservationDetailPage {
  private readonly http = inject(HttpClient);
  protected readonly auth = inject(AuthService);

  readonly id = input.required<string>();

  protected readonly reservation = httpResource<ReservationDetail>(
    () => `/api/reservations/${this.id()}`,
  );

  protected readonly rejecting = signal(false);
  protected readonly reason = signal('');
  protected readonly busy = signal(false);
  protected readonly actionError = signal<string | null>(null);

  protected readonly tripRating = signal(0);
  protected readonly hotelRating = signal(0);
  protected readonly guideRating = signal(0);
  protected readonly organisationRating = signal(0);
  protected readonly valueRating = signal(0);
  protected readonly currency = inject(CurrencyService);
  protected readonly formatEuro = formatEuro;
  protected readonly comment = signal('');
  protected readonly reviewError = signal<string | null>(null);

  constructor() {
    const notifications = inject(NotificationsService);
    const route = inject(ActivatedRoute);
    const injector = inject(Injector);

    effect(() => {
      const latest = notifications.latest();
      untracked(() => {
        if (latest?.link === `/reservations/${this.id()}`) {
          this.reservation.reload();
        }
      });
    });

    effect(() => {
      if (this.reservation.hasValue() && route.snapshot.fragment === 'review') {
        afterNextRender(
          () => document.getElementById('review')?.scrollIntoView({ behavior: 'smooth' }),
          { injector },
        );
      }
    });
  }

  protected readonly formatMoney = formatMoney;
  protected readonly formatDate = formatDate;
  protected readonly formatDateTime = formatDateTime;
  protected readonly formatDateRange = formatDateRange;
  protected readonly formatTravelers = formatTravelers;
  protected readonly imageUrl = imageUrl;
  protected readonly offerLabel = offerLabel;

  protected submitReview(r: ReservationDetail): void {
    if (!this.tripRating() || !this.hotelRating()) {
      this.reviewError.set('Rate both the trip and the hotel.');
      return;
    }

    this.busy.set(true);
    this.reviewError.set(null);
    this.http
      .post('/api/reviews', {
        reservationId: r.id,
        tripRating: this.tripRating(),
        hotelRating: this.hotelRating(),
        guideRating: this.guideRating() || null,
        organisationRating: this.organisationRating() || null,
        valueRating: this.valueRating() || null,
        comment: this.comment(),
      })
      .subscribe({
        next: () => {
          this.busy.set(false);
          this.reservation.reload();
        },
        error: (error) => {
          this.busy.set(false);
          this.reviewError.set(errorMessage(error));
        },
      });
  }

  protected historyLabel(status: ReservationStatus, role: string): string {
    switch (status) {
      case 'Pending':
        return 'Reservation made';
      case 'Confirmed':
        return 'Confirmed by the agency';
      case 'Rejected':
        return 'Rejected by the agency';
      case 'Expired':
        return 'Expired: not confirmed in time';
      default:
        return role === 'Customer' ? 'Cancelled by the customer' : 'Cancelled';
    }
  }

  protected cancel(r: ReservationDetail): void {
    const cost =
      r.cancellationFeeToday > 0
        ? `The cancellation fee is ${formatMoney(r.cancellationFeeToday)} (${r.cancellationFeePercentToday}% of the price).`
        : 'It is free of charge.';
    if (!confirm(`Cancel reservation ${r.code}? ${cost}`)) return;
    this.post(`/api/reservations/${r.id}/cancel`, null);
  }

  protected confirmReservation(r: ReservationDetail): void {
    this.post(`/api/agency/reservations/${r.id}/confirm`, null);
  }

  protected reject(r: ReservationDetail): void {
    this.post(`/api/agency/reservations/${r.id}/reject`, { reason: this.reason() || null }, () => {
      this.rejecting.set(false);
      this.reason.set('');
    });
  }

  private post(url: string, body: unknown, done?: () => void): void {
    this.busy.set(true);
    this.actionError.set(null);
    this.http.post(url, body).subscribe({
      next: () => {
        this.busy.set(false);
        done?.();
        this.reservation.reload();
      },
      error: (error) => {
        this.busy.set(false);
        this.actionError.set(errorMessage(error));
      },
    });
  }

  protected isNotFound(error: unknown): boolean {
    return httpStatus(error) === 404;
  }

  protected errorText(error: unknown): string {
    return errorMessage(error);
  }
}
