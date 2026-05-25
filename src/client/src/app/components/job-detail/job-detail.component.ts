import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NgIf, NgFor, DatePipe, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { JobService, JobResponse } from '../../services/job.service';
import { ProposalService, ProposalResponse } from '../../services/proposal.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-job-detail',
  standalone: true,
  imports: [RouterLink, NgIf, NgFor, DatePipe, CurrencyPipe, FormsModule],
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
    .status-Pending { background: #fff8e1; color: #f57f17; }
    .status-Accepted { background: #e8f5e9; color: #2e7d32; }
    .status-Rejected { background: #fbe9e7; color: #c62828; }
    .status-Withdrawn { background: #f5f5f5; color: #757575; }
    .description { margin: 1.5rem 0; line-height: 1.6; white-space: pre-wrap; }
    .skill-tag { display: inline-block; padding: 0.2rem 0.6rem; background: #f5f5f5; border-radius: 12px; font-size: 0.85rem; margin: 0.2rem 0.2rem 0 0; }
    .section { margin-top: 2rem; padding: 1.2rem; background: #f9f9f9; border-radius: 8px; }
    .section h3 { margin: 0 0 1rem; }
    .form-group { margin-bottom: 1rem; }
    .form-group label { display: block; margin-bottom: 0.3rem; font-weight: 500; }
    .form-group input, .form-group textarea { width: 100%; padding: 0.5rem; border: 1px solid #ccc; border-radius: 4px; }
    .form-group textarea { resize: vertical; min-height: 80px; }
    .form-row { display: flex; gap: 1rem; }
    .form-row .form-group { flex: 1; }
    .error { color: #d32f2f; margin-bottom: 0.5rem; }
    .actions { margin-top: 2rem; }
    button { padding: 0.5rem 1.2rem; border: none; border-radius: 4px; cursor: pointer; margin-right: 0.5rem; }
    button:disabled { opacity: 0.6; }
    .edit-btn { background: #1976d2; color: white; }
    .delete-btn { background: #d32f2f; color: white; }
    .back-btn { background: #757575; color: white; }
    .submit-btn { background: #2e7d32; color: white; }
    .accept-btn { background: #2e7d32; color: white; }
    .reject-btn { background: #d32f2f; color: white; }
    .client-info { padding: 1rem; background: #f9f9f9; border-radius: 8px; }
    .client-info a { color: #1565c0; text-decoration: none; font-weight: 500; }
    .proposal-card { border: 1px solid #e0e0e0; border-radius: 8px; padding: 1rem; margin-bottom: 0.8rem; background: white; }
    .proposal-card .header { display: flex; justify-content: space-between; align-items: flex-start; }
    .proposal-card .name { font-weight: 600; }
    .proposal-card .meta { font-size: 0.85rem; color: #666; }
    .proposal-card .actions { margin-top: 0.8rem; }
    .proposal-card .cover-letter { margin-top: 0.5rem; font-size: 0.9rem; white-space: pre-wrap; }
  `]
})
export class JobDetailComponent implements OnInit {
  job: JobResponse | null = null;
  currentUserId: string | null = null;
  userRole: string | null = null;
  error = '';

  proposals: ProposalResponse[] = [];
  myProposal: ProposalResponse | null = null;

  showSubmitForm = false;
  coverLetter = '';
  bidAmount: number | null = null;
  estimatedDays: number | null = null;
  submitting = false;
  submitError = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private jobService: JobService,
    private proposalService: ProposalService,
    private auth: AuthService
  ) {}

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!id) { this.error = 'Job not found.'; return; }

    const userInfo = this.auth.getUserInfo();
    this.userRole = userInfo?.role ?? null;

    this.jobService.getJob(id).subscribe({
      next: (job) => {
        this.job = job;
        try {
          const payload = JSON.parse(atob(this.auth.getToken()!.split('.')[1]));
          this.currentUserId = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
        } catch {}
        this.loadProposals();
      },
      error: () => this.error = 'Job not found.',
    });
  }

  private loadProposals() {
    if (!this.job) return;
    this.proposalService.listByJob(this.job.id).subscribe({
      next: (proposals) => {
        this.proposals = proposals;
        if (this.currentUserId) {
          this.myProposal = proposals.find(p => p.freelancerId === this.currentUserId) ?? null;
        }
      },
    });
  }

  get isOwner(): boolean {
    return !!this.job && !!this.currentUserId && this.job.clientId === this.currentUserId;
  }

  get canSubmit(): boolean {
    return this.userRole === 'Freelancer'
      && this.job?.status === 'Open'
      && !this.myProposal
      && !!this.currentUserId;
  }

  onSubmitProposal() {
    if (!this.job || !this.bidAmount || !this.estimatedDays) return;
    this.submitError = '';
    this.submitting = true;
    this.proposalService.submit(this.job.id, {
      coverLetter: this.coverLetter,
      bidAmount: this.bidAmount,
      estimatedDays: this.estimatedDays,
    }).subscribe({
      next: () => {
        this.showSubmitForm = false;
        this.submitting = false;
        this.loadProposals();
      },
      error: (err) => {
        this.submitError = err.error?.message || 'Failed to submit proposal';
        this.submitting = false;
      },
    });
  }

  onAccept(proposalId: number) {
    if (!confirm('Accept this proposal? This will reject all other pending proposals.')) return;
    this.proposalService.accept(proposalId).subscribe({
      next: () => {
        this.loadProposals();
        if (this.job) this.job.status = 'InProgress';
      },
      error: () => alert('Failed to accept proposal.'),
    });
  }

  onReject(proposalId: number) {
    if (!confirm('Reject this proposal?')) return;
    this.proposalService.reject(proposalId).subscribe({
      next: () => this.loadProposals(),
      error: () => alert('Failed to reject proposal.'),
    });
  }

  onWithdraw(proposalId: number) {
    if (!confirm('Withdraw this proposal?')) return;
    this.proposalService.withdraw(proposalId).subscribe({
      next: () => this.loadProposals(),
      error: () => alert('Failed to withdraw proposal.'),
    });
  }

  onDelete() {
    if (!this.job || !confirm('Delete this job?')) return;
    this.jobService.deleteJob(this.job.id).subscribe({
      next: () => this.router.navigate(['/jobs']),
      error: () => alert('Failed to delete job.'),
    });
  }
}
