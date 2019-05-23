import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl, FormGroup,  FormBuilder,  Validators } from '@angular/forms';
import { ServerService } from '../server.service';

@Component({
  selector: 'app-server-edit',
  templateUrl: './server-edit.component.html',
  styleUrls: ['./server-edit.component.scss']
})
export class ServerEditComponent implements OnInit {

  server: any = {};
  angForm: FormGroup;

  constructor(private route: ActivatedRoute,
    private router: Router,
    private ss: ServerService,
    private fb: FormBuilder) {
      this.createForm();
 }

//  get address():any{return this.server.address}
//  get port():any{return this.server.port}
//  get playedGames():any{return this.server.playedGames}

  createForm() {
    this.angForm = this.fb.group({
        address: ['', Validators.required ],
        port: ['', Validators.required ],
        playedGames: ['0', Validators.required ]
      });
      this.angForm.controls['address'].setValue(this.server.address);
    this.angForm.get('address').setValue(this.server.address);
    }

    updateServer(address, port, playedGames) {
      this.route.params.subscribe(params => {
         this.ss.updateServer(address, port, playedGames, params['id']);
         this.router.navigate(['server']);
   });
  }

  ngOnInit() {
    this.route.params.subscribe(params => {
        this.ss.editServer(params['id']).subscribe(res => {
          this.server = res;
      });
    });
    console.log(this.server);
    this.angForm.get('port').setValue(this.server.port);
    this.angForm.get('playedGames').setValue(this.server.playedGames);
  }

}
