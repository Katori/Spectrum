export class Server {
  public id:Number;
  public address:String;
  public port:Number;
  public players:Number;
  public playedGames:Number;
  get connectionString():String {
      return this.address+":"+this.port;
  }
  set connectionString(newConnString:String) {
      var c = newConnString.split(':');
      this.address = c[0];
      this.port = parseInt(c[1]);
  }
}
