import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgIf, NgFor, DatePipe, CurrencyPipe } from '@angular/common';
import { JobService, JobResponse } from '../../services/job.service';
import { LoadingComponent } from '../loading/loading.component';
import { EmptyStateComponent } from '../empty-state/empty-state.component';

@Component({
  selector: 'app-my-jobs',
  standalone: true,
  imports: [RouterLink, NgIf, NgFor, DatePipe, CurrencyPipe, LoadingComponent, EmptyStateComponent],
  template: `
    <div class="page">
      <div class="header">
        <h2>My Jobs</h2>
        <a routerLink="/jobs/new" class="post-btn">+ Post a Job</a>
      </div>

      <app-loading [isLoading]="loading" message="Loading your jobs...">
        <app-empty-state
          *ngIf="jobs.length === 0 && !loading"
          icon="📋"
          message="You haven't posted any jobs yet."
          actionLabel="Post a Job"
          actionRoute="/jobs/new"
        />

        <div class="job-card" *ngFor="let job of jobs">
          <div class="job-card-header">
            <h3><a [routerLink]="'/jobs/' + job.id">{{ job.title }}</a></h3>
            <span class="status-badge" [class]="'status-' + job.status">{{ job.status }}</span>
          </div>
          <div class="job-meta">
            <span>{{ job.proposalCount }} proposal{{ job.proposalCount !== 1 ? 's' : '' }}</span>
            <span *ngIf="job.budgetMin || job.budgetMax">
              Budget: {{ job.budgetMin | currency }} – {{ job.budgetMax | currency }}
            </span>
            <span>{{ job.createdAt | date }}</span>
          </div>
          <div class="job-actions">
            <a [routerLink]="'/jobs/' + job.id" class="btn btn-view">View</a>
            <a *ngIf="job.status === 'Open'" [routerLink]="'/jobs/' + job.id + '/edit'" class="btn btn-edit">Edit</a>
          </div>
        </div>
      </app-loading>
    </div>
  `,
  styles: [`
    .page { max-width: 800px; margin: 40px auto; padding: 2rem; }
    .header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem; }
    .header h2 { margin: 0; }
    .post-btn {
      padding: 0.5rem 1.2rem; background: #1976d2; color: white;
      text-decoration: none; border-radius: 4px; font-size: 0.9rem;
    }
    .job-card {
      border: 1px solid #e0e0e0; border-radius: 8px; padding: 1rem 1.2rem;
      margin-bottom: 0.8rem; background: white;
    }
    .job-card-header { display: flex; justify-content: space-between; align-items: flex-start; }
    .job-card-header h3 { margin: 0 0 0.3rem; font-size: 1.05rem; }
    .job-card-header h3 a { color: #1a1a2e; text-decoration: none; }
    .job-card-header h3 a:hover { color: #1976d2; }
    .job-meta { display: flex; gap: 1rem; font-size: 0.85rem; color: #666; flex-wrap: wrap; margin-bottom: 0.8rem; }
    .status-badge { padding: 0.2rem 0.6rem; border-radius: 12px; font-size: 0.8rem; }
    .status-Open { background: #e8f5e9; color: #2e7d32; }
    .status-InProgress { background: #fff3e0; color: #e65100; }
    .status-Completed { background: #e3f2fd; color: #1565c0; }
    .status-Cancelled { background: #fbe9e7; color: #c62828; }
    .job-actions { display: flex; gap: 0.5rem; }
    .btn { padding: 0.35rem 0.8rem; border-radius: 4px; font-size: 0.85rem; text-decoration: none; }
    .btn-view { background: #f5f5f5; color: #333; }
    .btn-edit { background: #e3f2fd; color: #1565c0; }
  `]
})
export class MyJobsComponent implements OnInit {
  jobs: JobResponse[] = [];
  loading = true;

  constructor(private jobService: JobService) {}

  ngOnInit() {
    this.jobService.listJobs({ pageSize: 50 }).subscribe({
      next: (res) => {
        this.jobs = res.items;
        this.loading = false;
      },
      error: () => this.loading = false,
    });
  }
}
