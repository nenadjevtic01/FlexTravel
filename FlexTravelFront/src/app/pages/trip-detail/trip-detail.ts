import { HttpClient, httpResource } from '@angular/common/http';
import {
  Component,
  computed,
  effect,
  inject,
  input,
  linkedSignal,
  signal,
  untracked,
} from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../../core/auth.service';
import { CurrencyService } from '../../core/currency.service';
import { NotificationsService } from '../../core/notifications.service';
import { RecentlyViewedService } from '../../core/recently-viewed.service';
import {
  errorMessage,
  extraIcon,
  formatDate,
  formatDateRange,
  formatEuro,
  formatMoney,
  mealsText,
  formatTravelers,
  httpStatus,
  imageUrl,
  offerLabel,
  transportIcon,
} from '../../core/format';
import {
  BookingRequest,
  PriceQuote,
  TripAccommodation,
  TripDetail,
  TripExtra,
  WaitlistEntry,
  Profile,
  SimilarTrips,
  Traveler,
} from '../../core/models';
import { attractionIcon } from '../../shared/attraction-icon';
import { CancellationPolicy } from '../../shared/booking-bits';
import { CompareButton } from '../../shared/compare-button';
import { RatingBreakdownView } from '../../shared/rating-breakdown';
import { WeatherCard } from '../../shared/weather-card';
import { TripCard } from '../../shared/trip-card';
import { RatingBadge } from '../../shared/rating';
import { ReviewList } from '../../shared/review-list';
import { Stars } from '../../shared/stars';
import { Stepper } from '../../shared/stepper';

@Component({
  selector: 'app-trip-detail',
  imports: [
    RouterLink,
    Stars,
    Stepper,
    RatingBadge,
    ReviewList,
    CancellationPolicy,
    TripCard,
    CompareButton,
    RatingBreakdownView,
    WeatherCard,
  ],
  templateUrl: './trip-detail.html',
  styleUrl: './trip-detail.scss',
})
export class TripDetailPage {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  protected readonly auth = inject(AuthService);
  protected readonly currency = inject(CurrencyService);

  readonly id = input.required<string>();

  protected readonly trip = httpResource<TripDetail>(() => `/api/trips/${this.id()}`);

  protected readonly adults = signal(2);
  protected readonly children = signal(0);

  protected readonly promoInput = signal('');
  protected readonly promoCode = signal('');

  protected readonly hotelId = linkedSignal<number | null>(() =>
    this.trip.hasValue() ? (this.trip.value().accommodations[0]?.id ?? null) : null,
  );

  protected readonly hotel = computed(() =>
    this.trip.hasValue()
      ? this.trip.value().accommodations.find((a) => a.id === this.hotelId())
      : undefined,
  );

  protected readonly rooms = linkedSignal<TripAccommodation | undefined, Record<number, number>>({
    source: this.hotel,
    computation: (hotel) => defaultRooms(hotel),
  });

  protected readonly extraCounts = linkedSignal<TripDetail | undefined, Record<number, number>>({
    source: () => (this.trip.hasValue() ? this.trip.value() : undefined),
    computation: (trip, previous) =>
      previous && trip && previous.source?.id === trip.id ? previous.value : {},
  });

  protected readonly travelerCount = computed(() => this.adults() + this.children());

  protected readonly request = computed<BookingRequest | undefined>(() => {
    const hotelId = this.hotelId();
    if (!this.trip.hasValue() || hotelId === null) {
      return undefined;
    }
    const travelers = this.travelerCount();
    return {
      tripId: this.trip.value().id,
      accommodationId: hotelId,
      adults: this.adults(),
      children: this.children(),
      rooms: Object.entries(this.rooms())
        .filter(([, count]) => count > 0)
        .map(([roomId, count]) => ({ roomId: Number(roomId), count })),

      extras: Object.entries(this.extraCounts())
        .filter(([, count]) => count > 0)
        .map(([extraId, count]) => ({
          extraId: Number(extraId),
          quantity: Math.min(count, travelers),
        })),
      promoCode: this.promoCode() || null,
    };
  });

  protected readonly waitlist = httpResource<WaitlistEntry[]>(() =>
    this.auth.canUseWaitlist() ? '/api/waitlist/mine' : undefined,
  );

  protected readonly myWaitlistEntry = computed(() =>
    this.waitlist.hasValue()
      ? this.waitlist.value().find((w) => w.tripId === Number(this.id()))
      : undefined,
  );

  protected readonly waitlistBusy = signal(false);
  protected readonly waitlistError = signal<string | null>(null);

  protected readonly similar = httpResource<SimilarTrips>(() => `/api/trips/${this.id()}/similar`);

  private readonly profile = httpResource<Profile>(() =>
    this.auth.canBook() ? '/api/profile' : undefined,
  );
  protected readonly travelers = signal<Traveler[]>([]);

  constructor() {
    const recent = inject(RecentlyViewedService);
    effect(() => {
      if (this.trip.hasValue()) {
        const trip = this.trip.value();
        untracked(() => recent.add(trip));
      }
    });

    const notifications = inject(NotificationsService);
    effect(() => {
      const latest = notifications.latest();
      untracked(() => {
        if (latest?.link === `/trips/${this.id()}`) {
          this.trip.reload();
          this.quote.reload();
          this.waitlist.reload();
        }
      });
    });
  }

