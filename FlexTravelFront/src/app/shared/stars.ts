import { Component, computed, input } from '@angular/core';

@Component({
  selector: 'app-stars',
  templateUrl: './stars.html',
  styleUrl: './stars.scss',
})
export class Stars {
  readonly value = input.required<number>();
  protected readonly slots = computed(() =>
    Array.from({ length: 5 }, (_, i) => i < Math.round(this.value())),
  );
}
