import { TestBed } from '@angular/core/testing';

import { CacheWarmerService } from './cache-warmer.service';

describe('CacheWarmerService', () => {
  let service: CacheWarmerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CacheWarmerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
