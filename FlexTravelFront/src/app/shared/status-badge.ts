import { Component, computed, input } from '@angular/core';
import { ReservationStatus } from '../core/models';

@Component({
  selector: 'app-status-badge',
  templateUrl: './status-badge.html',
  styleUrl: './status-badge.scss',
})
export class StatusBadge {
  readonly status = input.required<ReservationStatus>();

  protected readonly tone = computed(() => {
    switch (this.status()) {
      case 'Confirmed':
        return 'badge badge-success';
      case 'Rejected':
        return 'badge badge-danger';
      case 'Cancelled':
      case 'Expired':
        return 'badge cancelled';
      default:
        return 'badge pending';
    }
  });

  protected readonly label = computed(() =>
    this.status() === 'Pending' ? 'Awaiting confirmation' : this.status(),
  );
}
