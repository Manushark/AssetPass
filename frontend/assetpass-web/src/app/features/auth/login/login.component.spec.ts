import { describe, it, expect, beforeEach } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { LoginComponent } from './login.component';

describe('LoginComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LoginComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([]),
      ],
    }).compileComponents();
  });

  it('should create login component', () => {
    const fixture = TestBed.createComponent(LoginComponent);
    const component = fixture.componentInstance;
    expect(component).toBeTruthy();
  });

  it('should validate email and password fields', () => {
    const fixture = TestBed.createComponent(LoginComponent);
    const component = fixture.componentInstance;

    component.form.controls.email.setValue('invalid-email');
    component.form.controls.password.setValue('short');

    expect(component.form.valid).toBe(false);
    expect(component.form.controls.email.valid).toBe(false);
    expect(component.form.controls.password.valid).toBe(false);

    component.form.controls.email.setValue('user@example.com');
    component.form.controls.password.setValue('Password123!');

    expect(component.form.valid).toBe(true);
  });
});
