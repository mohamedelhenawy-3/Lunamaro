import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../../Service/auth.service';


export const customerGuard: CanActivateFn = (route, state) => {
 const auth=inject(AuthService);
 const router=inject(Router);



 const role=auth.getUserRole();
  if (role === 'Customer') {
    return true;
  }

  router.navigate(['/unauthorized']);
  return false;
};
