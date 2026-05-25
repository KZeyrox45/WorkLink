import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [],
  templateUrl: './dashboard.component.html',
  styles: [
    `
      .dashboard {
        max-width: 800px;
        margin: 40px auto;
        padding: 2rem;
      }
      .header {
        display: flex;
        justify-content: space-between;
        align-items: center;
      }
      .role-badge {
        background: #e3f2fd;
        color: #1565c0;
        padding: 0.2rem 0.6rem;
        border-radius: 12px;
        font-size: 0.85rem;
      }
      button {
        padding: 0.5rem 1rem;
        background: #d32f2f;
        color: white;
        border: none;
        border-radius: 4px;
        cursor: pointer;
      }
    `,
  ],
})
export class DashboardComponent implements OnInit {
  user: { email: string; name: string; role: string } | null = null;

  constructor(
    private auth: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.user = this.auth.getUserInfo();
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
