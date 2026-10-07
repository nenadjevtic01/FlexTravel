import { Component, computed, input } from '@angular/core';
import { RatingBreakdown } from '../core/models';

@Component({
  selector: 'app-rating-breakdown',
  templateUrl: './rating-breakdown.html',
  styleUrl: './rating-breakdown.scss',
})
export class RatingBreakdownView {
  readonly rating = input<RatingBreakdown | null>(null);

  private readonly max = computed(() => Math.max(1, ...(this.rating()?.stars ?? [1])));

  protected share(count: number): number {
    return (count / this.max()) * 100;
  }

  protected starsText(average: number | null): string {
    const full = Math.round(average ?? 0);
    return '★'.repeat(full) + '☆'.repeat(5 - full);
  }
}
