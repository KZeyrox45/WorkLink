import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIf, NgFor, DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../services/auth.service';
import { LoadingComponent } from '../loading/loading.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterLink, NgIf, NgFor, DatePipe, LoadingComponent],
  templateUrl: './dashboard.component.html',
  styles: [`
    .dashboard { max-width: 900px; margin: 40px auto; padding: 2rem; }
    .header { display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 1rem; }
    .header h2 { margin: 0; }
    .role-badge { background: #e3f2fd; color: #1565c0; padding: 0.2rem 0.6rem; border-radius: 12px; font-size: 0.85rem; margin-left: 0.5rem; }
    .stats-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(140px, 1fr)); gap: 1rem; margin: 2rem 0; }
    .stat-card { background: white; border: 1px solid #e0e0e0; border-radius: 8px; padding: 1.2rem; text-align: center; }
    .stat-number { font-size: 2rem; font-weight: 700; color: #1976d2; }
    .stat-label { font-size: 0.85rem; color: #666; margin-top: 0.3rem; }
    button { padding: 0.5rem 1rem; background: #d32f2f; color: white; border: none; border-radius: 4px; cursor: pointer; }
    .actions { display: flex; gap: 0.8rem; flex-wrap: wrap; }
    .actions a { padding: 0.5rem 1.2rem; background: #1976d2; color: white; text-decoration: none; border-radius: 4px; font-size: 0.9rem; }
    .actions a.secondary { background: #f5f5f5; color: #333; }
    .activity-list { margin-top: 2rem; }
    .activity-list h3 { margin: 0 0 1rem; }
    .activity-item { padding: 0.6rem 0; border-bottom: 1px solid #f0f0f0; font-size: 0.9rem; color: #444; }
    .activity-item .date { font-size: 0.8rem; color: #999; margin-left: 0.5rem; }
    .quick-links { margin-top: 2rem; display: flex; gap: 0.8rem; flex-wrap: wrap; }
  `]
})
export class DashboardComponent implements OnInit {
  user: { email: string; name: string; role: string } | null = null;
  stats: any = null;
  loading = true;

  private apiUrl = 'http://localhost:5114/api';

  constructor(
    private auth: AuthService,
    private router: Router,
    private http: HttpClient
  ) {}

  ngOnInit() {
    this.user = this.auth.getUserInfo();
    if (this.user) {
      this.http.get(`${this.apiUrl}/dashboard/stats`).subscribe({
        next: (stats) => {
          this.stats = stats;
          this.loading = false;
        },
        error: () => this.loading = false,
      });
    }
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  get isClient(): boolean {
    return this.user?.role === 'Client';
  }
}
