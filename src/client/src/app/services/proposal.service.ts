import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ProposalResponse {
  id: number;
  jobId: number;
  jobTitle: string;
  freelancerId: string;
  freelancerName: string;
  coverLetter: string;
  bidAmount: number;
  estimatedDays: number;
  status: string;
  createdAt: string;
}

export interface CreateProposalRequest {
  coverLetter: string;
  bidAmount: number;
  estimatedDays: number;
}

@Injectable({ providedIn: 'root' })
export class ProposalService {
  private apiUrl = 'http://localhost:5114/api';

  constructor(private http: HttpClient) {}

  listByJob(jobId: number): Observable<ProposalResponse[]> {
    return this.http.get<ProposalResponse[]>(`${this.apiUrl}/jobs/${jobId}/proposals`);
  }

  submit(jobId: number, data: CreateProposalRequest): Observable<ProposalResponse> {
    return this.http.post<ProposalResponse>(`${this.apiUrl}/jobs/${jobId}/proposals`, data);
  }

  accept(proposalId: number): Observable<ProposalResponse> {
    return this.http.put<ProposalResponse>(`${this.apiUrl}/proposals/${proposalId}/accept`, {});
  }

  reject(proposalId: number): Observable<ProposalResponse> {
    return this.http.put<ProposalResponse>(`${this.apiUrl}/proposals/${proposalId}/reject`, {});
  }

  withdraw(proposalId: number): Observable<ProposalResponse> {
    return this.http.put<ProposalResponse>(`${this.apiUrl}/proposals/${proposalId}/withdraw`, {});
  }
}
