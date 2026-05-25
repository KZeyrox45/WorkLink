import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NgIf, NgFor } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { JobService, CategoryDto, SkillDto, JobResponse } from '../../services/job.service';
import { ProfileService } from '../../services/profile.service';

@Component({
  selector: 'app-job-form',
  standalone: true,
  imports: [RouterLink, NgIf, NgFor, FormsModule],
  templateUrl: './job-form.component.html',
  styles: [`
    .form-page { max-width: 700px; margin: 40px auto; padding: 2rem; }
    .form-group { margin-bottom: 1rem; }
    .form-group label { display: block; margin-bottom: 0.3rem; font-weight: 500; }
    .form-group input, .form-group textarea, .form-group select { width: 100%; padding: 0.5rem; border: 1px solid #ccc; border-radius: 4px; }
    .form-group textarea { resize: vertical; min-height: 120px; }
    .form-row { display: flex; gap: 1rem; }
    .form-row .form-group { flex: 1; }
    .error { color: #d32f2f; margin-bottom: 1rem; }
    .skill-grid { display: flex; flex-wrap: wrap; gap: 0.5rem; margin-top: 0.3rem; }
    .skill-tag { padding: 0.3rem 0.6rem; background: #f5f5f5; border: 1px solid #ddd; border-radius: 16px; cursor: pointer; font-size: 0.85rem; user-select: none; }
    .skill-tag.selected { background: #e3f2fd; border-color: #1976d2; color: #1565c0; }
    button { padding: 0.6rem 1.5rem; background: #1976d2; color: white; border: none; border-radius: 4px; cursor: pointer; }
    button:disabled { opacity: 0.6; }
    button.secondary { background: #757575; margin-left: 0.5rem; }
  `]
})
export class JobFormComponent implements OnInit {
  isEdit = false;
  jobId = 0;
  title = '';
  description = '';
  budgetMin: number | null = null;
  budgetMax: number | null = null;
  durationDays: number | null = null;
  categoryId = 0;
  allCategories: CategoryDto[] = [];
  allSkills: SkillDto[] = [];
  selectedSkillIds: Set<number> = new Set();
  loading = false;
  error = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private jobService: JobService,
    private profileService: ProfileService
  ) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit = true;
      this.jobId = Number(id);
    }
    this.loadCategories();
    this.loadSkills();
    if (this.isEdit) this.loadJob();
  }

  private loadCategories() {
    this.jobService.getCategories().subscribe({
      next: (cats) => {
        this.allCategories = cats;
        if (!this.isEdit && cats.length > 0) this.categoryId = cats[0].id;
      },
    });
  }

  private loadSkills() {
    this.profileService.getSkills().subscribe({
      next: (skills) => (this.allSkills = skills),
    });
  }

  private loadJob() {
    this.jobService.getJob(this.jobId).subscribe({
      next: (job: JobResponse) => {
        this.title = job.title;
        this.description = job.description;
        this.budgetMin = job.budgetMin;
        this.budgetMax = job.budgetMax;
        this.durationDays = job.durationDays;
        this.categoryId = job.categoryId;
        this.selectedSkillIds = new Set(job.skills.map((s) => s.id));
      },
      error: () => this.router.navigate(['/jobs']),
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
    this.loading = true;
    const data = {
      title: this.title,
      description: this.description,
      budgetMin: this.budgetMin,
      budgetMax: this.budgetMax,
      durationDays: this.durationDays,
      categoryId: this.categoryId,
      skillIds: Array.from(this.selectedSkillIds),
    };

    const request = this.isEdit
      ? this.jobService.updateJob(this.jobId, data)
      : this.jobService.createJob(data);

    request.subscribe({
      next: () => this.router.navigate(['/jobs']),
      error: (err) => {
        this.error = err.error?.message || 'Failed to save job';
        this.loading = false;
      },
    });
  }
}
