import { Injectable, signal } from '@angular/core';
import { TripDetail } from './models';

export interface ViewedTrip {
  id: number;
  name: string;
  destination: string;
  image: string | null;
  startDate: string;
  endDate: string;
}

const STORAGE_KEY = 'flex-travel.recent-trips';
const KEEP = 6;

@Injectable({ providedIn: 'root' })
export class RecentlyViewedService {
  readonly trips = signal<ViewedTrip[]>(read());

  add(trip: TripDetail): void {
    const entry: ViewedTrip = {
      id: trip.id,
      name: trip.name,
      destination: trip.destination.name,
      image: trip.destination.images[0]?.src ?? null,
      startDate: trip.startDate,
      endDate: trip.endDate,
    };
    const today = new Date().toISOString().slice(0, 10);
    this.trips.update((trips) =>
      [entry, ...trips.filter((t) => t.id !== trip.id)]
        .filter((t) => t.startDate.slice(0, 10) > today)
        .slice(0, KEEP),
    );
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(this.trips()));
    } catch {}
  }
}

function read(): ViewedTrip[] {
  try {
    const today = new Date().toISOString().slice(0, 10);
    const trips = JSON.parse(localStorage.getItem(STORAGE_KEY) ?? '[]') as ViewedTrip[];
    return Array.isArray(trips) ? trips.filter((t) => t.startDate?.slice(0, 10) > today) : [];
  } catch {
    return [];
  }
}
