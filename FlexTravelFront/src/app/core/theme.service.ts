import { computed, Injectable, signal } from '@angular/core';

export type ThemeChoice = 'system' | 'light' | 'dark';

const STORAGE_KEY = 'flex-travel.theme';
const ORDER: ThemeChoice[] = ['system', 'light', 'dark'];

@Injectable({ providedIn: 'root' })
export class ThemeService {
  readonly choice = signal<ThemeChoice>(readChoice());
  readonly label = computed(() =>
    this.choice() === 'system' ? 'Theme: same as the device' : `Theme: ${this.choice()}`,
  );

  constructor() {
    apply(this.choice());
  }

  next(): void {
    const choice = ORDER[(ORDER.indexOf(this.choice()) + 1) % ORDER.length];
    this.choice.set(choice);
    apply(choice);
    try {
      localStorage.setItem(STORAGE_KEY, choice);
    } catch {}
  }
}

function apply(choice: ThemeChoice): void {
  if (choice === 'system') {
    delete document.documentElement.dataset['theme'];
  } else {
    document.documentElement.dataset['theme'] = choice;
  }
}

function readChoice(): ThemeChoice {
  try {
    const stored = localStorage.getItem(STORAGE_KEY) as ThemeChoice | null;
    return stored && ORDER.includes(stored) ? stored : 'system';
  } catch {
    return 'system';
  }
}
