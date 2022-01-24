import ButtonActionTypes from "../ButtonActionTypes";
import KeyDescription from "./KeyDescription";

export default class ButtonAction {
    type!: ButtonActionTypes;
    keys!: KeyDescription[]; 
    
    constructor(){
        this.type = ButtonActionTypes.Hold;
        this.keys = [];
    }
}