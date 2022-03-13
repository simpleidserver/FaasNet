import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'edit-domain',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss']
})
export class EditDomainComponent implements OnInit {
  isLoading: boolean = false;

  constructor() { }

  ngOnInit(): void {
  }
}
