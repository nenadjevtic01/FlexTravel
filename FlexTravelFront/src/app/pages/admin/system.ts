import { HttpClient, httpResource } from '@angular/common/http';
import { Component, inject, linkedSignal, signal } from '@angular/core';
import { CurrencyService } from '../../core/currency.service';
import { errorMessage, formatDateTime, timeAgo } from '../../core/format';
import { ExchangeRate, ScheduledJobRun, ScheduledJobsOverview } from '../../core/models';

@Component({
  selector: 'app-admin-system',
  templateUrl: './system.html',
  styleUrl: './system.scss',
})
export class AdminSystem {
  private readonly http = inject(HttpClient);
  private readonly currency = inject(CurrencyService);

  protected readonly jobs = httpResource<ScheduledJobsOverview>(() => '/api/admin/jobs');
  protected readonly rates = httpResource<ExchangeRate[]>(() => '/api/exchange-rates');

  protected readonly running = signal(false);
  protected readonly lastRun = signal<ScheduledJobRun | null>(null);
  protected readonly jobError = signal<string | null>(null);

  protected readonly rateValue = linkedSignal<ExchangeRate[] | undefined, Record<string, number>>({
    source: () => this.rates.value(),
    computation: () => ({}),
  });
  protected readonly savingRate = signal(false);
  protected readonly rateMessage = signal<string | null>(null);
  protected readonly rateFailed = signal(false);

  protected readonly formatDateTime = formatDateTime;
  protected readonly timeAgo = timeAgo;

  protected run(): void {
    this.running.set(true);
    this.jobError.set(null);
    this.lastRun.set(null);
    this.http.post<ScheduledJobRun>('/api/admin/jobs/run', {}).subscribe({
      next: (run) => {
        this.running.set(false);
        this.lastRun.set(run);
        this.jobs.reload();
      },
      error: (error) => {
        this.running.set(false);
        this.jobError.set(errorMessage(error));
      },
    });
  }

  protected describe(r: ScheduledJobRun): string {
    if (r.error) return `failed (${r.error})`;
    const parts = [
      [r.expiredReservations, 'reservation expired', 'reservations expired'],
      [r.paymentReminders, 'payment reminder', 'payment reminders'],
      [r.reviewInvitations, 'review invitation', 'review invitations'],
      [r.priceAlerts, 'price alert', 'price alerts'],
      [r.closedWaitlists, 'waitlist closed', 'waitlists closed'],
    ] as const;
    const done = parts
      .filter(([n]) => n > 0)
      .map(([n, one, many]) => `${n} ${n === 1 ? one : many}`);
    return done.length ? done.join(', ') : 'nothing to do';
  }

  protected setRate(currency: string, value: string): void {
    this.rateValue.update((values) => ({ ...values, [currency]: Number(value) }));
  }

  protected saveRate(event: Event, rate: ExchangeRate): void {
    event.preventDefault();
    const ratePerEuro = this.rateValue()[rate.currency] ?? rate.ratePerEuro;
    this.savingRate.set(true);
    this.rateMessage.set(null);
    this.http.put(`/api/admin/exchange-rates/${rate.currency}`, { ratePerEuro }).subscribe({
      next: () => {
        this.savingRate.set(false);
        this.rateFailed.set(false);
        this.rateMessage.set(`1 EUR = ${ratePerEuro} ${rate.currency} saved.`);
        this.rates.reload();
        this.currency.load();
      },
      error: (error) => {
        this.savingRate.set(false);
        this.rateFailed.set(true);
        this.rateMessage.set(errorMessage(error));
      },
    });
  }
}
