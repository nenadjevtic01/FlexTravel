import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, input, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService, RegisterRequest } from '../../core/auth.service';
import { errorMessage } from '../../core/format';

type Field = keyof RegisterRequest;

@Component({
  selector: 'app-register',
  imports: [RouterLink],
  templateUrl: './register.html',
  styleUrl: '../login/login.scss',
})
export class Register {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly returnUrl = input<string>();

  protected readonly form = signal<RegisterRequest>({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    phone: '',
    address: '',
    city: '',
  });
  protected readonly submitting = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly fieldErrors = signal<Partial<Record<Field, string>>>({});

  protected set(field: Field, value: string): void {
    this.form.update((form) => ({ ...form, [field]: value }));
  }

  protected submit(event: Event): void {
    event.preventDefault();
    if (this.submitting()) return;

    this.submitting.set(true);
    this.error.set(null);
    this.fieldErrors.set({});

    this.auth.register(this.form()).subscribe({
      next: () => {
        this.submitting.set(false);
        this.router.navigateByUrl(this.returnUrl() || '/trips');
      },
      error: (error) => {
        this.submitting.set(false);

        if (
          error instanceof HttpErrorResponse &&
          error.status === 422 &&
          Array.isArray(error.error?.errors)
        ) {
          const errors: Partial<Record<Field, string>> = {};
          for (const e of error.error.errors as { property: string; error: string }[]) {
            const field = (e.property.charAt(0).toLowerCase() + e.property.slice(1)) as Field;
            errors[field] ??= e.error;
          }
          this.fieldErrors.set(errors);
        } else {
          this.error.set(errorMessage(error));
        }
      },
    });
  }
}
