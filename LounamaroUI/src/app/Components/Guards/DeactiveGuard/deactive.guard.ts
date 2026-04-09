import { CanDeactivateFn } from '@angular/router';
import { AddItemComponent } from '../../Admin/add-item/add-item.component';

export const deactiveGuard: CanDeactivateFn<AddItemComponent> = (component, currentRoute, currentState, nextState) => {
  return component.canExit();
};
