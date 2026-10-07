import { Component, computed, input, signal } from '@angular/core';
import { formatMoney } from '../../core/format';

interface MonthPoint {
  month: string;
  reservations: number;
  value: number;
}

const monthLabel = new Intl.DateTimeFormat('en-GB', { month: 'short' });
const monthLong = new Intl.DateTimeFormat('en-GB', { month: 'long', year: 'numeric' });

@Component({
  selector: 'app-monthly-chart',
  templateUrl: './monthly-chart.html',
  styleUrl: './monthly-chart.scss',
})
export class MonthlyChart {
  readonly data = input.required<MonthPoint[]>();
  protected readonly active = signal<number | null>(null);

  protected readonly points = computed(() => {
    const max = Math.max(1, ...this.data().map((d) => d.reservations));
    return this.data().map((d) => {
      const [year, month] = d.month.split('-').map(Number);
      const date = new Date(year, month - 1, 1);
      return {
        ...d,
        height: (d.reservations / max) * 85,
        short: monthLabel.format(date),
        long: monthLong.format(date),
        money: formatMoney(d.value),
      };
    });
  });

  protected readonly summary = computed(
    () =>
      'Reservations per month: ' +
      this.points()
        .map((p) => `${p.long} ${p.reservations}`)
        .join(', '),
  );
}
