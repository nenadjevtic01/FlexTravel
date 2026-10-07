import { Component, computed, input, signal } from '@angular/core';
import { imageUrl } from '../core/format';

@Component({
  selector: 'app-avatar',
  templateUrl: './avatar.html',
  styleUrl: './avatar.scss',
})
export class Avatar {
  readonly src = input<string | null | undefined>(null);
  readonly name = input('');
  readonly size = input(32);

  protected readonly failed = signal(false);
  protected readonly url = computed(() => imageUrl(this.src()));
  protected readonly initial = computed(() => this.name().trim().charAt(0).toUpperCase() || '?');
  protected readonly alt = computed(() =>
    this.name() ? `Photo of ${this.name()}` : 'Profile photo',
  );
}
