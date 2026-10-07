import { httpResource } from '@angular/common/http';
import { Component, computed, inject, input, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TripDestinationLookup } from '../core/models';
import { TripFilters } from '../core/trip-filters.service';

@Component({
  selector: 'app-trip-search',
  templateUrl: './trip-search.html',
  styleUrl: './trip-search.scss',
})
export class TripSearch {
  private readonly router = inject(Router);
  private readonly filters = inject(TripFilters);

  readonly compact = input(false);

  protected readonly destinations = httpResource<TripDestinationLookup[]>(
    () => '/api/trips/destinations',
  );

  protected readonly groups = computed(() => {
    const list = this.destinations.hasValue() ? this.destinations.value() : [];
    const byCountry = new Map<string, TripDestinationLookup[]>();
    for (const d of list) {
      byCountry.set(d.country, [...(byCountry.get(d.country) ?? []), d]);
    }
    return [...byCountry.entries()].map(([country, destinations]) => ({ country, destinations }));
  });

  protected readonly destinationId = signal<number | null>(this.filters.destinationId());
  protected readonly dateFrom = signal(this.filters.dateFrom());
  protected readonly dateTo = signal(this.filters.dateTo());
  protected readonly travelers = signal<number | null>(this.filters.travelers() ?? 2);

  protected readonly travelerOptions = [1, 2, 3, 4, 5, 6, 7, 8];
  protected readonly today = new Date().toISOString().slice(0, 10);

  protected toNumber(value: string): number | null {
    return value ? Number(value) : null;
  }

  protected search(event: Event): void {
    event.preventDefault();
    const queryParams: Record<string, string | number> = {};
    if (this.destinationId() !== null) queryParams['destinationId'] = this.destinationId()!;
    if (this.dateFrom()) queryParams['from'] = this.dateFrom();
    if (this.dateTo()) queryParams['to'] = this.dateTo();
    if (this.travelers() !== null) queryParams['travelers'] = this.travelers()!;
    this.router.navigate(['/trips'], { queryParams });
  }
}
