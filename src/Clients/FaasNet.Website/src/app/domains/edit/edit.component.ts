import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'edit-domain',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss']
})
export class EditDomainComponent implements OnInit {
  isLoading: boolean = false;
  id: string = "";

  constructor(private route : ActivatedRoute) { }

  ngOnInit(): void {
    this.id = this.route.snapshot.params['id'];
    console.log(this.id);
  }
}
