import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { NgIf } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink, NgIf],
  templateUrl: './register.component.html',
  styles: [
    `
      .auth-page {
        max-width: 400px;
        margin: 80px auto;
        padding: 2rem;
      }
      .form-group {
        margin-bottom: 1rem;
      }
      .form-group label {
        display: block;
        margin-bottom: 0.3rem;
        font-weight: 500;
      }
      .form-group input,
      .form-group select {
        width: 100%;
        padding: 0.5rem;
        border: 1px solid #ccc;
        border-radius: 4px;
      }
      .error {
        color: #d32f2f;
        margin-bottom: 1rem;
      }
      button {
        width: 100%;
        padding: 0.6rem;
        background: #1976d2;
        color: white;
        border: none;
        border-radius: 4px;
        cursor: pointer;
      }
      button:disabled {
        opacity: 0.6;
      }
    `,
  ],
})
export class RegisterComponent {
  email = '';
  password = '';
  displayName = '';
  role = 'Freelancer';
  error = '';
  loading = false;

  constructor(
    private auth: AuthService,
    private router: Router
  ) {}

  onSubmit() {
    this.error = '';
    this.loading = true;
    this.auth
      .register({
        email: this.email,
        password: this.password,
        displayName: this.displayName,
        role: this.role,
      })
      .subscribe({
        next: () => this.router.navigate(['/dashboard']),
        error: (err) => {
          this.error = err.error?.message || 'Registration failed';
          this.loading = false;
        },
      });
  }
}
