import { HttpClient, HttpErrorResponse, httpResource } from '@angular/common/http';
import { Component, effect, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { errorMessage, formatDate } from '../../core/format';
import { Profile } from '../../core/models';
import { Avatar } from '../../shared/avatar';

const PHOTO_SIZE = 320;
const MAX_UPLOAD = 5 * 1024 * 1024;

async function squarePhoto(file: File): Promise<Blob> {
  const bitmap = await createImageBitmap(file);
  const side = Math.min(bitmap.width, bitmap.height);
  const canvas = document.createElement('canvas');
  canvas.width = canvas.height = PHOTO_SIZE;
  const context = canvas.getContext('2d')!;
  context.drawImage(
    bitmap,
    (bitmap.width - side) / 2,
    (bitmap.height - side) / 2,
    side,
    side,
    0,
    0,
    PHOTO_SIZE,
    PHOTO_SIZE,
  );
  bitmap.close();
  return new Promise((resolve, reject) =>
    canvas.toBlob(
      (blob) => (blob ? resolve(blob) : reject(new Error('empty'))),
      'image/jpeg',
      0.88,
    ),
  );
}

function errorList(error: unknown): string[] {
  return error instanceof HttpErrorResponse && Array.isArray(error.error?.errors)
    ? (error.error.errors as { error: string }[]).map((e) => e.error)
    : [errorMessage(error)];
}

@Component({
  selector: 'app-profile',
  imports: [RouterLink, Avatar],
  templateUrl: './profile.html',
  styleUrl: './profile.scss',
})
export class ProfilePage {
  private readonly http = inject(HttpClient);
  protected readonly auth = inject(AuthService);

  protected readonly profile = httpResource<Profile>(() =>
    this.auth.hasProfile() ? '/api/profile' : undefined,
  );

  protected readonly firstName = signal('');
  protected readonly lastName = signal('');
  protected readonly phone = signal('');
  protected readonly address = signal('');
  protected readonly city = signal('');
  protected readonly currentPassword = signal('');
  protected readonly newPassword = signal('');
  protected readonly repeatPassword = signal('');

  protected readonly saving = signal(false);
  protected readonly profileErrors = signal<string[]>([]);
  protected readonly passwordErrors = signal<string[]>([]);
  protected readonly profileSaved = signal(false);
  protected readonly passwordSaved = signal(false);

  protected readonly formatDate = formatDate;
  protected readonly photoBusy = signal(false);
  protected readonly photoError = signal<string | null>(null);

  constructor() {
    effect(() => {
      if (!this.profile.hasValue()) return;
      const p = this.profile.value();
      this.firstName.set(p.firstName);
      this.lastName.set(p.lastName);
      this.phone.set(p.phone);
      this.address.set(p.address);
      this.city.set(p.city);
    });
  }

  protected roleLabel(role: Profile['role']): string {
    return role === 'AgencyEmployee' ? 'Agency employee' : role;
  }

  protected save(event: Event): void {
    event.preventDefault();
    this.saving.set(true);
    this.profileErrors.set([]);
    this.profileSaved.set(false);

    this.http
      .put<{ token: string }>('/api/profile', {
        firstName: this.firstName(),
        lastName: this.lastName(),
        phone: this.phone(),
        address: this.address(),
        city: this.city(),
      })
      .subscribe({
        next: ({ token }) => {
          this.saving.set(false);
          this.profileSaved.set(true);

          this.auth.replaceToken(token);
        },
        error: (error) => {
          this.saving.set(false);
          this.profileErrors.set(errorList(error));
        },
      });
  }

  protected async uploadPhoto(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    input.value = '';
    if (!file) return;

    this.photoError.set(null);
    if (file.size > MAX_UPLOAD) {
      this.photoError.set('The picture can be at most 5 MB.');
      return;
    }

    this.photoBusy.set(true);
    let photo: Blob;
    try {
      photo = await squarePhoto(file);
    } catch {
      this.photoBusy.set(false);
      this.photoError.set(
        'This file could not be read as a picture. Choose a JPEG, PNG or WebP image.',
      );
      return;
    }

    const form = new FormData();
    form.append('file', photo, 'avatar.jpg');
    this.http.post<{ avatar: string; token: string }>('/api/profile/avatar', form).subscribe({
      next: ({ token }) => this.photoSaved(token),
      error: (error) => this.photoFailed(error),
    });
  }

  protected removePhoto(): void {
    this.photoBusy.set(true);
    this.photoError.set(null);
    this.http.delete<{ token: string }>('/api/profile/avatar').subscribe({
      next: ({ token }) => this.photoSaved(token),
      error: (error) => this.photoFailed(error),
    });
  }

  private photoSaved(token: string): void {
    this.photoBusy.set(false);
    this.auth.replaceToken(token);
    this.profile.reload();
  }

  private photoFailed(error: unknown): void {
    this.photoBusy.set(false);
    this.photoError.set(errorList(error).join(' '));
  }

  protected changePassword(event: Event): void {
    event.preventDefault();
    this.passwordSaved.set(false);

    if (this.newPassword() !== this.repeatPassword()) {
      this.passwordErrors.set(['The new passwords are not the same.']);
      return;
    }

    this.saving.set(true);
    this.passwordErrors.set([]);
    this.http
      .post('/api/profile/password', {
        currentPassword: this.currentPassword(),
        newPassword: this.newPassword(),

        keepRefreshToken: this.auth.currentRefreshToken(),
      })
      .subscribe({
        next: () => {
          this.saving.set(false);
          this.passwordSaved.set(true);
          this.currentPassword.set('');
          this.newPassword.set('');
          this.repeatPassword.set('');
        },
        error: (error) => {
          this.saving.set(false);
          this.passwordErrors.set(errorList(error));
        },
      });
  }
}
