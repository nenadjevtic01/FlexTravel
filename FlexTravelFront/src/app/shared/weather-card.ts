import { httpResource } from '@angular/common/http';
import { Component, computed, input } from '@angular/core';
import { Weather } from '../core/models';

const dayFormat = new Intl.DateTimeFormat('en-GB', {
  weekday: 'short',
  day: 'numeric',
  month: 'short',
});

export function weatherIcon(code: number | null): string {
  if (code === null) return '·';
  if (code === 0) return '☀️';
  if (code <= 2) return '🌤️';
  if (code === 3) return '☁️';
  if (code === 45 || code === 48) return '🌫️';
  if (code >= 51 && code <= 57) return '🌦️';
  if ((code >= 61 && code <= 67) || (code >= 80 && code <= 82)) return '🌧️';
  if ((code >= 71 && code <= 77) || code === 85 || code === 86) return '❄️';
  if (code >= 95) return '⛈️';
  return '🌥️';
}

@Component({
  selector: 'app-weather-card',
  templateUrl: './weather-card.html',
  styleUrl: './weather-card.scss',
})
export class WeatherCard {
  readonly destinationId = input.required<number>();
  readonly from = input<string | null>(null);
  readonly to = input<string | null>(null);

  protected readonly weather = httpResource<Weather>(() => {
    const params: Record<string, string> = {};
    if (this.from()) params['from'] = this.from()!.slice(0, 10);
    if (this.to()) params['to'] = this.to()!.slice(0, 10);
    return { url: `/api/destinations/${this.destinationId()}/weather`, params };
  });

  protected readonly heading = computed(() => {
    const kind = this.weather.hasValue() ? this.weather.value().kind : null;
    if (!this.from()) return 'Weather this week';
    return kind === 'Forecast' ? 'Forecast for your dates' : 'Weather on these dates last year';
  });

  protected icon = weatherIcon;

  protected day(date: string): string {
    const [y, m, d] = date.slice(0, 10).split('-').map(Number);
    return dayFormat.format(new Date(y, m - 1, d));
  }

  protected round(value: number | null): string {
    return value === null ? '–' : Math.round(value).toString();
  }
}
