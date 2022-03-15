import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'editor-domain',
  templateUrl: './editor.component.html',
  styleUrls: ['./editor.component.scss']
})
export class EditorDomainComponent implements OnInit {
  isLoading: boolean = false;

  constructor() { }

  ngOnInit(): void {
  }
}
