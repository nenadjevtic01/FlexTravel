import { HttpClient, httpResource } from '@angular/common/http';
import {
  Component,
  computed,
  effect,
  inject,
  input,
  OnInit,
  signal,
  untracked,
} from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import {
  errorMessage,
  formatDate,
  formatDateRange,
  formatDateTime,
  formatMoney,
  formatTravelers,
  offerLabel,
} from '../../core/format';
import { AgencyOverview, AgencyTrip, PagedResponse, Reservation } from '../../core/models';
import { NotificationsService } from '../../core/notifications.service';
import { Pagination } from '../../shared/pagination';
import { PaymentBadge } from '../../shared/booking-bits';
import { RatingBadge } from '../../shared/rating';
import { ReviewList } from '../../shared/review-list';
import { StatusBadge } from '../../shared/status-badge';
import { Avatar } from '../../shared/avatar';
import { AgencyProfileEditor } from './agency-profile';
import { AgencyCalendarView } from './calendar';
import { PromoCodes } from './promo-codes';
import { AgencyReportView } from './report';

type Tab =
  'reservations' | 'trips' | 'calendar' | 'report' | 'reviews' | 'promo' | 'team' | 'profile';
const TABS: Tab[] = [
  'reservations',
  'trips',
  'calendar',
  'report',
  'reviews',
  'promo',
  'team',
  'profile',
];

@Component({
  selector: 'app-agency',
  imports: [
    RouterLink,
    Pagination,
    StatusBadge,
    RatingBadge,
    ReviewList,
    PromoCodes,
    PaymentBadge,
    AgencyCalendarView,
    AgencyReportView,
    AgencyProfileEditor,
    Avatar,
  ],
  templateUrl: './agency.html',
  styleUrl: './agency.scss',
})
export class Agency implements OnInit {
  private readonly http = inject(HttpClient);
  protected readonly auth = inject(AuthService);

  readonly tabParam = input<string | undefined>(undefined, { alias: 'tab' });
  readonly saved = input<string>();
  readonly created = input<string>();

  protected readonly tab = signal<Tab>('reservations');
  protected readonly notice = signal<string | null>(null);
  protected readonly actionError = signal<string | null>(null);
  protected readonly busyId = signal<number | null>(null);

  protected readonly rejectingId = signal<number | null>(null);
  protected readonly rejectReason = signal('');

  protected readonly overview = httpResource<AgencyOverview>(() =>
    this.auth.isAgency() ? '/api/agency/trips' : undefined,
  );

  protected readonly statuses = ['Pending', 'Confirmed', 'Cancelled', 'Rejected', 'Expired', ''];
  protected readonly status = signal('Pending');
  protected readonly tripId = signal<number | null>(null);
  protected readonly page = signal(1);

  protected readonly reservations = httpResource<PagedResponse<Reservation>>(() => {
    if (!this.auth.isAgency()) return undefined;
    const params: Record<string, string | number> = { page: this.page(), perPage: 10 };
    if (this.status()) params['status'] = this.status();
    if (this.tripId() !== null) params['tripId'] = this.tripId()!;
    return { url: '/api/agency/reservations', params };
  });

  protected readonly stats = computed(() => {
    if (!this.overview.hasValue()) return null;
    const today = new Date().toISOString().slice(0, 10);
    const trips = this.overview.value().trips;
    const upcoming = trips.filter((t) => t.isActive && t.startDate.slice(0, 10) > today);
    return {
      upcoming: upcoming.length,
      seats: upcoming.reduce((sum, t) => sum + t.seatsBooked, 0),
      capacity: upcoming.reduce((sum, t) => sum + t.capacity, 0),
      pending: trips.reduce((sum, t) => sum + t.pendingReservations, 0),
    };
  });

  protected readonly unansweredOnly = signal(true);
  protected readonly reviewFilter = computed<Record<string, string | number | boolean>>(() => {
    const filter: Record<string, string | number | boolean> = {
      agencyId: this.overview.value()?.agencyId ?? 0,
    };
    if (this.unansweredOnly()) filter['unanswered'] = true;
    return filter;
  });

  constructor() {
    const notifications = inject(NotificationsService);
    effect(() => {
      if (!notifications.latest()) return;
      untracked(() => {
        this.reservations.reload();
        this.overview.reload();
      });
    });
  }

