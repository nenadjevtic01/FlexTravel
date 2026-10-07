import { Component, inject, input } from '@angular/core';
import { AuthService } from '../core/auth.service';
import { FavoritesService } from '../core/favorites.service';

@Component({
  selector: 'app-favorite-button',
  templateUrl: './favorite-button.html',
  styleUrl: './favorite-button.scss',
})
export class FavoriteButton {
  protected readonly auth = inject(AuthService);
  protected readonly favorites = inject(FavoritesService);

  readonly destinationId = input.required<number>();
  readonly destinationName = input('');
  readonly withLabel = input(false);

  protected toggle(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    this.favorites.toggle(this.destinationId(), this.destinationName());
  }
}
