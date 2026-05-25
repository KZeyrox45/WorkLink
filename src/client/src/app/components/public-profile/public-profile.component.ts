import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { NgIf, NgFor } from '@angular/common';
import { ProfileService, PublicProfileResponse } from '../../services/profile.service';

@Component({
  selector: 'app-public-profile',
  standalone: true,
  imports: [NgIf, NgFor, RouterLink],
  templateUrl: './public-profile.component.html',
  styles: [
    `
      .profile-page {
        max-width: 500px;
        margin: 60px auto;
        padding: 2rem;
      }
      .avatar {
        width: 100px;
        height: 100px;
        border-radius: 50%;
        object-fit: cover;
        background: #e0e0e0;
      }
      .role-badge {
        display: inline-block;
        background: #e3f2fd;
        color: #1565c0;
        padding: 0.2rem 0.6rem;
        border-radius: 12px;
        font-size: 0.85rem;
        margin-top: 0.3rem;
      }
      .skill-list {
        display: flex;
        flex-wrap: wrap;
        gap: 0.4rem;
        margin-top: 0.5rem;
      }
      .skill-tag {
        padding: 0.2rem 0.6rem;
        background: #f5f5f5;
        border-radius: 12px;
        font-size: 0.85rem;
      }
      .not-found {
        color: #d32f2f;
      }
    `,
  ],
})
export class PublicProfileComponent implements OnInit {
  profile: PublicProfileResponse | null = null;
  error = '';

  constructor(
    private route: ActivatedRoute,
    private profileService: ProfileService
  ) {}

  ngOnInit() {
    const userId = this.route.snapshot.paramMap.get('id');
    if (!userId) {
      this.error = 'User not found.';
      return;
    }
    this.profileService.getPublicProfile(userId).subscribe({
      next: (p) => (this.profile = p),
      error: () => (this.error = 'User not found.'),
    });
  }
}
