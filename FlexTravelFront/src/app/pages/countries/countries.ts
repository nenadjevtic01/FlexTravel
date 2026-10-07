import { httpResource } from '@angular/common/http';
import { Component, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { DestinationFilters } from '../../core/destination-filters.service';
import { errorMessage, formatNumber } from '../../core/format';
import { Continent, Country } from '../../core/models';
import { SafetyMeter } from '../../shared/safety-meter';

@Component({
  selector: 'app-countries',
  imports: [SafetyMeter],
  templateUrl: './countries.html',
  styleUrl: './countries.scss',
})
export class Countries {
  private readonly router = inject(Router);
  private readonly filters = inject(DestinationFilters);

  protected readonly continentId = signal<number | null>(null);

  protected readonly continents = httpResource<Continent[]>(() => '/api/continents');
  protected readonly countries = httpResource<Country[]>(() => '/api/countries');

  protected readonly groups = computed(() => {
    if (!this.continents.hasValue() || !this.countries.hasValue()) {
      return [];
    }
    const countries = this.countries.value();
    return this.continents
      .value()
      .filter((c) => this.continentId() === null || c.id === this.continentId())
      .map((continent) => ({
        continent,
        countries: countries.filter((country) => country.continentId === continent.id),
      }))
      .filter((group) => group.countries.length > 0);
  });

  protected readonly formatNumber = formatNumber;

  protected showDestinations(country: Country): void {
    this.filters.showCountry(country.continentId, country.id);
    this.router.navigateByUrl('/');
  }

  protected errorText(error: unknown): string {
    return errorMessage(error);
  }
}
