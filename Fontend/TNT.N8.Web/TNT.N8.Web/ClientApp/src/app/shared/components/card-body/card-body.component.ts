import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'card-body',
  template: `
    <div class="card-body">
      <ng-content></ng-content>
    </div>
  `,
  // styleUrls: ['./card-body.component.css']
  styles: ['.card-body { background: #fff; padding: 15px;}']
})
export class CardBodyComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

}
