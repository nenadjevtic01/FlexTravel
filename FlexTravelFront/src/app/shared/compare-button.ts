import { Component, computed, inject, input } from '@angular/core';
import { CompareService, MAX_COMPARED } from '../core/compare.service';

@Component({
  selector: 'app-compare-button',
  templateUrl: './compare-button.html',
  styleUrl: './compare-button.scss',
})
export class CompareButton {
  protected readonly compare = inject(CompareService);
  readonly tripId = input.required<number>();

  protected readonly on = computed(() => this.compare.ids().includes(this.tripId()));
  protected readonly title = computed(() =>
    this.on()
      ? 'Remove from the comparison'
      : this.compare.isFull()
        ? `You can compare up to ${MAX_COMPARED} trips`
        : 'Add to the comparison',
  );

  protected toggle(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    this.compare.toggle(this.tripId());
  }
}
