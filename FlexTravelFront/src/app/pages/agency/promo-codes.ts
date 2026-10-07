import { HttpClient, httpResource } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { errorMessage, formatDate, formatMoney } from '../../core/format';
import { PromoCode } from '../../core/models';

function isoDate(daysFromToday: number): string {
  const date = new Date();
  date.setDate(date.getDate() + daysFromToday);
  return date.toISOString().slice(0, 10);
}

@Component({
  selector: 'app-promo-codes',
  templateUrl: './promo-codes.html',
  styleUrl: './promo-codes.scss',
})
export class PromoCodes {
  private readonly http = inject(HttpClient);

  protected readonly codes = httpResource<PromoCode[]>(() => '/api/agency/promo-codes');

  protected readonly code = signal('');
  protected readonly description = signal('');
  protected readonly percent = signal(10);
  protected readonly maxUses = signal<number | null>(null);
  protected readonly validFrom = signal(isoDate(0));
  protected readonly validTo = signal(isoDate(30));
  protected readonly saving = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly created = signal<string | null>(null);
  protected readonly busyId = signal<number | null>(null);

  protected readonly formatDate = formatDate;
  protected readonly formatMoney = formatMoney;

  protected setMaxUses(value: string): void {
    this.maxUses.set(value ? Number(value) : null);
  }

  protected create(): void {
    this.saving.set(true);
    this.error.set(null);
    this.created.set(null);

    this.http
      .post('/api/agency/promo-codes', {
        code: this.code(),
        description: this.description() || null,
        discountPercent: this.percent(),
        validFrom: this.validFrom(),
        validTo: this.validTo(),
        maxUses: this.maxUses(),
      })
      .subscribe({
        next: () => {
          this.saving.set(false);
          this.created.set(this.code().trim().toUpperCase());
          this.code.set('');
          this.description.set('');
          this.codes.reload();
        },
        error: (e) => {
          this.saving.set(false);
          this.error.set(errorMessage(e));
        },
      });
  }

  protected setActive(code: PromoCode, active: boolean): void {
    this.busyId.set(code.id);
    this.http
      .post(`/api/agency/promo-codes/${code.id}/${active ? 'activate' : 'deactivate'}`, null)
      .subscribe({
        next: () => {
          this.busyId.set(null);
          this.codes.reload();
        },
        error: (e) => {
          this.busyId.set(null);
          this.error.set(errorMessage(e));
        },
      });
  }
}
