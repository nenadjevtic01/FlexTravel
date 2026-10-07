import { HttpErrorResponse } from '@angular/common/http';
import { signal } from '@angular/core';

export interface DisplayCurrency {
  code: string;
  rate: number;
}

export const displayCurrency = signal<DisplayCurrency>({ code: 'EUR', rate: 1 });

const numberFormat = new Intl.NumberFormat('en-GB');
const dateTimeFormat = new Intl.DateTimeFormat('en-GB', {
  day: '2-digit',
  month: 'short',
  year: 'numeric',
  hour: '2-digit',
  minute: '2-digit',
  second: '2-digit',
});

export function formatNumber(value: number | null | undefined): string {
  return value === null || value === undefined ? '—' : numberFormat.format(value);
}

export function formatDateTime(value: string): string {
  const iso = /[zZ]|[+-]\d\d:\d\d$/.test(value) ? value : value + 'Z';
  return dateTimeFormat.format(new Date(iso));
}

const moneyFormat = new Intl.NumberFormat('en-GB', {
  style: 'currency',
  currency: 'EUR',
  minimumFractionDigits: 0,
  maximumFractionDigits: 2,
});
const dateFormat = new Intl.DateTimeFormat('en-GB', {
  day: 'numeric',
  month: 'short',
  year: 'numeric',
});
const dayMonthFormat = new Intl.DateTimeFormat('en-GB', { day: 'numeric', month: 'short' });

const moneyWithCentsFormat = new Intl.NumberFormat('en-GB', {
  style: 'currency',
  currency: 'EUR',
  minimumFractionDigits: 2,
  maximumFractionDigits: 2,
});

const otherFormats = new Map<string, Intl.NumberFormat>();

export function formatMoney(value: number | null | undefined): string {
  if (value === null || value === undefined) return '—';

  const { code, rate } = displayCurrency();
  if (code === 'EUR') return formatEuro(value);

  let format = otherFormats.get(code);
  if (!format) {
    format = new Intl.NumberFormat('en-GB', {
      style: 'currency',
      currency: code,
      maximumFractionDigits: 0,
    });
    otherFormats.set(code, format);
  }
  return format.format(Math.round(value * rate));
}

export function formatEuro(value: number | null | undefined): string {
  if (value === null || value === undefined) return '—';
  return Number.isInteger(value) ? moneyFormat.format(value) : moneyWithCentsFormat.format(value);
}

function toDate(value: string): Date {
  const [year, month, day] = value.slice(0, 10).split('-').map(Number);
  return new Date(year, month - 1, day);
}

export function formatDate(value: string): string {
  return dateFormat.format(toDate(value));
}

export function formatDateRange(start: string, end: string): string {
  const from = toDate(start);
  const to = toDate(end);
  return from.getFullYear() === to.getFullYear()
    ? `${dayMonthFormat.format(from)} – ${dateFormat.format(to)}`
    : `${dateFormat.format(from)} – ${dateFormat.format(to)}`;
}

export function formatTravelers(adults: number, children: number): string {
  const a = `${adults} ${adults === 1 ? 'adult' : 'adults'}`;
  return children ? `${a}, ${children} ${children === 1 ? 'child' : 'children'}` : a;
}

export function transportIcon(transport: string): string {
  switch (transport) {
    case 'Plane':
      return '✈️';
    case 'Bus':
      return '🚌';
    case 'Train':
      return '🚆';
    case 'Ship':
      return '🚢';
    default:
      return '🧭';
  }
}

export function offerLabel(offer: string | null | undefined, percent: number): string {
  if (!offer || !percent) return '';
  return `${offer === 'EarlyBooking' ? 'Early booking' : 'Last minute'} −${percent}%`;
}

export function extraIcon(kind: string): string {
  switch (kind) {
    case 'Insurance':
      return '🛡️';
    case 'Transfer':
      return '🚐';
    case 'Excursion':
      return '🧭';
    default:
      return '✨';
  }
}

export function mealsText(day: { breakfast: boolean; lunch: boolean; dinner: boolean }): string {
  return [day.breakfast && 'Breakfast', day.lunch && 'Lunch', day.dinner && 'Dinner']
    .filter(Boolean)
    .join(' · ');
}

export function formatRating(value: number | null | undefined): string {
  return value === null || value === undefined ? '' : value.toFixed(1);
}

export function timeAgo(value: string, now = Date.now()): string {
  const iso = /[zZ]|[+-]\d\d:\d\d$/.test(value) ? value : value + 'Z';
  const seconds = Math.max(0, Math.round((now - new Date(iso).getTime()) / 1000));
  if (seconds < 60) return 'just now';
  const minutes = Math.round(seconds / 60);
  if (minutes < 60) return `${minutes} min ago`;
  const hours = Math.round(minutes / 60);
  if (hours < 24) return `${hours} ${hours === 1 ? 'hour' : 'hours'} ago`;
  const days = Math.round(hours / 24);
  if (days === 1) return 'yesterday';
  if (days < 7) return `${days} days ago`;
  return dateFormat.format(new Date(iso));
}

export function imageUrl(src: string | null | undefined): string {
  if (!src) return '';
  return src.startsWith('uploads/') ? `/${src}` : `/images/${src}`;
}

export function errorMessage(error: unknown): string {
  if (error instanceof HttpErrorResponse) {
    if (error.status === 0) {
      return 'The backend is not reachable. Check that the API is running.';
    }
    return error.error?.message ?? `Error ${error.status}`;
  }
  return 'Something went wrong.';
}

export function httpStatus(error: unknown): number | null {
  return error instanceof HttpErrorResponse ? error.status : null;
}
