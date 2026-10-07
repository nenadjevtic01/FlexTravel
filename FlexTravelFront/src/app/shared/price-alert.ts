import { HttpClient, httpResource } from '@angular/common/http';
import { Component, computed, inject, input, linkedSignal, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../core/auth.service';
import { errorMessage, formatEuro, formatMoney } from '../core/format';
import { PriceAlert } from '../core/models';

@Component({
  selector: 'app-price-alert',
  imports: [RouterLink],
  templateUrl: './price-alert.html',
  styleUrl: './price-alert.scss',
})
export class PriceAlertControl {
  private readonly http = inject(HttpClient);
  protected readonly auth = inject(AuthService);

  readonly destinationId = input.required<number>();
  readonly destinationName = input.required<string>();

  private readonly alerts = httpResource<PriceAlert[]>(() =>
    this.auth.canSetPriceAlerts() ? '/api/price-alerts' : undefined,
  );

  protected readonly alert = computed(() =>
    this.alerts.hasValue()
      ? this.alerts.value().find((a) => a.destinationId === this.destinationId())
      : undefined,
  );

  protected readonly limit = linkedSignal<number | null>(() => this.alert()?.maxPrice ?? null);
  protected readonly busy = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly formatMoney = formatMoney;
  protected readonly formatEuro = formatEuro;

  protected toNumber(value: string): number | null {
    return value === '' ? null : Number(value);
  }

  protected save(event: Event): void {
    event.preventDefault();
    this.send(
      this.http.put(`/api/price-alerts/${this.destinationId()}`, { maxPrice: this.limit() }),
    );
  }

  protected remove(): void {
    this.send(this.http.delete(`/api/price-alerts/${this.destinationId()}`));
  }

  private send(request: ReturnType<HttpClient['put']>): void {
    this.busy.set(true);
    this.error.set(null);
    request.subscribe({
      next: () => {
        this.busy.set(false);
        this.alerts.reload();
      },
      error: (error) => {
        this.busy.set(false);
        this.error.set(errorMessage(error));
      },
    });
  }
}
