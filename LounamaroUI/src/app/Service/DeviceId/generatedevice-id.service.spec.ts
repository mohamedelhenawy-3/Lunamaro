import { TestBed } from '@angular/core/testing';

import { GeneratedeviceIdService } from './generatedevice-id.service';

describe('GeneratedeviceIdService', () => {
  let service: GeneratedeviceIdService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GeneratedeviceIdService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
