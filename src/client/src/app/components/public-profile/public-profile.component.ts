import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { NgIf, NgFor, DatePipe } from '@angular/common';
import { ProfileService, PublicProfileResponse } from '../../services/profile.service';
import { ReviewService, ReviewResponse } from '../../services/review.service';

@Component({
  selector: 'app-public-profile',
  standalone: true,
  imports: [NgIf, NgFor, DatePipe, RouterLink],
  templateUrl: './public-profile.component.html',
  styles: [`
    .profile-page { max-width: 600px; margin: 60px auto; padding: 2rem; }
    .avatar { width: 100px; height: 100px; border-radius: 50%; object-fit: cover; background: #e0e0e0; }
    .role-badge { display: inline-block; background: #e3f2fd; color: #1565c0; padding: 0.2rem 0.6rem; border-radius: 12px; font-size: 0.85rem; margin-top: 0.3rem; }
    .skill-list { display: flex; flex-wrap: wrap; gap: 0.4rem; margin-top: 0.5rem; }
    .skill-tag { padding: 0.2rem 0.6rem; background: #f5f5f5; border-radius: 12px; font-size: 0.85rem; }
    .not-found { color: #d32f2f; }
    .rating { margin-top: 0.5rem; }
    .star { color: #fbc02d; font-size: 1.1rem; }
    .star.empty { color: #ccc; }
    .review-card { border: 1px solid #e0e0e0; border-radius: 8px; padding: 1rem; margin-bottom: 0.8rem; }
    .review-card .header { display: flex; justify-content: space-between; align-items: flex-start; }
    .review-card .meta { font-size: 0.85rem; color: #666; }
    .section-title { margin-top: 1.5rem; margin-bottom: 0.5rem; font-weight: 600; }
  `]
})
export class PublicProfileComponent implements OnInit {
  profile: PublicProfileResponse | null = null;
  reviews: ReviewResponse[] = [];
  error = '';

  constructor(
    private route: ActivatedRoute,
    private profileService: ProfileService,
    private reviewService: ReviewService
  ) {}

  ngOnInit() {
    const userId = this.route.snapshot.paramMap.get('id');
    if (!userId) {
      this.error = 'User not found.';
      return;
    }
    this.profileService.getPublicProfile(userId).subscribe({
      next: (p) => {
        this.profile = p;
        this.loadReviews(userId);
      },
      error: () => (this.error = 'User not found.'),
    });
  }

  private loadReviews(userId: string) {
    this.reviewService.listByUser(userId).subscribe({
      next: (reviews) => (this.reviews = reviews),
    });
  }

  starsArray(): number[] {
    return [1, 2, 3, 4, 5];
  }

  get roundedRating(): number {
    return Math.round(this.profile?.averageRating ?? 0);
  }
}
