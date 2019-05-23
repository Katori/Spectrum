import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import {Server} from './server';

@Injectable({
  providedIn: 'root'
})
export class ServerService {
  uri = 'https://mydomain.com:443/api/Servers';

  private headers : HttpHeaders;

  constructor(private http: HttpClient) {
    this.headers = new HttpHeaders({'Content-Type':'application/json; charset=utf-8'});
  }

  getServers() {
    return this
           .http
           .get(`${this.uri}`);
  }

  addServer(address, port, playedGames) {
    const obj : Server = {
      id: undefined,
      address: address,
      port: port,
      players: 0,
      playedGames: playedGames,
      connectionString: address+":"+port
    };
    console.log(obj);
    this.http.post(this.uri, obj, {headers : this.headers})
        .subscribe(res => console.log('Done'));
  }

  editServer(id) {
    return this
            .http
            .get(`${this.uri}/${id}`);
  }

  deleteServer(id) {
    return this
              .http
              .delete(`${this.uri}/${id}`);
  }

    updateServer(address, port, playedGames, id) {

      const obj = {
        id: id,
        address: address,
        port: port,
        playedGames: playedGames
        };
      this
        .http
        .put(`${this.uri}/${id}`, obj)
        .subscribe(res => console.log('Done'));
    }
}
