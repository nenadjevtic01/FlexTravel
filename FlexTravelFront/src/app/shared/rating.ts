import { Component, computed, input, output } from '@angular/core';

@Component({
  selector: 'app-rating',
  templateUrl: './rating.html',
  styleUrl: './rating.scss',
})
export class RatingBadge {
  readonly average = input<number | null>(null);
  readonly count = input(0);
  readonly compact = input(false);
  readonly light = input(false);

  protected readonly label = computed(
    () => `Rated ${this.average()?.toFixed(1)} out of 5 by ${this.count()} travelers`,
  );
}

@Component({
  selector: 'app-star-input',
  templateUrl: './star-input.html',
  styleUrl: './star-input.scss',
})
export class StarInput {
  readonly value = input(0);
  readonly label = input('Rating');
  readonly valueChange = output<number>();
}
