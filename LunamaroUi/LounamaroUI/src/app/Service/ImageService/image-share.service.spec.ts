import { TestBed } from '@angular/core/testing';

import { ImageShareService } from './image-share.service';

describe('ImageShareService', () => {
  let service: ImageShareService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ImageShareService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
