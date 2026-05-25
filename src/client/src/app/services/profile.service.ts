import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface SkillDto {
  id: number;
  name: string;
}

export interface ProfileResponse {
  id: string;
  email: string;
  displayName: string;
  bio: string | null;
  avatarUrl: string | null;
  role: string;
  skills: SkillDto[];
  createdAt: string;
}

export interface PublicProfileResponse {
  id: string;
  displayName: string;
  bio: string | null;
  avatarUrl: string | null;
  role: string;
  skills: SkillDto[];
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class ProfileService {
  private apiUrl = 'http://localhost:5114/api';

  constructor(private http: HttpClient) {}

  getProfile(): Observable<ProfileResponse> {
    return this.http.get<ProfileResponse>(`${this.apiUrl}/profile`);
  }

  updateProfile(data: {
    displayName: string;
    bio: string | null;
    avatarUrl: string | null;
    skillIds: number[];
  }): Observable<ProfileResponse> {
    return this.http.put<ProfileResponse>(`${this.apiUrl}/profile`, data);
  }

  getSkills(): Observable<SkillDto[]> {
    return this.http.get<SkillDto[]>(`${this.apiUrl}/skills`);
  }

  getPublicProfile(userId: string): Observable<PublicProfileResponse> {
    return this.http.get<PublicProfileResponse>(
      `${this.apiUrl}/users/${userId}`
    );
  }
}
