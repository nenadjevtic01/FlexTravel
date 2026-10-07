import { HttpClient } from '@angular/common/http';
import { computed, effect, inject, Injectable, signal, untracked } from '@angular/core';
import { AuthService } from './auth.service';
import { FavoriteDestination } from './models';
import { NotificationsService } from './notifications.service';

@Injectable({ providedIn: 'root' })
export class FavoritesService {
  private readonly http = inject(HttpClient);
  private readonly auth = inject(AuthService);

  readonly items = signal<FavoriteDestination[]>([]);
  readonly loaded = signal(false);
  private readonly ids = computed(() => new Set(this.items().map((f) => f.id)));

  constructor() {
    effect(() => {
      const enabled = this.auth.canFavorite();
      this.auth.token();
      untracked(() => (enabled ? this.load() : this.clear()));
    });

    const notifications = inject(NotificationsService);
    effect(() => {
      if (notifications.latest()?.type === 'NewTripToFavorite') {
        untracked(() => this.load());
      }
    });
  }

  isFavorite(destinationId: number): boolean {
    return this.ids().has(destinationId);
  }

  load(): void {
    this.http.get<FavoriteDestination[]>('/api/favorites').subscribe({
      next: (items) => {
        this.items.set(items);
        this.loaded.set(true);
      },
      error: () => this.loaded.set(true),
    });
  }

  toggle(destinationId: number, name: string): void {
    const favorite = !this.isFavorite(destinationId);
    const previous = this.items();

    this.items.set(
      favorite
        ? [
            ...previous,
            {
              id: destinationId,
              name,
              country: '',
              image: null,
              upcomingTrips: 0,
              nextDeparture: null,
            },
          ]
        : previous.filter((f) => f.id !== destinationId),
    );

    const request = favorite
      ? this.http.put(`/api/favorites/${destinationId}`, null)
      : this.http.delete(`/api/favorites/${destinationId}`);

    request.subscribe({
      next: () => this.load(),
      error: () => this.items.set(previous),
    });
  }

  private clear(): void {
    this.items.set([]);
    this.loaded.set(false);
  }
}
