import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.css',
})
export class ForgotPasswordComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);

  readonly isLoading = signal(false);
  readonly message = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
  });

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    this.message.set(null);

    this.authService.forgotPassword(this.form.getRawValue().email).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.message.set(
          'Si el correo está registrado, recibirás un enlace para restablecer tu contraseña.'
        );
      },
      error: () => {
        this.isLoading.set(false);
        this.message.set(
          'Si el correo está registrado, recibirás un enlace para restablecer tu contraseña.'
        );
      },
    });
  }
}
