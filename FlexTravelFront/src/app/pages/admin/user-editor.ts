import { HttpClient, HttpErrorResponse, httpResource } from '@angular/common/http';
import { Component, computed, effect, inject, input, output, signal } from '@angular/core';
import { AuthService } from '../../core/auth.service';
import { errorMessage, formatDate, formatDateTime } from '../../core/format';
import { UserDetail } from '../../core/models';
import { Avatar } from '../../shared/avatar';

interface AccountForm {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  address: string;
  city: string;
}

const ROLE_LABELS: Record<string, string> = {
  Customer: 'Customer',
  AgencyEmployee: 'Agency employee',
  Admin: 'Administrator',
};

@Component({
  selector: 'app-user-editor',
  imports: [Avatar],
  templateUrl: './user-editor.html',
  styleUrl: './user-editor.scss',
})
export class UserEditor {
  private readonly http = inject(HttpClient);
  protected readonly auth = inject(AuthService);

  readonly userId = input.required<number>();
  readonly changed = output<void>();
  readonly closed = output<void>();

  protected readonly user = httpResource<UserDetail>(() => `/api/admin/users/${this.userId()}`);
  protected readonly selected = signal<number[]>([]);
  protected readonly saving = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly savedMessage = signal(false);

  protected readonly form = signal<AccountForm>({
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    address: '',
    city: '',
  });
  protected readonly detailErrors = signal<string[]>([]);
  protected readonly detailsSaved = signal(false);
  protected readonly detailsDirty = computed(() => {
    if (!this.user.hasValue()) return false;
    const u = this.user.value();
    const f = this.form();
    return (
      f.firstName !== u.firstName ||
      f.lastName !== u.lastName ||
      f.email !== u.email ||
      f.phone !== u.phone ||
      f.address !== u.address ||
      f.city !== u.city
    );
  });

  protected readonly dirty = computed(() => {
    if (!this.user.hasValue()) return false;
    const original = [...this.user.value().useCaseIds].sort((a, b) => a - b).join(',');
    return original !== [...this.selected()].sort((a, b) => a - b).join(',');
  });

  protected readonly formatDate = formatDate;
  protected readonly formatDateTime = formatDateTime;

  constructor() {
    effect(() => {
      if (this.user.hasValue()) {
        const u = this.user.value();
        this.selected.set([...u.useCaseIds]);
        this.form.set({
          firstName: u.firstName,
          lastName: u.lastName,
          email: u.email,
          phone: u.phone,
          address: u.address,
          city: u.city,
        });
      }
    });
  }

  protected setField(key: keyof AccountForm, value: string): void {
    this.detailsSaved.set(false);
    this.form.update((f) => ({ ...f, [key]: value }));
  }

  protected saveDetails(event: Event, u: UserDetail): void {
    event.preventDefault();
    this.detailErrors.set([]);
    this.saving.set(true);
    this.http.put(`/api/admin/users/${u.id}`, this.form()).subscribe({
      next: () => {
        this.saving.set(false);
        this.detailsSaved.set(true);
        this.user.reload();
        this.changed.emit();
      },
      error: (error) => {
        this.saving.set(false);
        this.detailErrors.set(
          error instanceof HttpErrorResponse && Array.isArray(error.error?.errors)
            ? (error.error.errors as { error: string }[]).map((e) => e.error)
            : [errorMessage(error)],
        );
      },
    });
  }

  protected removeAvatar(u: UserDetail): void {
    if (!confirm(`Remove the profile photo of ${u.name}? They can upload another one.`)) return;
    this.run(this.http.delete(`/api/admin/users/${u.id}/avatar`));
  }

  protected roleLabel(role: string): string {
    return ROLE_LABELS[role] ?? role;
  }

  protected toggle(id: number, checked: boolean): void {
    this.savedMessage.set(false);
    this.selected.update((ids) => (checked ? [...ids, id] : ids.filter((x) => x !== id)));
  }

  protected resetToDefaults(u: UserDetail): void {
    this.savedMessage.set(false);
    this.selected.set([...u.defaultUseCaseIds]);
  }

  protected save(u: UserDetail): void {
    this.run(this.http.put(`/api/admin/users/${u.id}/use-cases`, this.selected()), () =>
      this.savedMessage.set(true),
    );
  }

  protected toggleActive(u: UserDetail): void {
    const action = u.isActive ? 'block' : 'unblock';
    if (u.isActive && !confirm(`Block ${u.name}? They will not be able to sign in.`)) return;
    this.run(this.http.post(`/api/admin/users/${u.id}/${action}`, null));
  }

  private run(request: ReturnType<HttpClient['post']>, done?: () => void): void {
    this.saving.set(true);
    this.error.set(null);
    request.subscribe({
      next: () => {
        this.saving.set(false);
        done?.();
        this.user.reload();
        this.changed.emit();
      },
      error: (error) => {
        this.saving.set(false);
        this.error.set(errorMessage(error));
      },
    });
  }

  protected errorText(error: unknown): string {
    return errorMessage(error);
  }
}
