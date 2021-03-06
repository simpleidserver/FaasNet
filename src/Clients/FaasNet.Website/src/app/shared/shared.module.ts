import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { TranslateObjPipe } from '../pipes/translateobj.pipe';

@NgModule({
  imports: [
  ],
  declarations: [
    TranslateObjPipe
  ],
  exports: [
    CommonModule,
    RouterModule,
    TranslateModule,
    TranslateObjPipe
  ]
})

export class SharedModule { }
