import { CanDeactivateFn } from '@angular/router';
import { OrderComponent } from '../order/order.component';

export const pendingorderguardGuard: CanDeactivateFn<OrderComponent> = (component, currentRoute, currentState, nextState) => {
  return component.canExit();;
};
