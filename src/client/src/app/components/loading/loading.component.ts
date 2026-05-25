import { Component, Input } from '@angular/core';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-loading',
  standalone: true,
  imports: [NgIf],
  template: `
    <ng-container *ngIf="isLoading; else content">
      <div class="spinner-wrapper">
        <div class="spinner"></div>
        <p *ngIf="message" class="spinner-message">{{ message }}</p>
      </div>
    </ng-container>
    <ng-template #content>
      <ng-content />
    </ng-template>
  `,
  styles: [`
    .spinner-wrapper {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 3rem;
    }
    .spinner {
      width: 36px;
      height: 36px;
      border: 4px solid #e0e0e0;
      border-top: 4px solid #1976d2;
      border-radius: 50%;
      animation: spin 0.8s linear infinite;
    }
    .spinner-message {
      margin-top: 0.8rem;
      color: #666;
      font-size: 0.9rem;
    }
    @keyframes spin {
      to { transform: rotate(360deg); }
    }
  `]
})
export class LoadingComponent {
  @Input() isLoading = false;
  @Input() message = '';
}
