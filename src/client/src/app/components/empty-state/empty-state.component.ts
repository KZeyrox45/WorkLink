import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="empty-state">
      <div class="empty-icon">{{ icon }}</div>
      <p class="empty-message">{{ message }}</p>
      <a *ngIf="actionLabel && actionRoute" [routerLink]="actionRoute" class="empty-action">{{ actionLabel }}</a>
    </div>
  `,
  styles: [`
    .empty-state {
      text-align: center;
      padding: 3rem 1rem;
    }
    .empty-icon {
      font-size: 3rem;
      margin-bottom: 0.5rem;
    }
    .empty-message {
      color: #666;
      font-size: 1rem;
      margin: 0 0 1rem;
    }
    .empty-action {
      display: inline-block;
      padding: 0.5rem 1.2rem;
      background: #1976d2;
      color: white;
      text-decoration: none;
      border-radius: 4px;
      font-size: 0.9rem;
    }
  `]
})
export class EmptyStateComponent {
  @Input() icon = '📭';
  @Input() message = 'Nothing here yet.';
  @Input() actionLabel?: string;
  @Input() actionRoute?: string;
}
