import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'messageseditor-appdomain',
  templateUrl: './messageseditor.component.html'
})
export class AppDomainMessagesEditorComponent implements OnInit {
  appDomainId: string = "";

  constructor(private activatedRoute: ActivatedRoute) { }

  ngOnInit() {
    this.appDomainId = this.activatedRoute.parent?.snapshot.params['appDomainId'];
  }
}
