import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

export interface Toast {
  id: number;
  message: string;
  type: 'success' | 'error' | 'info' | 'warning';
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  private toastSubject = new Subject<Toast>();
  toasts$ = this.toastSubject.asObservable();
  private nextId = 0;

  show(message: string, type: 'success' | 'error' | 'info' | 'warning' = 'info', duration = 3000) {
    const toast: Toast = { id: this.nextId++, message, type };
    this.toastSubject.next(toast);
    setTimeout(() => this.toastSubject.next({ ...toast, type: 'info', message: '' }), duration);
  }
}
