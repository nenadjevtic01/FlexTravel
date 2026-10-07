import { HttpClient, httpResource } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { errorMessage, formatEuro, formatMoney, imageUrl, timeAgo } from '../../core/format';
import { PriceAlert } from '../../core/models';

@Component({
  selector: 'app-price-alerts',
  imports: [RouterLink],
  templateUrl: './price-alerts.html',
  styleUrl: './price-alerts.scss',
})
export class PriceAlerts {
  private readonly http = inject(HttpClient);

  protected readonly alerts = httpResource<PriceAlert[]>(() => '/api/price-alerts');
  protected readonly busy = signal<number | null>(null);
  protected readonly error = signal<string | null>(null);

  protected readonly formatMoney = formatMoney;
  protected readonly formatEuro = formatEuro;
  protected readonly imageUrl = imageUrl;
  protected readonly timeAgo = timeAgo;

  protected remove(alert: PriceAlert): void {
    this.busy.set(alert.id);
    this.error.set(null);
    this.http.delete(`/api/price-alerts/${alert.destinationId}`).subscribe({
      next: () => {
        this.busy.set(null);
        this.alerts.reload();
      },
      error: (error) => {
        this.busy.set(null);
        this.error.set(errorMessage(error));
      },
    });
  }
}
