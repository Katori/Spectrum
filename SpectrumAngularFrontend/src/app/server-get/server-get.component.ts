import { Component, OnInit } from '@angular/core';
import{ Server} from '../server';
import { ServerService } from '../server.service';

@Component({
  selector: 'app-server-get',
  templateUrl: './server-get.component.html',
  styleUrls: ['./server-get.component.scss']
})
export class ServerGetComponent implements OnInit {

  servers: Server[];

  constructor(private ss: ServerService) { }

  deleteServer(id) {
    this.ss.deleteServer(id).subscribe(res => {
      console.log('Deleted');
    });
  }

  ngOnInit() {
    this.ss
      .getServers()
      .subscribe((data: Server[]) => {
        this.servers = data;
    });
  }
}
