import { Component, computed, input } from '@angular/core';

@Component({
  selector: 'app-safety-meter',
  templateUrl: './safety-meter.html',
  styleUrl: './safety-meter.scss',
})
export class SafetyMeter {
  readonly level = input.required<number>();
  readonly showLabel = input(false);

  protected readonly segments = computed(() =>
    Array.from({ length: 5 }, (_, i) => i < this.level()),
  );
  protected readonly tone = computed(() =>
    this.level() >= 4 ? 'meter high' : this.level() === 3 ? 'meter mid' : 'meter low',
  );
  protected readonly label = computed(() =>
    this.level() >= 4 ? 'Safe' : this.level() === 3 ? 'Caution' : 'Higher risk',
  );
}
