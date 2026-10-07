import { Component, inject, input, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { errorMessage, httpStatus } from '../../core/format';

@Component({
  selector: 'app-login',
  imports: [RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly returnUrl = input<string>();

  protected readonly email = signal('');
  protected readonly password = signal('');
  protected readonly rememberMe = signal(false);
  protected readonly submitting = signal(false);
  protected readonly error = signal<string | null>(null);

  protected submit(event: Event): void {
    event.preventDefault();
    if (!this.email() || !this.password() || this.submitting()) {
      return;
    }

    this.submitting.set(true);
    this.error.set(null);

    this.auth.login(this.email(), this.password(), this.rememberMe()).subscribe({
      next: () => {
        this.submitting.set(false);
        this.router.navigateByUrl(this.returnUrl() || this.auth.homeUrl());
      },
      error: (error) => {
        this.submitting.set(false);
        this.error.set(
          httpStatus(error) === 401 ? 'Wrong email or password.' : errorMessage(error),
        );
      },
    });
  }
}
