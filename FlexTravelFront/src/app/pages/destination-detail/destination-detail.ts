import { httpResource } from '@angular/common/http';
import { Component, computed, inject, input, linkedSignal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { DestinationFilters } from '../../core/destination-filters.service';
import { errorMessage, formatNumber, httpStatus, imageUrl } from '../../core/format';
import { Accommodation, DestinationDetail, PagedResponse, TripListItem } from '../../core/models';
import { attractionIcon } from '../../shared/attraction-icon';
import { FavoriteButton } from '../../shared/favorite-button';
import { MapView } from '../../shared/map-view';
import { PriceAlertControl } from '../../shared/price-alert';
import { WeatherCard } from '../../shared/weather-card';
import { RatingBadge } from '../../shared/rating';
import { ReviewList } from '../../shared/review-list';
import { SafetyMeter } from '../../shared/safety-meter';
import { Stars } from '../../shared/stars';
import { TripCard } from '../../shared/trip-card';

@Component({
  selector: 'app-destination-detail',
  imports: [
    RouterLink,
    SafetyMeter,
    Stars,
    TripCard,
    FavoriteButton,
    RatingBadge,
    ReviewList,
    MapView,
    PriceAlertControl,
    WeatherCard,
  ],
  templateUrl: './destination-detail.html',
  styleUrl: './destination-detail.scss',
})
export class DestinationDetailPage {
  private readonly router = inject(Router);
  private readonly filters = inject(DestinationFilters);

  readonly id = input.required<string>();

  protected readonly destination = httpResource<DestinationDetail>(
    () => `/api/destinations/${this.id()}`,
  );

  protected readonly trips = httpResource<PagedResponse<TripListItem>>(() => ({
    url: '/api/trips',
    params: { destinationId: this.id(), perPage: 6 },
  }));

  protected readonly selectedImage = linkedSignal({ source: this.id, computation: () => 0 });

  protected readonly totals = computed(() => {
    if (!this.destination.hasValue()) {
      return { accommodations: 0, rooms: 0, beds: 0 };
    }
    const accommodations = this.destination.value().accommodations;
    return {
      accommodations: accommodations.length,
      rooms: accommodations.reduce((sum, a) => sum + this.roomsCount(a), 0),
      beds: accommodations.reduce((sum, a) => sum + this.bedsCount(a), 0),
    };
  });

  protected readonly imageUrl = imageUrl;
  protected readonly formatNumber = formatNumber;
  protected readonly attractionIcon = attractionIcon;

  protected roomsCount(accommodation: Accommodation): number {
    return accommodation.rooms.reduce((sum, r) => sum + r.numberOfRooms, 0);
  }

  protected bedsCount(accommodation: Accommodation): number {
    return accommodation.rooms.reduce((sum, r) => sum + r.numberOfRooms * r.capacity, 0);
  }

  protected showCountry(detail: DestinationDetail): void {
    this.filters.showCountry(detail.country.continentId, detail.country.id);
    this.router.navigateByUrl('/');
  }

  protected isNotFound(error: unknown): boolean {
    return httpStatus(error) === 404;
  }

  protected errorText(error: unknown): string {
    return errorMessage(error);
  }
}
