import { ClearScript } from "./ClearScript";

export class TestComponent extends ClearScript{
   
    protected offy:float=0.01;

    Update(){
        var y=this.Entity.Transform.Position.Y;
        if(y>1){
            this.offy=-0.01;
        }else if(y<0.1){
            this.offy=0.01;
        }
        this.Entity.Transform.Position.Y+=this.offy;
    }

}