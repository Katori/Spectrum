import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {ServerAddComponent} from './server-add/server-add.component';
import {ServerEditComponent} from './server-edit/server-edit.component';
import {ServerGetComponent} from './server-get/server-get.component';

const routes: Routes = [ {
  path: 'server/create',
  component: ServerAddComponent
},
{
  path: 'server/edit/:id',
  component: ServerEditComponent
},
{
  path: 'server',
  component: ServerGetComponent
}];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
