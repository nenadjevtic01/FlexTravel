import { HttpClient, HttpErrorResponse, httpResource } from '@angular/common/http';
import {
  Component,
  computed,
  effect,
  inject,
  input,
  output,
  signal,
  untracked,
} from '@angular/core';
import { Observable } from 'rxjs';
import { errorMessage, formatDate, formatDateTime, formatMoney } from '../../core/format';
import {
  ExtraKind,
  ItineraryDayInput,
  TripExtraInput,
  TripForEdit,
  TripFormOptions,
} from '../../core/models';

const emptyDay = (): ItineraryDayInput => ({
  title: '',
  activity: '',
  breakfast: false,
  lunch: false,
  dinner: false,
  overnight: '',
});

@Component({
  selector: 'app-trip-form',
  templateUrl: './trip-form.html',
  styleUrl: './trip-form.scss',
})
export class TripForm {
  private readonly http = inject(HttpClient);

  readonly tripId = input<number | null>(null);
  readonly saved = output<number>();

  protected readonly options = httpResource<TripFormOptions>(() => '/api/agency/trip-options');
  protected readonly existing = httpResource<TripForEdit>(() => {
    const id = this.tripId();
    return id ? `/api/agency/trips/${id}` : undefined;
  });

  protected readonly name = signal('');
  protected readonly description = signal('');
  protected readonly destinationId = signal<number | null>(null);
  protected readonly transportId = signal<number | null>(null);
  protected readonly startDate = signal('');
  protected readonly endDate = signal('');
  protected readonly capacity = signal(30);
  protected readonly price = signal(300);
  protected readonly childDiscount = signal(30);
  protected readonly hotelIds = signal<number[]>([]);
  protected readonly attractionIds = signal<number[]>([]);
  protected readonly itinerary = signal<ItineraryDayInput[]>([emptyDay()]);
  protected readonly extras = signal<TripExtraInput[]>([]);
  protected readonly extraKinds: { value: ExtraKind; label: string }[] = [
    { value: 'Insurance', label: 'Insurance' },
    { value: 'Transfer', label: 'Transfer' },
    { value: 'Excursion', label: 'Excursion' },
    { value: 'Other', label: 'Other' },
  ];

  protected readonly earlyPercent = signal(0);
  protected readonly earlyUntil = signal('');
  protected readonly lastMinutePercent = signal(0);
  protected readonly lastMinuteDays = signal(14);

  protected readonly isEdit = computed(() => this.tripId() !== null);
  protected readonly ready = computed(
    () => this.options.hasValue() && (!this.isEdit() || this.existing.hasValue()),
  );
  protected readonly datesLocked = computed(
    () => this.existing.hasValue() && this.existing.value().activeReservations > 0,
  );
  protected readonly lockedHotels = computed(() =>
    this.existing.hasValue() ? this.existing.value().lockedAccommodationIds : [],
  );

  protected readonly destination = computed(() =>
    this.options.hasValue()
      ? this.options.value().destinations.find((d) => d.id === this.destinationId())
      : undefined,
  );

  protected readonly country = signal('');
  protected readonly countries = computed(() =>
    this.options.hasValue()
      ? [...new Set(this.options.value().destinations.map((d) => d.country))].sort()
      : [],
  );
  protected readonly countryDestinations = computed(() =>
    this.options.hasValue()
      ? this.options
          .value()
          .destinations.filter((d) => d.country === this.country())
          .sort((a, b) => a.name.localeCompare(b.name))
      : [],
  );

  protected readonly nights = computed(() => {
    if (!this.startDate() || !this.endDate()) return 0;
    const days = (Date.parse(this.endDate()) - Date.parse(this.startDate())) / 86_400_000;
    return days > 0 ? days : 0;
  });

  protected readonly minDate = new Date(Date.now() + 86_400_000).toISOString().slice(0, 10);
  protected readonly submitting = signal(false);
  protected readonly errors = signal<string[]>([]);

  protected readonly formatMoney = formatMoney;
  protected readonly formatDate = formatDate;
  protected readonly formatDateTime = formatDateTime;

  constructor() {
    effect(() => {
      const destination = this.destination();
      if (destination) {
        untracked(() => this.country.set(destination.country));
      }
    });

    effect(() => {
      if (!this.existing.hasValue()) return;
      const t = this.existing.value();
      this.name.set(t.name);
      this.description.set(t.description);
      this.destinationId.set(t.destinationId);
      this.transportId.set(t.transportId);
      this.startDate.set(t.startDate.slice(0, 10));
      this.endDate.set(t.endDate.slice(0, 10));
      this.capacity.set(t.capacity);
      this.price.set(t.pricePerPerson);
      this.childDiscount.set(t.childDiscountPercent);
      this.hotelIds.set([...t.accommodationIds]);
      this.attractionIds.set([...t.attractionIds]);
      this.itinerary.set(t.itinerary.length ? t.itinerary.map((d) => ({ ...d })) : [emptyDay()]);
      this.extras.set(t.extras.map((e) => ({ ...e })));
      this.earlyPercent.set(t.earlyBookingPercent);
      this.earlyUntil.set(t.earlyBookingUntil?.slice(0, 10) ?? '');
      this.lastMinutePercent.set(t.lastMinutePercent);
      this.lastMinuteDays.set(t.lastMinuteDays || 14);
    });
  }

