import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ServerAddComponent } from './server-add/server-add.component';
import { ServerGetComponent } from './server-get/server-get.component';
import { ServerEditComponent } from './server-edit/server-edit.component';

import {ServerService} from './server.service';

import {SlimLoadingBarModule } from 'ng2-slim-loading-bar';

import { ReactiveFormsModule } from '@angular/forms';

import { HttpClientModule } from '@angular/common/http';

@NgModule({
  declarations: [
    AppComponent,
    ServerAddComponent,
    ServerGetComponent,
    ServerEditComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    SlimLoadingBarModule,
    ReactiveFormsModule,
    HttpClientModule
  ],
  providers: [
    ServerService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
