import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="dashboard-container">
      <header class="navbar">
        <h1 class="logo">Asset<span>Pass</span></h1>
        <div class="user-menu">
          <span class="user-email">{{ authService.currentUser()?.email }}</span>
          <span class="user-role badge">{{ authService.currentUser()?.role }}</span>
          <button (click)="onLogout()" class="btn-logout">Cerrar Sesión</button>
        </div>
      </header>

      <main class="content">
        <h2>Bienvenido a AssetPass</h2>
        <p>Panel principal de gestión de activos y pasaportes digitales.</p>
        <div class="tenant-info card">
          <h3>Información de Sesión</h3>
          <p><strong>Tenant ID:</strong> {{ authService.currentUser()?.tenantId }}</p>
          <p><strong>Usuario ID:</strong> {{ authService.currentUser()?.id }}</p>
        </div>
      </main>
    </div>
  `,
  styles: [
    `
      .dashboard-container {
        min-height: 100vh;
        background: #f8fafc;
        font-family: inherit;
      }
      .navbar {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 1rem 2rem;
        background: #ffffff;
        border-bottom: 1px solid #e2e8f0;
      }
      .logo {
        font-size: 1.5rem;
        font-weight: 700;
        margin: 0;
        color: #1e293b;
      }
      .logo span {
        color: #2563eb;
      }
      .user-menu {
        display: flex;
        align-items: center;
        gap: 1rem;
      }
      .badge {
        background: #dbeafe;
        color: #1e40af;
        padding: 0.25rem 0.5rem;
        border-radius: 4px;
        font-size: 0.75rem;
        font-weight: 600;
      }
      .btn-logout {
        padding: 0.5rem 1rem;
        background: #f1f5f9;
        border: 1px solid #cbd5e1;
        border-radius: 6px;
        cursor: pointer;
        font-size: 0.875rem;
      }
      .btn-logout:hover {
        background: #e2e8f0;
      }
      .content {
        padding: 2rem;
        max-width: 1000px;
        margin: 0 auto;
      }
      .card {
        background: #ffffff;
        padding: 1.5rem;
        border-radius: 8px;
        border: 1px solid #e2e8f0;
        margin-top: 1.5rem;
      }
    `,
  ],
})
export class DashboardComponent {
  readonly authService = inject(AuthService);

  onLogout(): void {
    this.authService.logout();
  }
}
