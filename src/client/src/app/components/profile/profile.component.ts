import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgIf, NgFor } from '@angular/common';
import { Router } from '@angular/router';
import { RouterLink } from '@angular/router';
import { ProfileService, SkillDto } from '../../services/profile.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [FormsModule, NgIf, NgFor, RouterLink],
  templateUrl: './profile.component.html',
  styles: [
    `
      .profile-page {
        max-width: 600px;
        margin: 40px auto;
        padding: 2rem;
      }
      .form-group {
        margin-bottom: 1rem;
      }
      .form-group label {
        display: block;
        margin-bottom: 0.3rem;
        font-weight: 500;
      }
      .form-group input,
      .form-group textarea {
        width: 100%;
        padding: 0.5rem;
        border: 1px solid #ccc;
        border-radius: 4px;
      }
      .form-group textarea {
        resize: vertical;
        min-height: 80px;
      }
      .error {
        color: #d32f2f;
        margin-bottom: 1rem;
      }
      .success {
        color: #2e7d32;
        margin-bottom: 1rem;
      }
      .skill-grid {
        display: flex;
        flex-wrap: wrap;
        gap: 0.5rem;
        margin-top: 0.3rem;
      }
      .skill-tag {
        display: flex;
        align-items: center;
        gap: 0.3rem;
        padding: 0.3rem 0.6rem;
        background: #f5f5f5;
        border: 1px solid #ddd;
        border-radius: 16px;
        cursor: pointer;
        font-size: 0.85rem;
        user-select: none;
      }
      .skill-tag.selected {
        background: #e3f2fd;
        border-color: #1976d2;
        color: #1565c0;
      }
      button {
        padding: 0.6rem 1.5rem;
        background: #1976d2;
        color: white;
        border: none;
        border-radius: 4px;
        cursor: pointer;
      }
      button:disabled {
        opacity: 0.6;
      }
      button.secondary {
        background: #757575;
        margin-left: 0.5rem;
      }
    `,
  ],
})
export class ProfileComponent implements OnInit {
  displayName = '';
  bio = '';
  avatarUrl = '';
  allSkills: SkillDto[] = [];
  selectedSkillIds: Set<number> = new Set();
  loading = false;
  error = '';
  saved = false;

  constructor(
    private profileService: ProfileService,
    private auth: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadProfile();
    this.loadSkills();
  }

  private loadProfile() {
    this.profileService.getProfile().subscribe({
      next: (profile) => {
        this.displayName = profile.displayName;
        this.bio = profile.bio || '';
        this.avatarUrl = profile.avatarUrl || '';
        this.selectedSkillIds = new Set(profile.skills.map((s) => s.id));
      },
      error: () => this.router.navigate(['/login']),
    });
  }

  private loadSkills() {
    this.profileService.getSkills().subscribe({
      next: (skills) => (this.allSkills = skills),
    });
  }

  toggleSkill(skillId: number) {
    if (this.selectedSkillIds.has(skillId)) {
      this.selectedSkillIds.delete(skillId);
    } else {
      this.selectedSkillIds.add(skillId);
    }
  }

  onSubmit() {
    this.error = '';
    this.saved = false;
    this.loading = true;
    this.profileService
      .updateProfile({
        displayName: this.displayName,
        bio: this.bio || null,
        avatarUrl: this.avatarUrl || null,
        skillIds: Array.from(this.selectedSkillIds),
      })
      .subscribe({
        next: () => {
          this.saved = true;
          this.loading = false;
        },
        error: (err) => {
          this.error = err.error?.message || 'Update failed';
          this.loading = false;
        },
      });
  }
}