  protected readonly filteredTrip = computed(() =>
    this.overview.hasValue()
      ? this.overview.value().trips.find((t) => t.id === this.tripId())
      : undefined,
  );

  protected readonly formatMoney = formatMoney;
  protected readonly formatTravelers = formatTravelers;
  protected readonly formatDate = formatDate;
  protected readonly formatDateTime = formatDateTime;
  protected readonly formatDateRange = formatDateRange;
  protected readonly offerLabel = offerLabel;

  ngOnInit(): void {
    const tab = this.tabParam() as Tab;
    if (TABS.includes(tab)) {
      this.tab.set(tab);
    }
    if (this.created()) {
      this.notice.set('The trip is published and visible to customers.');
    } else if (this.saved()) {
      this.notice.set('Changes saved.');
    }
  }

  protected selectTab(tab: Tab): void {
    this.notice.set(null);
    this.actionError.set(null);
    this.tab.set(tab);
  }

  protected setStatus(status: string): void {
    this.status.set(status);
    this.page.set(1);
  }

  protected showTripReservations(trip: AgencyTrip): void {
    this.tripId.set(trip.id);
    this.status.set('');
    this.page.set(1);
    this.selectTab('reservations');
  }

  protected clearTripFilter(): void {
    this.tripId.set(null);
    this.page.set(1);
  }

  protected readonly addingDateFor = signal<number | null>(null);
  protected readonly newDate = signal('');
  protected readonly tomorrow = new Date(Date.now() + 86_400_000).toISOString().slice(0, 10);

  protected startAddDate(trip: AgencyTrip): void {
    this.addingDateFor.set(trip.id);
    this.newDate.set('');
  }

  protected nightsOf(trip: AgencyTrip): number {
    return Math.round((Date.parse(trip.endDate) - Date.parse(trip.startDate)) / 86_400_000);
  }

  protected addDate(trip: AgencyTrip): void {
    this.busyId.set(trip.id);
    this.actionError.set(null);
    this.http
      .post(`/api/agency/trips/${trip.id}/departures`, { startDate: this.newDate() })
      .subscribe({
        next: () => {
          this.busyId.set(null);
          this.addingDateFor.set(null);
          this.notice.set(`${trip.name} now also departs on ${formatDate(this.newDate())}.`);
          this.overview.reload();
        },
        error: (error) => {
          this.busyId.set(null);
          this.actionError.set(errorMessage(error));
        },
      });
  }

  protected tripState(trip: AgencyTrip): 'Withdrawn' | 'Departed' | 'On sale' {
    if (!trip.isActive) return 'Withdrawn';
    return trip.startDate.slice(0, 10) <= new Date().toISOString().slice(0, 10)
      ? 'Departed'
      : 'On sale';
  }

  protected occupancy(trip: AgencyTrip): number {
    return trip.capacity ? Math.min(100, Math.round((trip.seatsBooked / trip.capacity) * 100)) : 0;
  }

  protected confirmReservation(reservation: Reservation): void {
    this.run(reservation.id, `/api/agency/reservations/${reservation.id}/confirm`, null, () =>
      this.notice.set(`Reservation ${reservation.code} confirmed.`),
    );
  }

  protected startReject(reservation: Reservation): void {
    this.rejectingId.set(reservation.id);
    this.rejectReason.set('');
  }

  protected reject(reservation: Reservation): void {
    const body = { reason: this.rejectReason() || null };
    this.run(reservation.id, `/api/agency/reservations/${reservation.id}/reject`, body, () => {
      this.rejectingId.set(null);
      this.notice.set(`Reservation ${reservation.code} rejected.`);
    });
  }

  protected withdraw(trip: AgencyTrip): void {
    if (!confirm(`Withdraw "${trip.name}" from sale?`)) return;
    this.run(trip.id, `/api/agency/trips/${trip.id}/deactivate`, null, () =>
      this.notice.set(`"${trip.name}" is no longer on sale.`),
    );
  }

  private run(id: number, url: string, body: unknown, done: () => void): void {
    this.busyId.set(id);
    this.actionError.set(null);
    this.notice.set(null);

    this.http.post(url, body).subscribe({
      next: () => {
        this.busyId.set(null);
        done();
        this.reservations.reload();
        this.overview.reload();
      },
      error: (error) => {
        this.busyId.set(null);
        this.actionError.set(errorMessage(error));
      },
    });
  }

  protected errorText(error: unknown): string {
    return errorMessage(error);
  }
}
