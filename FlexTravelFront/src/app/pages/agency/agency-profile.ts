import { HttpClient, HttpErrorResponse, httpResource } from '@angular/common/http';
import { Component, effect, inject, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { errorMessage, imageUrl } from '../../core/format';
import { AgencyPage } from '../../core/models';

@Component({
  selector: 'app-agency-profile',
  imports: [RouterLink],
  templateUrl: './agency-profile.html',
  styleUrl: './agency-profile.scss',
})
export class AgencyProfileEditor {
  private readonly http = inject(HttpClient);
  readonly agencyId = input.required<number>();

  protected readonly page = httpResource<AgencyPage>(() => `/api/agencies/${this.agencyId()}`);

  protected readonly description = signal('');
  protected readonly address = signal('');
  protected readonly phone = signal('');
  protected readonly email = signal('');
  protected readonly website = signal('');
  protected readonly logo = signal('');

  protected readonly saving = signal(false);
  protected readonly uploading = signal(false);
  protected readonly saved = signal(false);
  protected readonly errors = signal<string[]>([]);

  protected readonly imageUrl = imageUrl;

  constructor() {
    effect(() => {
      if (!this.page.hasValue()) return;
      const p = this.page.value();
      this.description.set(p.description ?? '');
      this.address.set(p.address);
      this.phone.set(p.phone);
      this.email.set(p.email ?? '');
      this.website.set(p.website ?? '');
      this.logo.set(p.logo ?? '');
    });
  }

  protected upload(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    input.value = '';
    if (!file) return;

    const form = new FormData();
    form.append('file', file);
    this.uploading.set(true);
    this.errors.set([]);
    this.http.post<{ src: string }>('/api/agency/logo', form).subscribe({
      next: ({ src }) => {
        this.uploading.set(false);
        this.logo.set(src);
        this.saved.set(false);
      },
      error: (error) => {
        this.uploading.set(false);
        this.errors.set([errorMessage(error)]);
      },
    });
  }

  protected save(event: Event): void {
    event.preventDefault();
    this.saving.set(true);
    this.saved.set(false);
    this.errors.set([]);

    this.http
      .put('/api/agency/profile', {
        description: this.description(),
        address: this.address(),
        phone: this.phone(),
        email: this.email(),
        website: this.website(),
        logo: this.logo() || null,
      })
      .subscribe({
        next: () => {
          this.saving.set(false);
          this.saved.set(true);
          this.page.reload();
        },
        error: (error) => {
          this.saving.set(false);
          this.errors.set(
            error instanceof HttpErrorResponse && Array.isArray(error.error?.errors)
              ? (error.error.errors as { error: string }[]).map((e) => e.error)
              : [errorMessage(error)],
          );
        },
      });
  }
}
