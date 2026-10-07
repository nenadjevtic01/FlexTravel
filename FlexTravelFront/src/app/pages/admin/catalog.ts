import { HttpClient, HttpErrorResponse, httpResource } from '@angular/common/http';
import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { errorMessage, formatNumber } from '../../core/format';
import {
  CatalogOptions,
  Country,
  DestinationListItem,
  PagedResponse,
  SaveCountry,
} from '../../core/models';
import { SafetyMeter } from '../../shared/safety-meter';

function emptyCountry(continentId: number | null): SaveCountry {
  return {
    name: '',
    continentId,
    capitalCity: '',
    currency: '',
    language: '',
    population: null,
    totalArea: null,
    visaType: '',
    visaDuration: '',
    visaRequirements: '',
    safetyLevel: null,
    safetyAdvices: '',
  };
}

@Component({
  selector: 'app-catalog',
  imports: [RouterLink, SafetyMeter],
  templateUrl: './catalog.html',
  styleUrl: './catalog.scss',
})
export class Catalog {
  private readonly http = inject(HttpClient);

  protected readonly options = httpResource<CatalogOptions>(() => '/api/admin/catalog/options');
  protected readonly countries = httpResource<Country[]>(() => '/api/countries');

  protected readonly continentId = signal<number | null>(null);
  protected readonly search = signal('');
  protected readonly selectedId = signal<number | null>(null);

  protected readonly editingId = signal<number | null>(null);
  protected readonly form = signal<SaveCountry>(emptyCountry(null));
  protected readonly saving = signal(false);
  protected readonly errors = signal<string[]>([]);
  protected readonly notice = signal<string | null>(null);

  protected readonly visibleCountries = computed(() => {
    if (!this.countries.hasValue()) return [];
    const term = this.search().trim().toLowerCase();
    return this.countries
      .value()
      .filter((c) => this.continentId() === null || c.continentId === this.continentId())
      .filter((c) => !term || c.name.toLowerCase().includes(term));
  });

  protected readonly selected = computed(() =>
    this.countries.hasValue()
      ? this.countries.value().find((c) => c.id === this.selectedId())
      : undefined,
  );

  protected readonly destinations = httpResource<PagedResponse<DestinationListItem>>(() => {
    const id = this.selectedId();
    return id
      ? { url: '/api/destinations', params: { countryId: id, perPage: 100, includeHidden: true } }
      : undefined;
  });

  protected readonly formatNumber = formatNumber;
  protected readonly cleanupResult = signal<string | null>(null);

  protected cleanUpImages(): void {
    this.cleanupResult.set(null);
    this.http.post<{ deleted: number }>('/api/admin/catalog/images/cleanup', null).subscribe({
      next: ({ deleted }) =>
        this.cleanupResult.set(
          deleted
            ? `${deleted} unused ${deleted === 1 ? 'image was' : 'images were'} deleted.`
            : 'No unused images.',
        ),
      error: (error) => this.cleanupResult.set(errorMessage(error)),
    });
  }

  protected select(country: Country): void {
    this.selectedId.set(country.id);
    this.editingId.set(null);
    this.notice.set(null);
  }

  protected newCountry(): void {
    this.form.set(emptyCountry(this.continentId()));
    this.editingId.set(0);
    this.selectedId.set(null);
    this.errors.set([]);
    this.notice.set(null);
  }

  protected edit(country: Country): void {
    this.form.set({
      name: country.name,
      continentId: country.continentId,
      capitalCity: country.capitalCity ?? '',
      currency: country.currency ?? '',
      language: country.language ?? '',
      population: country.population,
      totalArea: country.totalArea,
      visaType: country.visa?.type ?? '',
      visaDuration: country.visa?.duration ?? '',
      visaRequirements: country.visa?.requirements ?? '',
      safetyLevel: country.safety?.level ?? null,
      safetyAdvices: country.safety?.advices ?? '',
    });
    this.editingId.set(country.id);
    this.selectedId.set(country.id);
    this.errors.set([]);
    this.notice.set(null);
  }

  protected set<K extends keyof SaveCountry>(key: K, value: SaveCountry[K]): void {
    this.form.update((f) => ({ ...f, [key]: value }));
  }

  protected numberOrNull(value: string): number | null {
    return value === '' ? null : Number(value);
  }

  protected save(event: Event): void {
    event.preventDefault();
    const id = this.editingId();
    if (id === null || this.saving()) return;

    this.saving.set(true);
    this.errors.set([]);
    const request = id
      ? this.http.put(`/api/admin/catalog/countries/${id}`, this.form())
      : this.http.post<{ id: number }>('/api/admin/catalog/countries', this.form());

    request.subscribe({
      next: (result) => {
        this.saving.set(false);
        const savedId = id || (result as { id: number }).id;
        this.notice.set(`${this.form().name} saved.`);
        this.editingId.set(null);
        this.selectedId.set(savedId);
        this.countries.reload();
      },
      error: (error) => {
        this.saving.set(false);
        this.errors.set(
          error instanceof HttpErrorResponse && Array.isArray(error.error?.errors)
            ? (error.error.errors as { error: string }[]).map((e) => e.error)
            : [errorMessage(error)],
        );
      },
    });
  }
}
