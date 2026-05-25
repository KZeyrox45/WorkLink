import { Component, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { NgIf } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, NgIf],
  template: `
    <nav class="navbar">
      <div class="nav-inner">
        <a routerLink="/dashboard" class="nav-brand">WorkLink</a>
        <button class="hamburger" (click)="mobileOpen = !mobileOpen" aria-label="Toggle navigation">
          <span></span><span></span><span></span>
        </button>
        <div class="nav-links" [class.open]="mobileOpen">
          <a routerLink="/jobs" routerLinkActive="active" [routerLinkActiveOptions]="{exact:true}">Browse Jobs</a>
          <a *ngIf="user?.role === 'Client'" routerLink="/my-jobs" routerLinkActive="active">My Jobs</a>
          <a *ngIf="user?.role === 'Freelancer'" routerLink="/my-proposals" routerLinkActive="active">My Proposals</a>
          <a routerLink="/profile" routerLinkActive="active">Profile</a>
          <span class="nav-user">{{ user?.name }}</span>
          <button class="nav-logout" (click)="logout()">Logout</button>
        </div>
      </div>
    </nav>
  `,
  styles: [`
    .navbar {
      background: #1a1a2e;
      color: white;
      padding: 0 1rem;
      position: sticky;
      top: 0;
      z-index: 1000;
    }
    .nav-inner {
      max-width: 1200px;
      margin: 0 auto;
      display: flex;
      align-items: center;
      justify-content: space-between;
      height: 56px;
    }
    .nav-brand {
      color: white;
      text-decoration: none;
      font-weight: 700;
      font-size: 1.2rem;
    }
    .nav-links {
      display: flex;
      align-items: center;
      gap: 1rem;
    }
    .nav-links a {
      color: #ccc;
      text-decoration: none;
      font-size: 0.9rem;
      padding: 0.3rem 0;
    }
    .nav-links a:hover, .nav-links a.active {
      color: white;
    }
    .nav-user {
      font-size: 0.85rem;
      color: #999;
      margin-left: 0.5rem;
    }
    .nav-logout {
      background: #d32f2f;
      color: white;
      border: none;
      padding: 0.35rem 0.8rem;
      border-radius: 4px;
      cursor: pointer;
      font-size: 0.85rem;
    }
    .hamburger {
      display: none;
      flex-direction: column;
      gap: 4px;
      background: none;
      border: none;
      cursor: pointer;
      padding: 4px;
    }
    .hamburger span {
      display: block;
      width: 22px;
      height: 2px;
      background: white;
    }
    @media (max-width: 640px) {
      .hamburger { display: flex; }
      .nav-links {
        display: none;
        position: absolute;
        top: 56px;
        left: 0;
        right: 0;
        background: #1a1a2e;
        flex-direction: column;
        padding: 1rem;
        gap: 0.8rem;
      }
      .nav-links.open { display: flex; }
    }
  `]
})
export class NavbarComponent implements OnInit {
  user: { email: string; name: string; role: string } | null = null;
  mobileOpen = false;

  constructor(private auth: AuthService, private router: Router) {}

  ngOnInit() {
    this.user = this.auth.getUserInfo();
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
