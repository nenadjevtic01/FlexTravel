import { HttpClient, httpResource } from '@angular/common/http';
import { Component, computed, inject, OnDestroy, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { DestinationFilters } from '../../core/destination-filters.service';
import { errorMessage, formatDateRange, imageUrl } from '../../core/format';
import {
  Continent,
  Country,
  DestinationListItem,
  PagedResponse,
  TripListItem,
} from '../../core/models';
import { RecentlyViewedService } from '../../core/recently-viewed.service';
import { FavoriteButton } from '../../shared/favorite-button';
import { MapMarker, MapView } from '../../shared/map-view';
import { Pagination } from '../../shared/pagination';
import { SafetyMeter } from '../../shared/safety-meter';
import { TripCard } from '../../shared/trip-card';
import { TripSearch } from '../../shared/trip-search';

const PER_PAGE = 12;

@Component({
  selector: 'app-destinations',
  imports: [RouterLink, Pagination, SafetyMeter, TripCard, TripSearch, FavoriteButton, MapView],
  templateUrl: './destinations.html',
  styleUrl: './destinations.scss',
})
export class Destinations implements OnDestroy {
  private readonly http = inject(HttpClient);
  protected readonly filters = inject(DestinationFilters);
  protected readonly imageUrl = imageUrl;
  protected readonly formatDateRange = formatDateRange;
  protected readonly recent = inject(RecentlyViewedService);
  private readonly router = inject(Router);

  private readonly allDestinations = httpResource<PagedResponse<DestinationListItem>>(() => ({
    url: '/api/destinations',
    params: { perPage: 100 },
  }));

  protected readonly mapMarkers = computed<MapMarker[]>(() =>
    (this.allDestinations.value()?.items ?? [])
      .filter((d) => d.latitude !== null && d.longitude !== null)
      .map((d) => ({
        id: d.id,
        lat: d.latitude!,
        lng: d.longitude!,
        label: `${d.name}, ${d.country}`,
      })),
  );

  protected openDestination(id: number): void {
    this.router.navigate(['/destinations', id]);
  }

  protected readonly continents = httpResource<Continent[]>(() => '/api/continents');

  protected readonly upcoming = httpResource<PagedResponse<TripListItem>>(() => ({
    url: '/api/trips',
    params: { perPage: 4, sort: 'date' },
  }));

  protected readonly countries = httpResource<Country[]>(() => {
    const continentId = this.filters.continentId();
    return continentId === null ? undefined : { url: '/api/countries', params: { continentId } };
  });

  protected readonly destinations = httpResource<PagedResponse<DestinationListItem>>(() => {
    const params: Record<string, string | number> = {
      page: this.filters.page(),
      perPage: PER_PAGE,
    };
    const keyword = this.filters.keyword().trim();
    const continentId = this.filters.continentId();
    const countryId = this.filters.countryId();

    if (keyword) params['keyword'] = keyword;
    if (continentId !== null) params['continentId'] = continentId;
    if (countryId !== null) params['countryId'] = countryId;

    return { url: '/api/destinations', params };
  });

  protected readonly totalDestinations = computed(() =>
    this.continents.hasValue()
      ? this.continents.value().reduce((sum, c) => sum + c.destinationsCount, 0)
      : 0,
  );
  protected readonly databaseEmpty = computed(
    () => this.continents.hasValue() && this.continents.value().length === 0,
  );

  protected readonly keywordInput = signal(this.filters.keyword());
  protected readonly seeding = signal(false);
  protected readonly seedError = signal<string | null>(null);
  protected readonly skeletons = Array.from({ length: 6 });

  private keywordTimer: ReturnType<typeof setTimeout> | undefined;

  protected onKeyword(value: string): void {
    this.keywordInput.set(value);
    clearTimeout(this.keywordTimer);
    this.keywordTimer = setTimeout(() => this.filters.setKeyword(value), 300);
  }

  protected clearFilters(): void {
    this.keywordInput.set('');
    this.filters.setKeyword('');
    this.filters.setContinent(null);
  }

  protected onCountryChange(value: string): void {
    this.filters.setCountry(value ? Number(value) : null);
  }

  protected changePage(page: number): void {
    this.filters.page.set(page);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  protected fillDatabase(): void {
    this.seeding.set(true);
    this.seedError.set(null);

    this.http.post('/api/initial', null).subscribe({
      next: () => {
        this.seeding.set(false);
        this.continents.reload();
        this.destinations.reload();
        this.upcoming.reload();
      },
      error: (error) => {
        this.seeding.set(false);
        this.seedError.set(errorMessage(error));
      },
    });
  }

  protected errorText(error: unknown): string {
    return errorMessage(error);
  }

  ngOnDestroy(): void {
    clearTimeout(this.keywordTimer);
  }
}
