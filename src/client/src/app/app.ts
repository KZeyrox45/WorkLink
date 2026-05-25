import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NgIf } from '@angular/common';
import { NavbarComponent } from './components/navbar/navbar.component';
import { ToastComponent } from './components/toast/toast.component';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NgIf, NavbarComponent, ToastComponent],
  templateUrl: './app.html',
})
export class App implements OnInit {
  isAuthenticated = false;

  constructor(private auth: AuthService) {}

  ngOnInit() {
    this.isAuthenticated = this.auth.isAuthenticated();
  }
}
