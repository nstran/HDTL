import { ChangeDetectorRef, Component, ElementRef, OnInit, ViewChild } from '@angular/core';

@Component({
  selector: 'sub-header',
  templateUrl: './sub-header.component.html',
  styleUrls: ['./sub-header.component.css']
})
export class SubHeaderComponent {
	constructor(private cdRef: ChangeDetectorRef) {
  }
  
	ngAfterViewChecked() {
		this.cdRef.detectChanges()
  }

}
