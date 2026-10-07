import { httpResource } from '@angular/common/http';
import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { httpStatus, imageUrl } from '../../core/format';
import { AgencyPage } from '../../core/models';
import { RatingBreakdownView } from '../../shared/rating-breakdown';
import { ReviewList } from '../../shared/review-list';
import { TripCard } from '../../shared/trip-card';

@Component({
  selector: 'app-agency-page',
  imports: [RouterLink, TripCard, RatingBreakdownView, ReviewList],
  templateUrl: './agency-page.html',
  styleUrl: './agency-page.scss',
})
export class AgencyPageView {
  readonly id = input.required<string>();

  protected readonly page = httpResource<AgencyPage>(() => `/api/agencies/${this.id()}`);
  protected readonly imageUrl = imageUrl;

  protected notFound(error: unknown): boolean {
    return httpStatus(error) === 404;
  }

  protected host(url: string): string {
    try {
      return new URL(url).host;
    } catch {
      return url;
    }
  }
}
