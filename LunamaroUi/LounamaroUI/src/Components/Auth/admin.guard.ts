import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../../app/Service/auth.service';

export const adminGuard: CanActivateFn = (route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const role = auth.getUserRole();

  if (role === 'Admin') {
    return true;
  }

  router.navigate(['/unauthorized']);
  return false;
};
