import { Component, OnInit } from '@angular/core';
import { FormGroup,  FormBuilder,  Validators } from '@angular/forms';

import { ServerService } from '../server.service';

@Component({
  selector: 'app-server-add',
  templateUrl: './server-add.component.html',
  styleUrls: ['./server-add.component.scss']
})
export class ServerAddComponent implements OnInit {

  angForm: FormGroup;
  constructor(private fb: FormBuilder, private ss: ServerService) {
    this.createForm();
  }

  createForm() {
    this.angForm = this.fb.group({
      address: ['', Validators.required ],
      port: ['', Validators.required ],
      playedGames: ['', Validators.required ]
    });
  }

  addServer(address, port, playedGames) {
    this.ss.addServer(address, port, playedGames);
  }

  ngOnInit() {
  }

}