  protected numberValue(value: string): number | null {
    return value === '' ? null : Number(value);
  }

  protected toNumber(value: string): number {
    return Number(value) || 0;
  }

  protected chooseCountry(value: string): void {
    this.country.set(value);
    this.destinationId.set(null);
    this.hotelIds.set([]);
    this.attractionIds.set([]);
  }

  protected chooseDestination(value: string): void {
    this.destinationId.set(this.numberValue(value));

    this.hotelIds.set(this.destination()?.accommodations.map((a) => a.id) ?? []);
    this.attractionIds.set([]);
  }

  protected toggleAttraction(id: number, checked: boolean): void {
    this.attractionIds.update((ids) => (checked ? [...ids, id] : ids.filter((x) => x !== id)));
  }

  protected toggleHotel(id: number, checked: boolean): void {
    this.hotelIds.update((ids) => (checked ? [...ids, id] : ids.filter((x) => x !== id)));
  }

  protected setDay<K extends keyof ItineraryDayInput>(
    index: number,
    key: K,
    value: ItineraryDayInput[K],
  ): void {
    this.itinerary.update((days) => days.map((d, i) => (i === index ? { ...d, [key]: value } : d)));
  }

  protected addDay(): void {
    this.itinerary.update((days) => [
      ...days,
      { ...emptyDay(), overnight: days.at(-1)?.overnight ?? '' },
    ]);
  }

  protected setExtra<K extends keyof TripExtraInput>(
    index: number,
    key: K,
    value: TripExtraInput[K],
  ): void {
    this.extras.update((list) => list.map((e, i) => (i === index ? { ...e, [key]: value } : e)));
  }

  protected addExtra(kind: ExtraKind = 'Excursion'): void {
    const presets: Record<ExtraKind, Partial<TripExtraInput>> = {
      Insurance: { name: 'Travel insurance', perPerson: true, price: 25 },
      Transfer: { name: 'Private airport transfer', perPerson: false, price: 45 },
      Excursion: { name: '', perPerson: true, price: 50 },
      Other: { name: '', perPerson: false, price: 20 },
    };
    this.extras.update((list) => [
      ...list,
      { id: null, kind, name: '', description: '', price: 0, perPerson: true, ...presets[kind] },
    ]);
  }

  protected removeExtra(index: number): void {
    this.extras.update((list) => list.filter((_, i) => i !== index));
  }

  protected removeDay(index: number): void {
    this.itinerary.update((days) => days.filter((_, i) => i !== index));
  }

  protected submit(event: Event): void {
    event.preventDefault();
    if (this.submitting()) return;

    this.submitting.set(true);
    this.errors.set([]);

    const body = {
      name: this.name(),
      description: this.description(),
      destinationId: this.destinationId() ?? 0,
      transportId: this.transportId() ?? 0,
      startDate: this.startDate() || null,
      endDate: this.endDate() || null,
      capacity: this.capacity(),
      pricePerPerson: this.price(),
      childDiscountPercent: this.childDiscount(),
      accommodationIds: this.hotelIds(),
      attractionIds: this.attractionIds(),
      itinerary: this.itinerary(),
      extras: this.extras(),
      earlyBookingPercent: this.earlyPercent(),
      earlyBookingUntil: this.earlyPercent() > 0 && this.earlyUntil() ? this.earlyUntil() : null,
      lastMinutePercent: this.lastMinutePercent(),
      lastMinuteDays: this.lastMinutePercent() > 0 ? this.lastMinuteDays() : 0,
    };

    const id = this.tripId();
    const request: Observable<{ id: number } | null> = id
      ? this.http.put<null>(`/api/agency/trips/${id}`, body)
      : this.http.post<{ id: number }>('/api/agency/trips', body);

    request.subscribe({
      next: (result) => {
        this.submitting.set(false);
        this.saved.emit(id ?? result?.id ?? 0);
      },
      error: (error) => {
        this.submitting.set(false);
        if (error instanceof HttpErrorResponse && Array.isArray(error.error?.errors)) {
          this.errors.set((error.error.errors as { error: string }[]).map((e) => e.error));
        } else {
          this.errors.set([errorMessage(error)]);
        }
      },
    });
  }
}
