import { Component, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { FavoritesService } from '../../core/favorites.service';
import { formatDate, imageUrl } from '../../core/format';
import { FavoriteButton } from '../../shared/favorite-button';
import { PriceAlerts } from './price-alerts';

@Component({
  selector: 'app-favorites',
  imports: [RouterLink, FavoriteButton, PriceAlerts],
  templateUrl: './favorites.html',
  styleUrl: './favorites.scss',
})
export class Favorites implements OnInit {
  protected readonly auth = inject(AuthService);
  protected readonly favorites = inject(FavoritesService);

  protected readonly imageUrl = imageUrl;
  protected readonly formatDate = formatDate;

  ngOnInit(): void {
    if (this.auth.canFavorite()) {
      this.favorites.load();
    }
  }
}
