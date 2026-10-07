import { HttpClient } from '@angular/common/http';
import { computed, effect, inject, Injectable, signal } from '@angular/core';
import { AuthService } from './auth.service';
import { displayCurrency, DisplayCurrency } from './format';
import { ExchangeRate } from './models';

const STORAGE_KEY = 'flex-travel.currency';

@Injectable({ providedIn: 'root' })
export class CurrencyService {
  private readonly http = inject(HttpClient);
  private readonly auth = inject(AuthService);

  readonly rates = signal<ExchangeRate[]>([]);
  readonly chosen = signal<string>(readChoice());
  readonly canSwitch = computed(
    () => !this.auth.isAgency() && !this.auth.isAdmin() && this.rates().length > 0,
  );
  readonly codes = computed(() => ['EUR', ...this.rates().map((r) => r.currency)]);

  readonly current = computed<DisplayCurrency>(() => {
    const rate = this.rates().find((r) => r.currency === this.chosen());
    return this.canSwitch() && rate
      ? { code: rate.currency, rate: rate.ratePerEuro }
      : { code: 'EUR', rate: 1 };
  });
  readonly isConverted = computed(() => this.current().code !== 'EUR');

  constructor() {
    effect(() => displayCurrency.set(this.current()));
    this.load();
  }

  load(): void {
    this.http.get<ExchangeRate[]>('/api/exchange-rates').subscribe({
      next: (rates) => this.rates.set(rates),
      error: () => this.rates.set([]),
    });
  }

  choose(code: string): void {
    this.chosen.set(code);
    try {
      localStorage.setItem(STORAGE_KEY, code);
    } catch {}
  }
}

function readChoice(): string {
  try {
    return localStorage.getItem(STORAGE_KEY) ?? 'EUR';
  } catch {
    return 'EUR';
  }
}
