import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'messages-domain',
  templateUrl: './messages.component.html'
})
export class MessagesDomainComponent implements OnInit {
  isLoading: boolean = false;

  constructor() { }

  ngOnInit(): void {
  }
}
