import { httpResource } from '@angular/common/http';
import { Component, computed, inject, input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { errorMessage, formatDate, formatMoney } from '../../core/format';
import { PagedResponse, TripDestinationLookup, TripListItem } from '../../core/models';
import { TripFilters } from '../../core/trip-filters.service';
import { Pagination } from '../../shared/pagination';
import { TripCard } from '../../shared/trip-card';

const PER_PAGE = 12;

function numberOrNull(value: string | undefined | null): number | null {
  return value ? Number(value) : null;
}

@Component({
  selector: 'app-trips',
  imports: [Pagination, TripCard],
  templateUrl: './trips.html',
  styleUrl: './trips.scss',
})
export class Trips implements OnInit {
  private readonly router = inject(Router);
  protected readonly filters = inject(TripFilters);

  readonly destinationId = input<string>();
  readonly from = input<string>();
  readonly to = input<string>();
  readonly travelers = input<string>();

  protected readonly destinations = httpResource<TripDestinationLookup[]>(
    () => '/api/trips/destinations',
  );

  protected readonly trips = httpResource<PagedResponse<TripListItem>>(() => {
    const f = this.filters;
    const params: Record<string, string | number> = {
      page: f.page(),
      perPage: PER_PAGE,
      sort: f.sort(),
    };

    if (f.destinationId() !== null) params['destinationId'] = f.destinationId()!;
    if (f.dateFrom()) params['dateFrom'] = f.dateFrom();
    if (f.dateTo()) params['dateTo'] = f.dateTo();
    if (f.travelers() !== null) params['travelers'] = f.travelers()!;
    if (f.maxPrice() !== null) params['maxPrice'] = f.maxPrice()!;
    if (f.deals()) params['deals'] = 'true';

    return { url: '/api/trips', params };
  });

  protected readonly selectedDestination = computed(() =>
    this.destinations.hasValue()
      ? this.destinations.value().find((d) => d.id === this.filters.destinationId())
      : undefined,
  );

  protected readonly summary = computed(() => {
    const parts: string[] = [];
    const destination = this.selectedDestination();
    if (destination) parts.push(`to ${destination.name}`);
    const from = this.filters.dateFrom();
    const to = this.filters.dateTo();
    if (from && to) parts.push(`departing ${formatDate(from)} – ${formatDate(to)}`);
    else if (from) parts.push(`departing after ${formatDate(from)}`);
    else if (to) parts.push(`departing before ${formatDate(to)}`);
    const travelers = this.filters.travelers();
    if (travelers) parts.push(`for ${travelers} ${travelers === 1 ? 'person' : 'people'}`);
    return parts.join(', ');
  });

  protected readonly travelerOptions = [1, 2, 3, 4, 5, 6, 7, 8];
  protected readonly priceOptions = [500, 1000, 1500, 2000];
  protected readonly formatMoney = formatMoney;
  protected readonly today = new Date().toISOString().slice(0, 10);
  protected readonly skeletons = Array.from({ length: 6 });

  ngOnInit(): void {
    if (this.destinationId() || this.from() || this.to() || this.travelers()) {
      this.filters.apply({
        destinationId: numberOrNull(this.destinationId()),
        dateFrom: this.from() ?? '',
        dateTo: this.to() ?? '',
        travelers: numberOrNull(this.travelers()),
      });

      this.router.navigate([], { replaceUrl: true });
    }
  }

  protected setDestination(value: string): void {
    this.filters.update(() => this.filters.destinationId.set(numberOrNull(value)));
  }

  protected setDateFrom(value: string): void {
    this.filters.update(() => this.filters.dateFrom.set(value));
  }

  protected setDateTo(value: string): void {
    this.filters.update(() => this.filters.dateTo.set(value));
  }

  protected setTravelers(value: string): void {
    this.filters.update(() => this.filters.travelers.set(numberOrNull(value)));
  }

  protected setMaxPrice(value: string): void {
    this.filters.update(() => this.filters.maxPrice.set(numberOrNull(value)));
  }

  protected setDeals(value: boolean): void {
    this.filters.update(() => this.filters.deals.set(value));
  }

  protected setSort(value: string): void {
    this.filters.update(() => this.filters.sort.set(value));
  }

  protected reset(): void {
    this.filters.reset();
  }

  protected changePage(page: number): void {
    this.filters.page.set(page);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  protected errorText(error: unknown): string {
    return errorMessage(error);
  }
}