  protected readonly quote = httpResource<PriceQuote>(() => {
    const body = this.request();
    return body ? { url: '/api/trips/quote', method: 'POST', body } : undefined;
  });

  protected readonly lastQuote = linkedSignal<PriceQuote | undefined, PriceQuote | undefined>({
    source: () => (this.quote.hasValue() ? this.quote.value() : undefined),
    computation: (quote, previous) => quote ?? previous?.value,
  });

  protected readonly step = signal<'configure' | 'review'>('configure');
  protected readonly note = signal('');
  protected readonly submitting = signal(false);
  protected readonly submitError = signal<string | null>(null);
  protected readonly selectedImage = linkedSignal({ source: this.id, computation: () => 0 });

  protected readonly formatMoney = formatMoney;
  protected readonly formatEuro = formatEuro;
  protected readonly extraIcon = extraIcon;
  protected readonly mealsText = mealsText;
  protected readonly formatTravelers = formatTravelers;
  protected readonly formatDateRange = formatDateRange;
  protected readonly imageUrl = imageUrl;
  protected readonly transportIcon = transportIcon;
  protected readonly offerLabel = offerLabel;
  protected readonly attractionIcon = attractionIcon;
  protected readonly formatDate = formatDate;

  protected selectHotel(value: string): void {
    this.hotelId.set(Number(value));
  }

  protected setRoom(roomId: number, count: number): void {
    this.rooms.update((rooms) => ({ ...rooms, [roomId]: count }));
  }

  protected extraCount(extra: TripExtra): number {
    const count = this.extraCounts()[extra.id] ?? 0;
    return extra.perPerson ? Math.min(count, this.travelerCount()) : count;
  }

  protected toggleExtra(extra: TripExtra, checked: boolean): void {
    this.setExtra(extra, checked ? (extra.perPerson ? this.travelerCount() : 1) : 0);
  }

  protected setExtra(extra: TripExtra, count: number): void {
    this.extraCounts.update((counts) => ({ ...counts, [extra.id]: count }));
  }

  protected cheapestRoom(hotel: TripAccommodation): number {
    return Math.min(...hotel.rooms.map((r) => r.pricePerNight));
  }

  protected applyPromo(): void {
    this.promoCode.set(this.promoInput().trim());
  }

  protected removePromo(): void {
    this.promoInput.set('');
    this.promoCode.set('');
  }

  protected notEnoughSeats(quote: PriceQuote): boolean {
    return quote.seatsLeft < quote.adults + quote.children;
  }

  protected joinWaitlist(): void {
    this.changeWaitlist(
      this.http.post('/api/waitlist', {
        tripId: Number(this.id()),
        adults: this.adults(),
        children: this.children(),
      }),
    );
  }

  protected leaveWaitlist(entry: WaitlistEntry): void {
    this.changeWaitlist(this.http.delete(`/api/waitlist/${entry.id}`));
  }

  private changeWaitlist(request: Observable<unknown>): void {
    this.waitlistBusy.set(true);
    this.waitlistError.set(null);
    request.subscribe({
      next: () => {
        this.waitlistBusy.set(false);
        this.waitlist.reload();
      },
      error: (error) => {
        this.waitlistBusy.set(false);
        this.waitlistError.set(errorMessage(error));
      },
    });
  }

  protected continueToReview(): void {
    this.submitError.set(null);

    const count = this.adults() + this.children();
    const previous = this.travelers();
    const me = this.profile.value();
    this.travelers.set(
      Array.from(
        { length: count },
        (_, i) =>
          previous[i] ??
          (i === 0 && me
            ? { firstName: me.firstName, lastName: me.lastName, dateOfBirth: '' }
            : { firstName: '', lastName: me?.lastName ?? '', dateOfBirth: '' }),
      ),
    );
    this.step.set('review');
  }

  protected isChildRow(index: number): boolean {
    return index >= this.adults();
  }

  protected setTraveler(index: number, change: Partial<Traveler>): void {
    this.travelers.update((list) => list.map((t, i) => (i === index ? { ...t, ...change } : t)));
  }

  protected confirm(): void {
    const body = this.request();
    if (!body || this.submitting()) return;

    this.submitting.set(true);
    this.submitError.set(null);

    this.http
      .post<{ id: number; code: string }>('/api/reservations', {
        ...body,
        note: this.note() || null,
        travelers: this.travelers(),
      })
      .subscribe({
        next: ({ code }) => {
          this.submitting.set(false);
          this.router.navigate(['/my-trips'], { queryParams: { booked: code } });
        },
        error: (error) => {
          this.submitting.set(false);
          this.submitError.set(errorMessage(error));

          this.trip.reload();
          this.quote.reload();
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

function defaultRooms(hotel: TripAccommodation | undefined): Record<number, number> {
  if (!hotel) return {};
  const room =
    hotel.rooms.find((r) => r.roomType === 'Double Room' && r.available > 0) ??
    hotel.rooms.find((r) => r.available > 0);
  return room ? { [room.id]: 1 } : {};
}
