import { Component, computed, inject, input } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { TripForm } from './trip-form';

@Component({
  selector: 'app-trip-edit',
  imports: [RouterLink, TripForm],
  templateUrl: './trip-edit.html',
  styleUrl: './trip-edit.scss',
})
export class TripEdit {
  private readonly router = inject(Router);

  readonly id = input<string>();
  protected readonly tripId = computed(() => (this.id() ? Number(this.id()) : null));

  protected onSaved(id: number): void {
    this.router.navigate(['/agency'], {
      queryParams: { tab: 'trips', saved: id, ...(this.tripId() ? {} : { created: 1 }) },
    });
  }
}
