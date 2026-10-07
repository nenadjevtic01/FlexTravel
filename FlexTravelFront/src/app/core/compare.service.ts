import { computed, Injectable, signal } from '@angular/core';

const STORAGE_KEY = 'flex-travel.compare';
export const MAX_COMPARED = 3;

@Injectable({ providedIn: 'root' })
export class CompareService {
  readonly ids = signal<number[]>(read());
  readonly count = computed(() => this.ids().length);
  readonly isFull = computed(() => this.ids().length >= MAX_COMPARED);

  has(id: number): boolean {
    return this.ids().includes(id);
  }

  toggle(id: number): boolean {
    if (this.has(id)) {
      this.save(this.ids().filter((x) => x !== id));
      return true;
    }
    if (this.isFull()) return false;
    this.save([...this.ids(), id]);
    return true;
  }

  remove(id: number): void {
    this.save(this.ids().filter((x) => x !== id));
  }

  clear(): void {
    this.save([]);
  }

  private save(ids: number[]): void {
    this.ids.set(ids);
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(ids));
    } catch {}
  }
}

function read(): number[] {
  try {
    const ids = JSON.parse(localStorage.getItem(STORAGE_KEY) ?? '[]');
    return Array.isArray(ids) ? ids.filter((x) => Number.isInteger(x)).slice(0, MAX_COMPARED) : [];
  } catch {
    return [];
  }
}
