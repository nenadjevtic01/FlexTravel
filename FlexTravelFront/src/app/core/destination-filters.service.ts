import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class DestinationFilters {
  readonly keyword = signal('');
  readonly continentId = signal<number | null>(null);
  readonly countryId = signal<number | null>(null);
  readonly page = signal(1);

  setContinent(id: number | null): void {
    this.continentId.set(id);
    this.countryId.set(null);
    this.page.set(1);
  }

  setCountry(id: number | null): void {
    this.countryId.set(id);
    this.page.set(1);
  }

  setKeyword(value: string): void {
    this.keyword.set(value);
    this.page.set(1);
  }

  showCountry(continentId: number, countryId: number): void {
    this.keyword.set('');
    this.continentId.set(continentId);
    this.countryId.set(countryId);
    this.page.set(1);
  }
}
