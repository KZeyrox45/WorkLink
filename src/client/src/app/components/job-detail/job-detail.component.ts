import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NgIf, NgFor, DatePipe, CurrencyPipe } from '@angular/common';
import { JobService, JobResponse } from '../../services/job.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-job-detail',
  standalone: true,
  imports: [RouterLink, NgIf, NgFor, DatePipe, CurrencyPipe],
  templateUrl: './job-detail.component.html',
  styles: [`
    .detail-page { max-width: 800px; margin: 40px auto; padding: 2rem; }
    .header { display: flex; justify-content: space-between; align-items: flex-start; }
    .meta { display: flex; gap: 1rem; font-size: 0.9rem; color: #666; margin: 0.5rem 0; flex-wrap: wrap; }
    .budget { font-weight: 600; color: #2e7d32; font-size: 1.1rem; }
    .status-badge { padding: 0.2rem 0.6rem; border-radius: 12px; font-size: 0.85rem; }
    .status-Open { background: #e8f5e9; color: #2e7d32; }
    .status-InProgress { background: #fff3e0; color: #e65100; }
    .status-Completed { background: #e3f2fd; color: #1565c0; }
    .status-Cancelled { background: #fbe9e7; color: #c62828; }
    .description { margin: 1.5rem 0; line-height: 1.6; white-space: pre-wrap; }
    .skill-tag { display: inline-block; padding: 0.2rem 0.6rem; background: #f5f5f5; border-radius: 12px; font-size: 0.85rem; margin: 0.2rem 0.2rem 0 0; }
    .actions { margin-top: 2rem; }
    button { padding: 0.5rem 1.2rem; border: none; border-radius: 4px; cursor: pointer; margin-right: 0.5rem; }
    .edit-btn { background: #1976d2; color: white; }
    .delete-btn { background: #d32f2f; color: white; }
    .back-btn { background: #757575; color: white; }
    .client-info { margin-top: 1rem; padding: 1rem; background: #f9f9f9; border-radius: 8px; }
    .client-info a { color: #1565c0; text-decoration: none; font-weight: 500; }
  `]
})
export class JobDetailComponent implements OnInit {
  job: JobResponse | null = null;
  currentUserId: string | null = null;
  error = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private jobService: JobService,
    private auth: AuthService
  ) {}

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!id) { this.error = 'Job not found.'; return; }
    this.currentUserId = this.auth.getUserInfo() ? 'from_token' : null;
    this.jobService.getJob(id).subscribe({
      next: (job) => {
        this.job = job;
        try {
          const payload = JSON.parse(atob(this.auth.getToken()!.split('.')[1]));
          this.currentUserId = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
        } catch {}
      },
      error: () => this.error = 'Job not found.',
    });
  }

  get isOwner(): boolean {
    return !!this.job && !!this.currentUserId && this.job.clientId === this.currentUserId;
  }

  onDelete() {
    if (!this.job || !confirm('Delete this job?')) return;
    this.jobService.deleteJob(this.job.id).subscribe({
      next: () => this.router.navigate(['/jobs']),
      error: () => alert('Failed to delete job.'),
    });
  }
}
