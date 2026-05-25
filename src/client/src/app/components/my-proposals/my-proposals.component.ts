import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgIf, NgFor, DatePipe, CurrencyPipe } from '@angular/common';
import { ProposalService, ProposalResponse } from '../../services/proposal.service';
import { ToastService } from '../../services/toast.service';
import { LoadingComponent } from '../loading/loading.component';
import { EmptyStateComponent } from '../empty-state/empty-state.component';

@Component({
  selector: 'app-my-proposals',
  standalone: true,
  imports: [RouterLink, NgIf, NgFor, DatePipe, CurrencyPipe, LoadingComponent, EmptyStateComponent],
  template: `
    <div class="page">
      <h2>My Proposals</h2>

      <app-loading [isLoading]="loading" message="Loading your proposals...">
        <app-empty-state
          *ngIf="proposals.length === 0 && !loading"
          icon="📝"
          message="You haven't submitted any proposals yet."
          actionLabel="Browse Jobs"
          actionRoute="/jobs"
        />

        <div class="proposal-card" *ngFor="let prop of proposals">
          <div class="card-header">
            <h3><a [routerLink]="'/jobs/' + prop.jobId">{{ prop.jobTitle }}</a></h3>
            <span class="status-badge" [class]="'status-' + prop.status">{{ prop.status }}</span>
          </div>
          <div class="card-meta">
            <span>Bid: {{ prop.bidAmount | currency }}</span>
            <span>{{ prop.estimatedDays }} days</span>
            <span>Client: {{ prop.clientName }}</span>
            <span>{{ prop.createdAt | date }}</span>
          </div>
          <p class="cover-letter" *ngIf="prop.coverLetter">{{ prop.coverLetter }}</p>
          <div class="card-actions">
            <a [routerLink]="'/jobs/' + prop.jobId" class="btn btn-view">View Job</a>
            <button *ngIf="prop.status === 'Pending'" class="btn btn-withdraw" (click)="onWithdraw(prop.id)">Withdraw</button>
          </div>
        </div>
      </app-loading>
    </div>
  `,
  styles: [`
    .page { max-width: 800px; margin: 40px auto; padding: 2rem; }
    h2 { margin: 0 0 1.5rem; }
    .proposal-card {
      border: 1px solid #e0e0e0; border-radius: 8px; padding: 1rem 1.2rem;
      margin-bottom: 0.8rem; background: white;
    }
    .card-header { display: flex; justify-content: space-between; align-items: flex-start; }
    .card-header h3 { margin: 0 0 0.3rem; font-size: 1.05rem; }
    .card-header h3 a { color: #1a1a2e; text-decoration: none; }
    .card-header h3 a:hover { color: #1976d2; }
    .card-meta { display: flex; gap: 1rem; font-size: 0.85rem; color: #666; flex-wrap: wrap; margin-bottom: 0.5rem; }
    .cover-letter { font-size: 0.9rem; color: #444; white-space: pre-wrap; margin: 0.5rem 0; }
    .status-badge { padding: 0.2rem 0.6rem; border-radius: 12px; font-size: 0.8rem; }
    .status-Pending { background: #fff8e1; color: #f57f17; }
    .status-Accepted { background: #e8f5e9; color: #2e7d32; }
    .status-Rejected { background: #fbe9e7; color: #c62828; }
    .status-Withdrawn { background: #f5f5f5; color: #757575; }
    .card-actions { display: flex; gap: 0.5rem; margin-top: 0.5rem; }
    .btn { padding: 0.35rem 0.8rem; border-radius: 4px; font-size: 0.85rem; text-decoration: none; border: none; cursor: pointer; }
    .btn-view { background: #f5f5f5; color: #333; }
    .btn-withdraw { background: #fff3e0; color: #e65100; }
  `]
})
export class MyProposalsComponent implements OnInit {
  proposals: ProposalResponse[] = [];
  loading = true;

  constructor(
    private proposalService: ProposalService,
    private toast: ToastService
  ) {}

  ngOnInit() {
    this.proposalService.listMine().subscribe({
      next: (props) => {
        this.proposals = props;
        this.loading = false;
      },
      error: () => this.loading = false,
    });
  }

  onWithdraw(proposalId: number) {
    this.proposalService.withdraw(proposalId).subscribe({
      next: () => {
        this.toast.show('Proposal withdrawn', 'success');
        this.ngOnInit();
      },
      error: () => this.toast.show('Failed to withdraw proposal', 'error'),
    });
  }
}
