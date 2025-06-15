import { TestBed } from '@angular/core/testing';

import { UsercartService } from './usercart.service';

describe('UsercartService', () => {
  let service: UsercartService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UsercartService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
