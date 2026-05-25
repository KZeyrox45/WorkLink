import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface CategoryDto {
  id: number;
  name: string;
  slug: string;
}

export interface SkillDto {
  id: number;
  name: string;
}

export interface JobResponse {
  id: number;
  title: string;
  description: string;
  budgetMin: number | null;
  budgetMax: number | null;
  durationDays: number | null;
  status: string;
  categoryId: number;
  categoryName: string;
  clientId: string;
  clientName: string;
  skills: SkillDto[];
  proposalCount: number;
  createdAt: string;
}

export interface JobListResponse {
  items: JobResponse[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface CreateJobRequest {
  title: string;
  description: string;
  budgetMin: number | null;
  budgetMax: number | null;
  durationDays: number | null;
  categoryId: number;
  skillIds: number[];
}

export interface UpdateJobRequest {
  title: string;
  description: string;
  budgetMin: number | null;
  budgetMax: number | null;
  durationDays: number | null;
  categoryId: number;
  skillIds: number[];
}

@Injectable({ providedIn: 'root' })
export class JobService {
  private apiUrl = 'http://localhost:5114/api';

  constructor(private http: HttpClient) {}

  getCategories(): Observable<CategoryDto[]> {
    return this.http.get<CategoryDto[]>(`${this.apiUrl}/categories`);
  }

  listJobs(params: {
    keyword?: string;
    categoryId?: number;
    budgetMin?: number;
    budgetMax?: number;
    skillIds?: string;
    status?: string;
    clientId?: string;
    sortBy?: string;
    sortOrder?: string;
    page?: number;
    pageSize?: number;
  }): Observable<JobListResponse> {
    const query = new URLSearchParams();
    if (params.keyword) query.set('keyword', params.keyword);
    if (params.categoryId) query.set('categoryId', params.categoryId.toString());
    if (params.budgetMin) query.set('budgetMin', params.budgetMin.toString());
    if (params.budgetMax) query.set('budgetMax', params.budgetMax.toString());
    if (params.skillIds) query.set('skillIds', params.skillIds);
    if (params.status) query.set('status', params.status);
    if (params.clientId) query.set('clientId', params.clientId);
    if (params.sortBy) query.set('sortBy', params.sortBy);
    if (params.sortOrder) query.set('sortOrder', params.sortOrder);
    if (params.page) query.set('page', params.page.toString());
    if (params.pageSize) query.set('pageSize', params.pageSize.toString());
    return this.http.get<JobListResponse>(`${this.apiUrl}/jobs?${query}`);
  }

  getJob(id: number): Observable<JobResponse> {
    return this.http.get<JobResponse>(`${this.apiUrl}/jobs/${id}`);
  }

  createJob(data: CreateJobRequest): Observable<JobResponse> {
    return this.http.post<JobResponse>(`${this.apiUrl}/jobs`, data);
  }

  updateJob(id: number, data: UpdateJobRequest): Observable<JobResponse> {
    return this.http.put<JobResponse>(`${this.apiUrl}/jobs/${id}`, data);
  }

  deleteJob(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/jobs/${id}`);
  }
}
