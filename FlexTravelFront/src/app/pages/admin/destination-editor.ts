import { HttpClient, HttpErrorResponse, httpResource } from '@angular/common/http';
import { Component, computed, effect, inject, input, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { errorMessage, imageUrl } from '../../core/format';
import {
  CatalogImage,
  CatalogOptions,
  Country,
  DestinationForEdit,
  HotelForEdit,
  SightForEdit,
} from '../../core/models';
import { Stars } from '../../shared/stars';
import { MapMarker, MapView } from '../../shared/map-view';
import { HotelEditor } from './hotel-editor';

function errorList(error: unknown): string[] {
  return error instanceof HttpErrorResponse && Array.isArray(error.error?.errors)
    ? (error.error.errors as { error: string }[]).map((e) => e.error)
    : [errorMessage(error)];
}

@Component({
  selector: 'app-destination-editor',
  imports: [RouterLink, Stars, HotelEditor, MapView],
  templateUrl: './destination-editor.html',
  styleUrl: './destination-editor.scss',
})
export class DestinationEditor {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  protected readonly auth = inject(AuthService);

  readonly id = input.required<string>();
  readonly countryId = input<string>();

  protected readonly isNew = computed(() => this.id() === 'new');

  protected readonly options = httpResource<CatalogOptions>(() =>
    this.auth.canManageCatalog() ? '/api/admin/catalog/options' : undefined,
  );
  protected readonly countries = httpResource<Country[]>(() => '/api/countries');
  protected readonly destination = httpResource<DestinationForEdit>(() =>
    this.auth.canManageCatalog() && !this.isNew()
      ? `/api/admin/catalog/destinations/${this.id()}`
      : undefined,
  );

  protected readonly country = signal<number | null>(null);
  protected readonly name = signal('');
  protected readonly description = signal('');
  protected readonly climate = signal('');
  protected readonly latitude = signal<number | null>(null);
  protected readonly longitude = signal<number | null>(null);
  protected readonly locationMarker = computed<MapMarker[]>(() => {
    const lat = this.latitude();
    const lng = this.longitude();
    return lat !== null && lng !== null
      ? [{ id: 0, lat, lng, label: this.name() || 'Destination' }]
      : [];
  });
  protected readonly images = signal<CatalogImage[]>([]);
  protected readonly uploading = signal(0);
  protected readonly saving = signal(false);
  protected readonly errors = signal<string[]>([]);
  protected readonly notice = signal<string | null>(null);

  protected readonly editingHotel = signal<number | null>(null);

  protected readonly draftHotels = signal<HotelForEdit[]>([]);
  protected readonly draftSights = signal<SightForEdit[]>([]);
  private nextDraftId = -1;

  protected readonly hotels = computed(() =>
    this.isNew() ? this.draftHotels() : (this.destination.value()?.hotels ?? []),
  );
  protected readonly sights = computed(() =>
    this.isNew() ? this.draftSights() : (this.destination.value()?.sights ?? []),
  );

  protected readonly editingSight = signal<number | null>(null);
  protected readonly sightName = signal('');
  protected readonly sightCategory = signal<number | null>(null);
  protected readonly sightDescription = signal('');
  protected readonly sightErrors = signal<string[]>([]);

  protected readonly imageUrl = imageUrl;

  constructor() {
    effect(() => {
      if (this.isNew()) {
        this.country.set(this.countryId() ? Number(this.countryId()) : null);
        return;
      }
      if (!this.destination.hasValue()) return;
      const d = this.destination.value();
      this.country.set(d.countryId);
      this.name.set(d.name);
      this.description.set(d.description);
      this.climate.set(d.climate ?? '');
      this.latitude.set(d.latitude);
      this.longitude.set(d.longitude);
      this.images.set(d.images.map((i) => ({ ...i })));
    });
  }

  protected countryName(id: number | null): string {
    return this.countries.value()?.find((c) => c.id === id)?.name ?? '';
  }

  protected typeName(id: number): string {
    return this.options.value()?.accommodationTypes.find((t) => t.id === id)?.name ?? '';
  }

  protected roomTypeName(id: number): string {
    return this.options.value()?.roomTypes.find((t) => t.id === id)?.name ?? '';
  }

  protected categoryName(id: number): string {
    return this.options.value()?.attractionCategories.find((c) => c.id === id)?.name ?? '';
  }

  protected upload(input: HTMLInputElement): void {
    const files = Array.from(input.files ?? []);
    input.value = '';

    for (const file of files) {
      const form = new FormData();
      form.append('file', file);
      this.uploading.update((n) => n + 1);
      this.http.post<{ src: string }>('/api/admin/catalog/images', form).subscribe({
        next: ({ src }) => {
          this.uploading.update((n) => n - 1);
          const alt = file.name.replace(/\.[^.]+$/, '').replace(/[-_]+/g, ' ');
          this.images.update((images) => [...images, { src, alt }]);
        },
        error: (error) => {
          this.uploading.update((n) => n - 1);
          this.errors.set([`${file.name}: ${errorMessage(error)}`]);
        },
      });
    }
  }

  protected setAlt(index: number, alt: string): void {
    this.images.update((images) => images.map((img, i) => (i === index ? { ...img, alt } : img)));
  }

  protected moveImage(index: number, by: number): void {
    this.images.update((images) => {
      const copy = [...images];
      const [item] = copy.splice(index, 1);
      copy.splice(index + by, 0, item);
      return copy;
    });
  }

  protected removeImage(index: number): void {
    this.images.update((images) => images.filter((_, i) => i !== index));
  }

  protected save(event: Event): void {
    event.preventDefault();
    if (this.saving()) return;

    const body = {
      countryId: this.country() ?? 0,
      name: this.name(),
      description: this.description(),
      climate: this.climate() || null,
      latitude: this.latitude(),
      longitude: this.longitude(),
      images: this.images(),
      hotels: this.isNew()
        ? this.draftHotels().map((h) => ({
            name: h.name,
            typeId: h.typeId,
            stars: h.stars,
            rooms: h.rooms.map((r) => ({
              roomTypeId: r.roomTypeId,
              numberOfRooms: r.numberOfRooms,
              ac: r.ac,
              balcony: r.balcony,
              pricePerNight: r.pricePerNight,
            })),
          }))
        : [],
      sights: this.isNew()
        ? this.draftSights().map((s) => ({
            name: s.name,
            categoryId: s.categoryId,
            description: s.description,
          }))
        : [],
    };

    this.saving.set(true);
    this.errors.set([]);
    this.notice.set(null);

    if (this.isNew()) {
      this.http.post<{ id: number }>('/api/admin/catalog/destinations', body).subscribe({
        next: ({ id }) => {
          this.saving.set(false);
          this.notice.set(
            `${body.name} is created with ${body.hotels.length} ${body.hotels.length === 1 ? 'hotel' : 'hotels'} and ${body.sights.length} ${body.sights.length === 1 ? 'sight' : 'sights'}.`,
          );
          this.draftHotels.set([]);
          this.draftSights.set([]);
          this.editingHotel.set(null);
          this.editingSight.set(null);
          this.router.navigate(['/admin/destinations', id], { replaceUrl: true });
        },
        error: (error) => {
          this.saving.set(false);
          this.errors.set(errorList(error));
        },
      });
    } else {
      this.http.put(`/api/admin/catalog/destinations/${this.id()}`, body).subscribe({
        next: () => {
          this.saving.set(false);
          this.notice.set('Changes saved.');
          this.destination.reload();
        },
        error: (error) => {
          this.saving.set(false);
          this.errors.set(errorList(error));
        },
      });
    }
  }

  protected numberOrNull(value: string): number | null {
    return value === '' ? null : Number(value);
  }

  protected setLocation(position: { lat: number; lng: number }): void {
    this.latitude.set(position.lat);
    this.longitude.set(position.lng);
  }

  protected setDestinationActive(active: boolean): void {
    this.post(
      `/api/admin/catalog/destinations/${this.id()}/${active ? 'show' : 'hide'}`,
      active ? 'The destination is visible again.' : 'The destination is hidden from the site.',
    );
  }

  protected setHotelActive(hotelId: number, active: boolean): void {
    this.post(
      `/api/admin/catalog/hotels/${hotelId}/${active ? 'show' : 'hide'}`,
      active
        ? 'The hotel is offered again.'
        : 'The hotel is hidden and no longer offered for new trips.',
    );
  }

  private post(url: string, done: string): void {
    this.saving.set(true);
    this.errors.set([]);
    this.http.post(url, null).subscribe({
      next: () => {
        this.saving.set(false);
        this.notice.set(done);
        this.destination.reload();
      },
      error: (error) => {
        this.saving.set(false);
        this.notice.set(null);
        this.errors.set(errorList(error));
        window.scrollTo({ top: 0, behavior: 'smooth' });
      },
    });
  }

  protected saveDraftHotel(hotel: HotelForEdit): void {
    if (hotel.id === 0) {
      this.draftHotels.update((hotels) => [...hotels, { ...hotel, id: this.nextDraftId-- }]);
    } else {
      this.draftHotels.update((hotels) => hotels.map((h) => (h.id === hotel.id ? hotel : h)));
    }
    this.editingHotel.set(null);
  }

  protected removeDraftHotel(hotel: HotelForEdit): void {
    this.draftHotels.update((hotels) => hotels.filter((h) => h.id !== hotel.id));
  }

  protected hotelSaved(): void {
    this.editingHotel.set(null);
    this.notice.set('Hotel saved.');
    this.destination.reload();
  }

  protected newSight(): void {
    this.editingSight.set(0);
    this.sightName.set('');
    this.sightCategory.set(null);
    this.sightDescription.set('');
    this.sightErrors.set([]);
  }

  protected editSight(sight: SightForEdit): void {
    this.editingSight.set(sight.id);
    this.sightName.set(sight.name);
    this.sightCategory.set(sight.categoryId);
    this.sightDescription.set(sight.description);
    this.sightErrors.set([]);
  }

  protected saveSight(event: Event): void {
    event.preventDefault();
    const id = this.editingSight();
    if (id === null) return;

    const body = {
      name: this.sightName(),
      categoryId: this.sightCategory() ?? 0,
      description: this.sightDescription(),
    };

    if (this.isNew()) {
      const problems = [
        ...(body.name.trim() ? [] : ['Enter the name of the sight.']),
        ...(body.categoryId ? [] : ['Choose a category.']),
        ...(body.description.trim() ? [] : ['Describe the sight.']),
      ];
      this.sightErrors.set(problems);
      if (problems.length) return;

      const sight: SightForEdit = {
        id: id || this.nextDraftId--,
        name: body.name.trim(),
        categoryId: body.categoryId,
        description: body.description.trim(),
        trips: 0,
      };
      this.draftSights.update((sights) =>
        id ? sights.map((s) => (s.id === id ? sight : s)) : [...sights, sight],
      );
      this.editingSight.set(null);
      return;
    }

    const request = id
      ? this.http.put(`/api/admin/catalog/sights/${id}`, body)
      : this.http.post(`/api/admin/catalog/destinations/${this.id()}/sights`, body);

    request.subscribe({
      next: () => {
        this.editingSight.set(null);
        this.destination.reload();
      },
      error: (error) => this.sightErrors.set(errorList(error)),
    });
  }

  protected deleteSight(sight: SightForEdit): void {
    if (this.isNew()) {
      this.draftSights.update((sights) => sights.filter((s) => s.id !== sight.id));
      return;
    }
    if (!confirm(`Delete ${sight.name}?`)) return;
    this.http.delete(`/api/admin/catalog/sights/${sight.id}`).subscribe({
      next: () => this.destination.reload(),
      error: (error) => this.sightErrors.set([errorMessage(error)]),
    });
  }
}
