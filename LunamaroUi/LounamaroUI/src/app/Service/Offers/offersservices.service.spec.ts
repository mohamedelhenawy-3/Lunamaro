import { TestBed } from '@angular/core/testing';

import { OffersservicesService } from './offersservices.service';

describe('OffersservicesService', () => {
  let service: OffersservicesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OffersservicesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
