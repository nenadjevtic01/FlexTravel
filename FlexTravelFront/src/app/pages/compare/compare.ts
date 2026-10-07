import { HttpClient } from '@angular/common/http';
import { Component, computed, inject, resource } from '@angular/core';
import { RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { CompareService, MAX_COMPARED } from '../../core/compare.service';
import {
  extraIcon,
  formatDate,
  formatDateRange,
  formatMoney,
  imageUrl,
  offerLabel,
  transportIcon,
} from '../../core/format';
import { TripDetail } from '../../core/models';

interface Column {
  trip: TripDetail;
  priceFrom: number;
  hotels: string;
  mealDays: number;
  sights: number;
}

function priceFrom(trip: TripDetail): number {
  const beds = trip.accommodations.flatMap((a) => a.rooms).filter((r) => r.capacity > 0);
  const cheapestBed = beds.length ? Math.min(...beds.map((r) => r.pricePerNight / r.capacity)) : 0;
  const before = Math.round(trip.pricePerPerson + cheapestBed * trip.nights);
  return Math.round(before - (before * trip.offerPercent) / 100);
}

@Component({
  selector: 'app-compare',
  imports: [RouterLink],
  templateUrl: './compare.html',
  styleUrl: './compare.scss',
})
export class Compare {
  private readonly http = inject(HttpClient);
  protected readonly compare = inject(CompareService);
  protected readonly max = MAX_COMPARED;

  protected readonly trips = resource({
    params: () => this.compare.ids(),
    loader: ({ params }) =>
      Promise.all(
        params.map((id) =>
          firstValueFrom(this.http.get<TripDetail>(`/api/trips/${id}`)).catch(() => null),
        ),
      ),
  });

  protected readonly columns = computed<Column[]>(() =>
    (this.trips.value() ?? [])
      .filter((t): t is TripDetail => t !== null)
      .map((trip) => ({
        trip,
        priceFrom: priceFrom(trip),
        hotels: trip.accommodations
          .map((a) => `${a.name} ${'★'.repeat(Math.round(a.stars))}`)
          .join(', '),
        mealDays: trip.itinerary.filter((d) => d.breakfast || d.lunch || d.dinner).length,
        sights: trip.attractions.length,
      })),
  );

  protected readonly missing = computed(() =>
    this.trips.hasValue() ? this.compare.count() - this.columns().length : 0,
  );

  protected readonly bestPrice = computed(() =>
    Math.min(...this.columns().map((c) => c.priceFrom)),
  );
  protected readonly bestPerNight = computed(() =>
    Math.min(...this.columns().map((c) => this.perNight(c))),
  );
  protected readonly bestRating = computed(() =>
    Math.max(0, ...this.columns().map((c) => c.trip.rating?.average ?? 0)),
  );
  protected readonly bestChild = computed(() =>
    Math.max(...this.columns().map((c) => c.trip.childDiscountPercent)),
  );

  protected readonly formatMoney = formatMoney;
  protected readonly formatDate = formatDate;
  protected readonly formatDateRange = formatDateRange;
  protected readonly imageUrl = imageUrl;
  protected readonly offerLabel = offerLabel;
  protected readonly transportIcon = transportIcon;
  protected readonly extraIcon = extraIcon;

  protected perNight(c: Column): number {
    return Math.round(c.priceFrom / Math.max(1, c.trip.nights));
  }
}
