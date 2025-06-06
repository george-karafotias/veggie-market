import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DataAnalysisComponent } from './data-analysis.component';

describe('DataAnalysisComponent', () => {
  let component: DataAnalysisComponent;
  let fixture: ComponentFixture<DataAnalysisComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [DataAnalysisComponent]
    });
    fixture = TestBed.createComponent(DataAnalysisComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
