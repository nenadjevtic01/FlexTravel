import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { formatDateRange, formatMoney, imageUrl, offerLabel, transportIcon } from '../core/format';
import { TripListItem } from '../core/models';
import { CompareButton } from './compare-button';
import { RatingBadge } from './rating';

@Component({
  selector: 'app-trip-card',
  imports: [RouterLink, RatingBadge, CompareButton],
  templateUrl: './trip-card.html',
  styleUrl: './trip-card.scss',
})
export class TripCard {
  readonly trip = input.required<TripListItem>();

  protected readonly imageUrl = imageUrl;
  protected readonly formatMoney = formatMoney;
  protected readonly formatDateRange = formatDateRange;
  protected readonly transportIcon = transportIcon;
  protected readonly offerLabel = offerLabel;
}
