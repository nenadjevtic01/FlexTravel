import { httpResource } from '@angular/common/http';
import { Component, computed, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { formatDate, formatEuro } from '../../core/format';
import { AgencyReport } from '../../core/models';

const monthShort = new Intl.DateTimeFormat('en-GB', { month: 'short' });
const monthLong = new Intl.DateTimeFormat('en-GB', { month: 'long' });

function niceMax(value: number): number {
  if (value <= 0) return 1;
  const power = 10 ** Math.floor(Math.log10(value));
  return ([1, 2, 2.5, 5, 10].find((m) => m * power >= value) ?? 10) * power;
}

function csvCell(value: string | number): string {
  const text = String(value);
  return /[",\n;]/.test(text) ? `"${text.replace(/"/g, '""')}"` : text;
}

@Component({
  selector: 'app-agency-report',
  imports: [RouterLink],
  templateUrl: './report.html',
  styleUrl: './report.scss',
})
export class AgencyReportView {
  protected readonly year = signal(0);
  protected readonly showTable = signal(false);
  protected readonly active = signal<number | null>(null);

  protected readonly report = httpResource<AgencyReport>(() => ({
    url: '/api/agency/report',
    params: { year: this.year() },
  }));

  protected readonly years = computed(
    () => this.report.value()?.years ?? [new Date().getFullYear()],
  );

  protected readonly axisMax = computed(() => {
    const months = this.report.value()?.months ?? [];
    return niceMax(Math.max(0, ...months.map((m) => Math.max(m.sales, m.collected))));
  });

  protected readonly months = computed(() =>
    (this.report.value()?.months ?? []).map((m) => {
      const date = new Date(2000, m.month - 1, 1);
      return {
        ...m,
        short: monthShort.format(date),
        long: monthLong.format(date),
        salesHeight: (Math.max(0, m.sales) / this.axisMax()) * 100,
        collectedHeight: (Math.max(0, m.collected) / this.axisMax()) * 100,
      };
    }),
  );

  protected readonly maxDestination = computed(() =>
    Math.max(1, ...(this.report.value()?.destinations ?? []).map((d) => d.sales)),
  );

  protected readonly summary = computed(
    () =>
      'Sales and collected payments by month: ' +
      this.months()
        .map((m) => `${m.long} sales ${formatEuro(m.sales)}, collected ${formatEuro(m.collected)}`)
        .join('; '),
  );

  protected readonly formatDate = formatDate;

  protected euro(value: number): string {
    return formatEuro(Math.round(value));
  }

  protected share(value: number, max: number): number {
    return (value / max) * 100;
  }

  protected exportCsv(): void {
    const r = this.report.value();
    if (!r) return;

    const rows: (string | number)[][] = [
      ['Month', 'Reservations', 'Travelers', 'Sales (EUR)', 'Collected (EUR)'],
      ...this.months().map((m) => [m.long, m.reservations, m.travelers, m.sales, m.collected]),
      [],
      ['Departure', 'Date', 'Capacity', 'Seats sold', 'Occupancy %', 'Sales (EUR)'],
      ...r.departures.map((d) => [
        d.name,
        d.startDate.slice(0, 10),
        d.capacity,
        d.seatsSold,
        d.occupancy,
        d.sales,
      ]),
      [],
      ['Destination', 'Country', 'Reservations', 'Travelers', 'Sales (EUR)'],
      ...r.destinations.map((d) => [
        d.destination,
        d.country,
        d.reservations,
        d.travelers,
        d.sales,
      ]),
    ];

    const csv = '﻿' + rows.map((row) => row.map(csvCell).join(',')).join('\r\n');
    const link = document.createElement('a');
    link.href = URL.createObjectURL(new Blob([csv], { type: 'text/csv;charset=utf-8' }));
    link.download = `agency-report-${r.year}.csv`;
    link.click();
    URL.revokeObjectURL(link.href);
  }
}
