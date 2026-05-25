import { Component, OnInit, OnDestroy } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgIf, NgFor, NgForOf, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged, takeUntil } from 'rxjs';
import { JobService, JobResponse, CategoryDto, JobListResponse } from '../../services/job.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-job-list',
  standalone: true,
  imports: [RouterLink, NgIf, NgFor, NgForOf, CurrencyPipe, FormsModule],
  templateUrl: './job-list.component.html',
  styles: [`
    .jobs-page { max-width: 900px; margin: 40px auto; padding: 2rem; }
    .header { display: flex; justify-content: space-between; align-items: center; }
    .filters { display: flex; gap: 0.8rem; flex-wrap: wrap; margin: 1.5rem 0; }
    .filters input, .filters select { padding: 0.5rem; border: 1px solid #ccc; border-radius: 4px; }
    .filters .search-wrap { flex: 1; min-width: 180px; position: relative; }
    .filters .search-wrap input { width: 100%; }
    .filters .search-wrap .clear-btn { position: absolute; right: 6px; top: 50%; transform: translateY(-50%); background: none; border: none; color: #999; cursor: pointer; padding: 0 4px; font-size: 1.1rem; }
    .job-card { border: 1px solid #e0e0e0; border-radius: 8px; padding: 1.2rem; margin-bottom: 1rem; }
    .job-card:hover { border-color: #1976d2; }
    .job-title { font-size: 1.15rem; font-weight: 600; color: #1565c0; }
    .job-meta { display: flex; gap: 1rem; font-size: 0.85rem; color: #666; margin-top: 0.4rem; flex-wrap: wrap; }
    .budget { font-weight: 600; color: #2e7d32; }
    .status-badge { padding: 0.15rem 0.5rem; border-radius: 12px; font-size: 0.8rem; }
    .status-Open { background: #e8f5e9; color: #2e7d32; }
    .status-InProgress { background: #fff3e0; color: #e65100; }
    .status-Completed { background: #e3f2fd; color: #1565c0; }
    .status-Cancelled { background: #fbe9e7; color: #c62828; }
    .skill-tag { display: inline-block; padding: 0.15rem 0.5rem; background: #f5f5f5; border-radius: 12px; font-size: 0.8rem; margin: 0.2rem 0.2rem 0 0; }
    .pagination { display: flex; justify-content: center; gap: 0.5rem; margin-top: 1.5rem; }
    .pagination button { padding: 0.4rem 0.8rem; border: 1px solid #ddd; background: white; border-radius: 4px; cursor: pointer; }
    .pagination button.active { background: #1976d2; color: white; border-color: #1976d2; }
    .pagination button:disabled { opacity: 0.4; cursor: default; }
    .no-jobs { text-align: center; color: #999; margin-top: 2rem; }
    .create-btn { padding: 0.6rem 1.2rem; background: #1976d2; color: white; border: none; border-radius: 4px; cursor: pointer; }
    a { text-decoration: none; }
  `]
})
export class JobListComponent implements OnInit, OnDestroy {
  jobs: JobResponse[] = [];
  categories: CategoryDto[] = [];
  total = 0;
  page = 1;
  pageSize = 10;
  totalPages = 0;
  keyword = '';
  selectedCategory = '';
  selectedStatus = '';
  sortBy = 'newest';
  sortOrder = 'desc';
  userRole: string | null = null;

  private searchSubject = new Subject<string>();
  private destroy$ = new Subject<void>();

  constructor(
    private jobService: JobService,
    private auth: AuthService
  ) {}

  ngOnInit() {
    this.userRole = this.auth.getUserInfo()?.role ?? null;
    this.loadCategories();
    this.loadJobs();

    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(() => {
      this.page = 1;
      this.loadJobs();
    });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onSearchInput(value: string) {
    this.keyword = value;
    this.searchSubject.next(value);
  }

  clearSearch() {
    this.keyword = '';
    this.onSearchInput('');
  }

  private loadCategories() {
    this.jobService.getCategories().subscribe({
      next: (cats) => (this.categories = cats),
    });
  }

  loadJobs() {
    const params: any = {
      page: this.page,
      pageSize: this.pageSize,
      sortBy: this.sortBy,
      sortOrder: this.sortOrder,
    };
    if (this.keyword.trim()) params.keyword = this.keyword.trim();
    if (this.selectedCategory) params.categoryId = Number(this.selectedCategory);
    if (this.selectedStatus) params.status = this.selectedStatus;
    this.jobService.listJobs(params).subscribe({
      next: (res: JobListResponse) => {
        this.jobs = res.items;
        this.total = res.total;
        this.totalPages = res.totalPages;
      },
    });
  }

  onFilterChange() {
    this.page = 1;
    this.loadJobs();
  }

  onSortChange() {
    if (this.sortBy === 'budget') {
      this.sortOrder = 'asc';
    } else {
      this.sortOrder = 'desc';
    }
    this.page = 1;
    this.loadJobs();
  }

  goToPage(p: number) {
    if (p < 1 || p > this.totalPages) return;
    this.page = p;
    this.loadJobs();
  }

  pages(): number[] {
    const p: number[] = [];
    for (let i = 1; i <= this.totalPages; i++) p.push(i);
    return p;
  }
}
