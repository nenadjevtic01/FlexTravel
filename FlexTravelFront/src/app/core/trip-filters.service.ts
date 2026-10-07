import { Injectable, signal } from '@angular/core';

export interface TripSearch {
  destinationId: number | null;
  dateFrom: string;
  dateTo: string;
  travelers: number | null;
}

@Injectable({ providedIn: 'root' })
export class TripFilters {
  readonly destinationId = signal<number | null>(null);
  readonly dateFrom = signal('');
  readonly dateTo = signal('');
  readonly travelers = signal<number | null>(null);
  readonly maxPrice = signal<number | null>(null);
  readonly sort = signal('date');
  readonly deals = signal(false);
  readonly page = signal(1);

  update(change: () => void): void {
    change();
    this.page.set(1);
  }

  apply(search: TripSearch): void {
    this.destinationId.set(search.destinationId);
    this.dateFrom.set(search.dateFrom);
    this.dateTo.set(search.dateTo);
    this.travelers.set(search.travelers);
    this.page.set(1);
  }

  reset(): void {
    this.apply({ destinationId: null, dateFrom: '', dateTo: '', travelers: null });
    this.maxPrice.set(null);
    this.deals.set(false);
    this.sort.set('date');
  }
}
